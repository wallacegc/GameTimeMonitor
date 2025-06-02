using GameTimeMonitor.Controllers;
using GameTimeMonitor.Models;
using GameTimeMonitor.Services;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using System.Drawing;
using System.Windows.Forms;

namespace GameTimeMonitor.Views
{
    public partial class Form1 : Form
    {
        private readonly GameController _gameController;
        private readonly GameMonitoringService _gameMonitoringService;
        private readonly DatabaseService _databaseService;

        public Form1()
        {
            _databaseService = new DatabaseService();
            _gameController = new GameController(_databaseService);
            _gameMonitoringService = new GameMonitoringService(_databaseService);

            InitializeComponent();
            InitializeToolStrip();
            Load += Form1_Load;

            _gameMonitoringService.GameStatusChanged += UpdateStatus;
            _gameMonitoringService.GameUpdated += () =>
            {
                if (InvokeRequired)
                    Invoke(DisplayAllGamesTime);
                else
                    DisplayAllGamesTime();
            };
        }

        // Loads the database and game information when the form loads
        private void Form1_Load(object sender, EventArgs e)
        {
            _databaseService.InitializeDatabase();
            _gameController.LoadGames();
            _gameMonitoringService.StartMonitoring(_gameController.GetGames());
            DisplayAllGamesTime();
        }

        // Initializes the toolbar with buttons for Add Game, Update, and Remove Duplicates
        private void InitializeToolStrip()
        {
            ToolStrip toolStrip = new ToolStrip();

            ToolStripButton addGameButton = new ToolStripButton("Add Game");
            addGameButton.Click += AddGameButton_Click;
            toolStrip.Items.Add(addGameButton);

            ToolStripButton updateButton = new ToolStripButton("Update");
            updateButton.Click += (s, e) =>
            {
                try
                {
                    new BackupService().UpdateGamesJson();
                    _gameController.LoadGames();
                    DisplayAllGamesTime();
                    MessageBox.Show("games.json successfully updated!", "Success");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating: {ex.Message}", "Error");
                }
            };
            toolStrip.Items.Add(updateButton);

            ToolStripButton removeDuplicatesButton = new ToolStripButton("Remove Duplicates");
            removeDuplicatesButton.Click += (s, e) =>
            {
                try
                {
                    int count = new DuplicateCheckService().RemoveDuplicateSessions();
                    _gameController.LoadGames();
                    DisplayAllGamesTime();
                    MessageBox.Show($"{count} duplicate sessions removed.", "Success");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error removing duplicates: {ex.Message}", "Error");
                }
            };
            toolStrip.Items.Add(removeDuplicatesButton);

            toolStrip.Dock = DockStyle.Top;
            Controls.Add(toolStrip);
        }

        // Handles the Add Game button click to show a form and add a new game
        private void AddGameButton_Click(object sender, EventArgs e)
        {
            using var addGameForm = new AddGameForm();
            if (addGameForm.ShowDialog() == DialogResult.OK)
            {
                var newGame = new Game { Name = addGameForm.GameName, Process = addGameForm.GameProcess };
                _gameController.AddGame(newGame);
                DisplayAllGamesTime();
                MessageBox.Show("Game successfully added!");
            }
        }

