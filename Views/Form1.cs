using GameTimeMonitor.Controllers;
using GameTimeMonitor.Models;
using GameTimeMonitor.Services;
using Microsoft.Data.Sqlite;
using System.Drawing;
using System.Windows.Forms;

namespace GameTimeMonitor.Views
{
    public partial class Form1 : Form
    {
        private readonly GameController _gameController;
        private readonly GameMonitoringService _gameMonitoringService;
        private readonly DatabaseService _databaseService;

        private NotifyIcon trayIcon;
        private ContextMenuStrip trayMenu;

        public Form1()
        {
            _databaseService = new DatabaseService();
            _gameController = new GameController(_databaseService);
            _gameMonitoringService = new GameMonitoringService(_databaseService);

            InitializeComponent();
            InitializeMenuBar();

            trayMenu = new ContextMenuStrip();
            trayMenu.Items.Add("Restore", null, (s, e) => RestoreFromTray());
            trayMenu.Items.Add("Exit", null, (s, e) => Application.Exit());

            trayIcon = new NotifyIcon
            {
                Icon = SystemIcons.Application,
                ContextMenuStrip = trayMenu,
                Text = "GameTimeMonitor",
                Visible = true
            };
            trayIcon.DoubleClick += (s, e) => RestoreFromTray();
            this.Resize += Form1_Resize;
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

        private void Form1_Load(object sender, EventArgs e)
        {
            _databaseService.InitializeDatabase();
            _gameController.LoadGames();
            _gameMonitoringService.StartMonitoring(_gameController.GetGames());
            DisplayAllGamesTime();
        }

        private void InitializeMenuBar()
        {
            var menuStrip = new MenuStrip();

            // FILE
            var fileMenu = new ToolStripMenuItem("File");
            fileMenu.DropDownItems.Add("Exit", null, (s, e) => Application.Exit());

            // GAMES
            var gamesMenu = new ToolStripMenuItem("Games");
            gamesMenu.DropDownItems.Add("Add Game", null, AddGameButton_Click);
            gamesMenu.DropDownItems.Add("Update", null, (s, e) =>
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
            });
            gamesMenu.DropDownItems.Add("Remove Duplicates", null, (s, e) =>
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
            });

            // SETTINGS
            var settingsMenu = new ToolStripMenuItem("Settings");
            settingsMenu.DropDownItems.Add("Options", null, (s, e) =>
            {
                using var optionsForm = new OptionsForm();
                optionsForm.ShowDialog();
            });

            menuStrip.Items.Add(fileMenu);
            menuStrip.Items.Add(gamesMenu);
            menuStrip.Items.Add(settingsMenu);

            MainMenuStrip = menuStrip;
            Controls.Add(menuStrip);
        }

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

        private void DisplayAllGamesTime()
        {
            flowLayoutPanelGames.SuspendLayout();
            flowLayoutPanelGames.Controls.Clear();

            var today = DateTime.Today;
            var startWeek = GetStartOfWeek(today);
            var startMonth = new DateTime(today.Year, today.Month, 1);
            var startYear = new DateTime(today.Year, 1, 1);

            string selectedFilter = comboBoxTimeFilter?.SelectedItem?.ToString() ?? "Total";

            var gamesWithTimes = _gameController.GetGames()
                .Select(game => new
                {
                    Game = game,
                    TimeToday = _databaseService.GetGameTime(game.Name, today, DateTime.Now),
                    TimeWeek = _databaseService.GetGameTime(game.Name, startWeek, DateTime.Now),
                    TimeMonth = _databaseService.GetGameTime(game.Name, startMonth, DateTime.Now),
                    TimeYear = _databaseService.GetGameTime(game.Name, startYear, DateTime.Now),
                    TimeTotal = _databaseService.GetGameTime(game.Name, DateTime.MinValue, DateTime.Now)
                })
                .OrderByDescending(g => selectedFilter switch
                {
                    "Today" => g.TimeToday,
                    "Week" => g.TimeWeek,
                    "Month" => g.TimeMonth,
                    "Year" => g.TimeYear,
                    _ => g.TimeTotal
                })
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
                        $"Today: {FormatTime(g.TimeToday)} | Week: {FormatTime(g.TimeWeek)} | Month: {FormatTime(g.TimeMonth)} | Year: {FormatTime(g.TimeYear)} | Total: {FormatTime(g.TimeTotal)}\n" +
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

        private void UpdateStatus(string gameName, string status)
        {
            string message = status == "Running" ? $"Game running: {gameName}" : $"Game: {gameName} - {status}";
            if (labelStatus.InvokeRequired)
                labelStatus.Invoke(() => labelStatus.Text = message);
            else
                labelStatus.Text = message;
        }

        private void ShowGameDetails(Game game)
        {
            using var historyForm = new GameHistoryForm(game, _databaseService, _gameController);
            historyForm.ShowDialog();
            DisplayAllGamesTime();
        }

        private string FormatTime(double timeInMinutes)
        {
            if (timeInMinutes < 60)
                return $"{(int)Math.Round(timeInMinutes)} min";

            int hours = (int)(timeInMinutes / 60);
            int minutes = (int)(timeInMinutes % 60);
            return $"{hours}:{minutes:D2} h";
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
                trayIcon.ShowBalloonTip(
                    1000,
                    "Minimized",
                    "GameTimeMonitor is now running in the system tray.",
                    ToolTipIcon.Info
                );
            }
        }

        private void RestoreFromTray()
        {
            Show();
            WindowState = FormWindowState.Normal;
            Activate();
        }

        private void ComboBoxTimeFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisplayAllGamesTime();
        }
        private DateTime GetStartOfWeek(DateTime date)
        {
            DayOfWeek day = date.DayOfWeek;
            int diff = (7 + (day - DayOfWeek.Monday)) % 7;
            return date.AddDays(-1 * diff).Date;
        }
    }
}
