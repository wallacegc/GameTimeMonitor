using GameTimeMonitor.Controllers;
using GameTimeMonitor.Models;
using GameTimeMonitor.Services;

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

        // Initializes the ToolStrip and adds the "Add Game" button
        private void InitializeToolStrip()
        {
            ToolStrip toolStrip = new ToolStrip();
            ToolStripButton addGameButton = new ToolStripButton("Add Game");

            addGameButton.Click += AddGameButton_Click;
            toolStrip.Items.Add(addGameButton);
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

            foreach (var game in _gameController.GetGames())
            {
                var gameName = game.Name;
                var today = DateTime.Today;
                var startWeek = today.AddDays(-(int)today.DayOfWeek);
                var startMonth = new DateTime(today.Year, today.Month, 1);

                double hoursToday = _databaseService.GetGameTime(game.Name, today, DateTime.Now);
                double hoursWeek = _databaseService.GetGameTime(game.Name, startWeek, DateTime.Now);
                double hoursMonth = _databaseService.GetGameTime(game.Name, startMonth, DateTime.Now);
                double totalHours = _databaseService.GetGameTime(game.Name, DateTime.MinValue, DateTime.Now);

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
                    Text = game.Name,
                    Width = 740,
                    AutoSize = true,
                    Padding = new Padding(10),
                    AutoSizeMode = AutoSizeMode.GrowAndShrink
                };

                var label = new Label
                {
                    AutoSize = true,
                    MaximumSize = new Size(700, 0),
                    Text = $"{gameName} - Today: {FormatTime(hoursToday)} | Week: {FormatTime(hoursWeek)} | Month: {FormatTime(hoursMonth)} | Total: {FormatTime(totalHours)}"
                };

                var removeButton = new Button
                {
                    Text = "Remove",
                    AutoSize = true,
                    Margin = new Padding(10)
                };

                removeButton.Location = new Point(group.Width - removeButton.Width - 20, 30);

                removeButton.Click += (sender, args) =>
                {
                    var result = MessageBox.Show(
                        $"Are you sure you want to remove the game: {gameName}?",
                        "Confirm Removal",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    );

                    if (result == DialogResult.Yes)
                    {
                        _gameController.RemoveGame(gameName);
                        DisplayAllGamesTime();
                    }
                };

                group.Controls.Add(label);
                group.Controls.Add(removeButton);
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
