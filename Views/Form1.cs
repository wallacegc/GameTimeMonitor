using GameTimeMonitor.Controllers;
using GameTimeMonitor.Models;
using GameTimeMonitor.Services;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;

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

            // Initializing the toolbar and connecting the event handlers
            InitializeToolStrip();
            this.Load += Form1_Load;

            _databaseService.InitializeDatabase();
            _gameController.LoadGames();
            _gameMonitoringService.StartMonitoring(_gameController.GetGames());

            // Event handlers for game status and game updates
            _gameMonitoringService.GameStatusChanged += UpdateStatus;
            _gameMonitoringService.GameUpdated += () =>
            {
                if (InvokeRequired)
                {
                    Invoke(() => DisplayAllGamesTime());
                }
                else
                {
                    DisplayAllGamesTime();
                }
            };
        }

        // Initializes the ToolStrip and adds the "Add Game", "Backup" and "Restore" buttons
        private void InitializeToolStrip()
        {
            ToolStrip toolStrip = new ToolStrip();

            // Botão de adicionar jogo
            ToolStripButton addGameButton = new ToolStripButton("Add Game");
            addGameButton.Click += AddGameButton_Click;
            toolStrip.Items.Add(addGameButton);

            // Botão para atualizar o arquivo games.json
            ToolStripButton updateButton = new ToolStripButton("Update");
            updateButton.Click += (sender, e) =>
            {
                try
                {
                    var backupService = new BackupService();
                    backupService.UpdateGamesJson();

                    _gameController.LoadGames();
                    DisplayAllGamesTime();

                    MessageBox.Show("games.json successfully updated!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating games.json: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            toolStrip.Items.Add(updateButton);

            // Button to remove duplicate records
            ToolStripButton removeDuplicatesButton = new ToolStripButton("Remove Duplicates");
            removeDuplicatesButton.Click += (sender, e) =>
            {
                try
                {
                    var duplicateService = new DuplicateCheckService();
                    int removedCount = duplicateService.RemoveDuplicateSessions();

                    _gameController.LoadGames();
                    DisplayAllGamesTime();
                    MessageBox.Show($"{removedCount} duplicate sessions removed.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error while removing duplicates: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            toolStrip.Items.Add(removeDuplicatesButton);

            this.Controls.Add(toolStrip);
        }

        // Event handler for the "Add Game" button click
        private void AddGameButton_Click(object sender, EventArgs e)
        {
            using (var addGameForm = new AddGameForm())
            {
                var result = addGameForm.ShowDialog();

                if (result == DialogResult.OK)
                {
                    string gameName = addGameForm.GameName;
                    string gameProcess = addGameForm.GameProcess;

                    var newGame = new Game
                    {
                        Name = gameName,
                        Process = gameProcess
                    };

                    _gameController.AddGame(newGame);
                    DisplayAllGamesTime();

                    MessageBox.Show("Game added successfully!");
                }
            }
        }

        // Called when the form is loaded, loading and displaying games
        private void Form1_Load(object sender, EventArgs e)
        {
            _gameController.LoadGames();
            DisplayAllGamesTime();
        }

        // Displays all the games and their play time on the FlowLayoutPanel
        private void DisplayAllGamesTime()
        {
            flowLayoutPanelGames.Controls.Clear();

            var today = DateTime.Today;
            var startWeek = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday); // começa na segunda
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
                .OrderByDescending(g => g.TimeTotal) // ordena pelo tempo total decrescente
                .ToList();

            foreach (var g in gamesWithTimes)
            {
                string FormatTime(double timeInMinutes)
                {
                    if (timeInMinutes < 60)
                    {
                        int roundedMinutes = (int)Math.Round(timeInMinutes);
                        return $"{roundedMinutes} min";
                    }
                    else
                    {
                        int hours = (int)(timeInMinutes / 60);
                        int minutes = (int)(timeInMinutes % 60);
                        return $"{hours}:{minutes:D2} h";
                    }
                }

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
                    Text = g.Game.Name,
                    Width = 740,
                    AutoSize = true,
                    Padding = new Padding(10),
                    AutoSizeMode = AutoSizeMode.GrowAndShrink
                };

                var label = new Label
                {
                    AutoSize = true,
                    MaximumSize = new Size(700, 0)
                };

                // Texto base para exibir os tempos
                string baseText =
                    $"Today: {FormatTime(g.TimeToday)} | Week: {FormatTime(g.TimeWeek)} | Month: {FormatTime(g.TimeMonth)} | Total: {FormatTime(g.TimeTotal)}\n" +
                    $"🏆 Longest Session: {FormatTime(longestSession.TotalMinutes)} on {longestSessionDate:dd/MM/yyyy}\n" +
                    $"📆 Most Played Day: {maxDay.Key.ToShortDateString()} - {FormatTime(maxDay.Value)}";

                if (g.Game.Process == "no_process")
                {
                    // Seta o nome do grupo em vermelho
                    group.ForeColor = Color.Red;

                    // Adiciona aviso e pinta o texto do label em vermelho
                    label.Text = baseText + "\n⚠️ Please update the game process/path!";
                    label.ForeColor = Color.Red;
                }
                else
                {
                    group.ForeColor = SystemColors.ControlText;
                    label.Text = baseText;
                    label.ForeColor = SystemColors.ControlText;
                }

                var innerLayout = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    AutoSize = true,
                    ColumnCount = 1,
                    RowCount = 1
                };

                innerLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                innerLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

                label.Margin = new Padding(0, 0, 0, 10); // espaçamento inferior

                innerLayout.Controls.Add(label, 0, 0);

                group.Click += (sender, e) => ShowGameDetails(g.Game);
                label.Click += (sender, e) => ShowGameDetails(g.Game);

                group.Controls.Add(innerLayout);
                flowLayoutPanelGames.Controls.Add(group);
            }
        }

        // Updates the status label with the current game status
        private void UpdateStatus(string gameName, string status)
        {
            string message;

            if (status == "Running")
            {
                message = $"Game running: {gameName}";
            }
            else
            {
                message = $"Game: {gameName} - {status}";
            }

            if (labelStatus.InvokeRequired)
            {
                labelStatus.Invoke(() => labelStatus.Text = message);
            }
            else
            {
                labelStatus.Text = message;
            }
        }

        private void ShowGameDetails(Game game)
        {
            var historyForm = new GameHistoryForm(game, _databaseService, _gameController);
            historyForm.ShowDialog();

            // Atualiza a tela após edição/remoção
            DisplayAllGamesTime();
        }
    }
}