        // Displays all games and their playtime metrics in the UI
        private void DisplayAllGamesTime()
        {
            flowLayoutPanelGames.SuspendLayout();
            flowLayoutPanelGames.Controls.Clear();

            var today = DateTime.Today;
            var startWeek = today.AddDays(-((int)today.DayOfWeek == 0 ? 6 : (int)today.DayOfWeek - 1));
            var startMonth = new DateTime(today.Year, today.Month, 1);

            var gamesWithTimes = _gameController.GetGames()
                .Select(game => new
                {
                    Game = game,
                    TimeToday = _databaseService.GetGameTime(game.Name, today, DateTime.Now),
                    TimeWeek = _databaseService.GetGameTime(game.Name, startWeek, DateTime.Now),
                    TimeMonth = _databaseService.GetGameTime(game.Name, startMonth, DateTime.Now),
                    TimeTotal = _databaseService.GetGameTime(game.Name, DateTime.MinValue, DateTime.Now)
                })
                .OrderByDescending(g => g.TimeTotal)
                .ToList();

            foreach (var g in gamesWithTimes)
            {
                var sessions = _databaseService.GetSessionsForGame(g.Game.Name);
                TimeSpan longestSession = TimeSpan.Zero;
                DateTime longestSessionDate = DateTime.MinValue;
                Dictionary<DateTime, double> hoursPerDay = new();

                foreach (var session in sessions)
                {
                    var duration = session.EndTime - session.StartTime;
                    if (duration > longestSession)
                    {
                        longestSession = duration;
                        longestSessionDate = session.StartTime.Date;
                    }
                    var day = session.StartTime.Date;
                    if (!hoursPerDay.ContainsKey(day)) hoursPerDay[day] = 0;
                    hoursPerDay[day] += duration.TotalMinutes;
                }

                var maxDay = hoursPerDay.OrderByDescending(d => d.Value).FirstOrDefault();

                var group = new GroupBox
                {
                    Text = g.Game.Name + (g.Game.Process == "no_process" ? " ⚠️ (Missing Process)" : ""),
                    ForeColor = g.Game.Process == "no_process" ? Color.FromArgb(200, 0, 0) : SystemColors.ControlText,
                    Width = 740,
                    AutoSize = true,
                    Padding = new Padding(10)
                };

                var label = new Label
                {
                    AutoSize = true,
                    MaximumSize = new Size(700, 0),
                    Margin = new Padding(0, 0, 0, 10),
                    ForeColor = g.Game.Process == "no_process" ? Color.FromArgb(200, 0, 0) : SystemColors.ControlText,
                    Text =
                        $"Today: {FormatTime(g.TimeToday)} | Week: {FormatTime(g.TimeWeek)} | Month: {FormatTime(g.TimeMonth)} | Total: {FormatTime(g.TimeTotal)}\n" +
                        $"🏆 Longest Session: {FormatTime(longestSession.TotalMinutes)} on {longestSessionDate:MM/dd/yyyy}\n" +
                        $"📆 Most Played Day: {maxDay.Key.ToShortDateString()} - {FormatTime(maxDay.Value)}" +
                        (g.Game.Process == "no_process" ? "\n⚠️ Please update the game process/path!" : "")
                };

                var innerLayout = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    AutoSize = true,
                    ColumnCount = 1
                };

                innerLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                innerLayout.Controls.Add(label, 0, 0);

                group.Click += (s, e) => ShowGameDetails(g.Game);
                label.Click += (s, e) => ShowGameDetails(g.Game);
                group.Controls.Add(innerLayout);

                flowLayoutPanelGames.Controls.Add(group);
            }

            flowLayoutPanelGames.ResumeLayout();
        }

        // Updates the status label when a game status changes
        private void UpdateStatus(string gameName, string status)
        {
            string message = status == "Running" ? $"Game running: {gameName}" : $"Game: {gameName} - {status}";
            if (labelStatus.InvokeRequired)
                labelStatus.Invoke(() => labelStatus.Text = message);
            else
                labelStatus.Text = message;
        }

        // Shows a modal with the game session history
        private void ShowGameDetails(Game game)
        {
            using var historyForm = new GameHistoryForm(game, _databaseService, _gameController);
            historyForm.ShowDialog();
            DisplayAllGamesTime();
        }

        // Formats time in minutes into readable string (e.g., 1:45 h or 30 min)
        private string FormatTime(double timeInMinutes)
        {
            if (timeInMinutes < 60)
                return $"{(int)Math.Round(timeInMinutes)} min";

            int hours = (int)(timeInMinutes / 60);
            int minutes = (int)(timeInMinutes % 60);
            return $"{hours}:{minutes:D2} h";
        }
    }
}
