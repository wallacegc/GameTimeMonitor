using GameTimeMonitor.Controllers;
using GameTimeMonitor.Services;
using GameTimeMonitor.Models;

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

            // Inicializando a barra de ferramentas
            InitializeToolStrip();

            this.Load += Form1_Load; // Conecta o método ao evento Load
            _databaseService.InitializeDatabase();
            _gameController.LoadGames();
            _gameMonitoringService.StartMonitoring(_gameController.GetGames());
            _gameMonitoringService.GameStatusChanged += UpdateStatus;
        }

        // Método que inicializa o ToolStrip
        private void InitializeToolStrip()
        {
            // Criando o ToolStrip
            ToolStrip toolStrip = new ToolStrip();
            ToolStripButton addGameButton = new ToolStripButton("Adicionar Jogo");

            // Associando o evento de click ao botão
            addGameButton.Click += AddGameButton_Click;

            // Adicionando o botão ao ToolStrip
            toolStrip.Items.Add(addGameButton);

            // Adicionando o ToolStrip ao Form
            this.Controls.Add(toolStrip);
        }

        // Evento de clique para o botão "Adicionar Jogo"
        private void AddGameButton_Click(object sender, EventArgs e)
        {
            using (var addGameForm = new AddGameForm())
            {
                var result = addGameForm.ShowDialog(); // Exibe o formulário modal

                if (result == DialogResult.OK)
                {
                    string gameName = addGameForm.GameName;
                    string gameProcess = addGameForm.GameProcess;

                    // Cria o novo jogo
                    var newGame = new Game
                    {
                        Name = gameName,
                        Process = gameProcess
                    };

                    // Adiciona o jogo ao GameController e salva no arquivo JSON
                    _gameController.AddGame(newGame);

                    // Atualiza a lista de jogos na interface
                    DisplayAllGamesTime();

                    MessageBox.Show("Jogo adicionado com sucesso!");
                }
            }
        }


        // O método Form1_Load que será chamado quando o formulário for carregado
        private void Form1_Load(object sender, EventArgs e)
        {
            // Carregar jogos e exibir na interface
            _gameController.LoadGames();
            DisplayAllGamesTime(); // Certifique-se de que esta função está sendo chamada para exibir os jogos
        }

        // Função para exibir os jogos no FlowLayoutPanel
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

                // Função para formatar o tempo
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
                    Text = $"{gameName} - Hoje: {FormatTime(hoursToday)} | Semana: {FormatTime(hoursWeek)} | Mês: {FormatTime(hoursMonth)} | Total: {FormatTime(totalHours)}"
                };

                group.Controls.Add(label);
                flowLayoutPanelGames.Controls.Add(group);
            }
        }

        private void UpdateStatus(string gameName, string status)
        {
            string message = $"Jogo: {gameName} - {status}";

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