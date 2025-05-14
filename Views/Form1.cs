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
                    MaximumSize = new Size(700, 0),
                    Text = $"{g.Game.Name} - Today: {FormatTime(g.TimeToday)} | Week: {FormatTime(g.TimeWeek)} | Month: {FormatTime(g.TimeMonth)} | Total: {FormatTime(g.TimeTotal)}"
                };

                var removeButton = new Button
                {
                    Text = "Remove",
                    AutoSize = true,
                    Margin = new Padding(10)
                };

                removeButton.Click += (sender, args) =>
                {
                    var result = MessageBox.Show(
                        $"Are you sure you want to remove the game: {g.Game.Name}?",
                        "Confirm Removal",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    );

                    if (result == DialogResult.Yes)
                    {
                        _gameController.RemoveGame(g.Game.Name);
                        DisplayAllGamesTime();
                    }
                };

                var innerLayout = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    AutoSize = true,
                    ColumnCount = 1,
                    RowCount = 2
                };

                innerLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                innerLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

                label.Margin = new Padding(0, 0, 0, 10); // espaçamento inferior
                removeButton.Anchor = AnchorStyles.Right; // ou AnchorStyles.None para centralizar

                innerLayout.Controls.Add(label, 0, 0);
                innerLayout.Controls.Add(removeButton, 0, 1);

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
    }
}
