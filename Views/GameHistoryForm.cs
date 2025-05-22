using GameTimeMonitor.Controllers;
using GameTimeMonitor.Models;
using GameTimeMonitor.Services;
using System.Diagnostics;

namespace GameTimeMonitor.Views
{
    public partial class GameHistoryForm : Form
    {
        private readonly Game _game;
        private readonly DatabaseService _databaseService;
        private readonly GameController _gameController;

        public GameHistoryForm(Game game, DatabaseService databaseService, GameController gameController)
        {
            InitializeComponent();
            _game = game;
            _databaseService = databaseService;
            _gameController = gameController;

            Text = $"Details: {_game.Name}";
            lblTitle.Text = _game.Name;
            LoadHistory();
        }

        private void LoadHistory()
        {
            // Fixed missing semicolon
            var sessions = _databaseService.GetSessionsForGame(_game.Name);

            listBoxSessions.Items.Clear();
            foreach (var session in sessions)
            {
                listBoxSessions.Items.Add($"{session.StartTime:g} - {session.EndTime:g} ({session.DurationMinutes} min)");
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            var form = new AddGameForm();
            form.Controls["txtGameName"].Text = _game.Name;
            form.Controls["txtGamePath"].Text = _game.Process + ".exe";

            if (form.ShowDialog() == DialogResult.OK)
            {
                var updatedGame = new Game { Name = form.GameName, Process = form.GameProcess };
                _gameController.UpdateGame(_game, updatedGame);
                MessageBox.Show("Game updated successfully.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
        }

        private void BtnRemove_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show($"Remove game '{_game.Name}' and all data?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm == DialogResult.Yes)
            {
                _gameController.RemoveGame(_game.Name);
                MessageBox.Show("Game removed.", "Removed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
        }
    }
}
