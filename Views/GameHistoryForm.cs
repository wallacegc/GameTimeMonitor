using GameTimeMonitor.Controllers;
using GameTimeMonitor.Models;
using GameTimeMonitor.Services;
using System.Linq;

namespace GameTimeMonitor.Views
{
    public partial class GameHistoryForm : Form
    {
        private readonly Game _game;
        private readonly DatabaseService _databaseService;
        private readonly GameController _gameController;

        // Initialize form and set date filters based on first session or default
        public GameHistoryForm(Game game, DatabaseService databaseService, GameController gameController)
        {
            InitializeComponent();

            _game = game;
            _databaseService = databaseService;
            _gameController = gameController;

            Text = $"Details: {_game.Name}";
            lblTitle.Text = _game.Name;

            var sessions = _databaseService.GetSessionsForGame(_game.Name);

            if (sessions.Any())
            {
                var firstSessionDate = sessions.Min(s => s.StartTime.Date);
                dtpStartDate.Value = firstSessionDate;
            }
            else
            {
                dtpStartDate.Value = DateTime.Now.AddMonths(-1);
            }

            dtpEndDate.Value = DateTime.Now;

            LoadHistory(dtpStartDate.Value, dtpEndDate.Value);
        }

        // Load game sessions filtered by date range and update UI labels
        private void LoadHistory(DateTime? startDate = null, DateTime? endDate = null)
        {
            var sessions = _databaseService.GetSessionsForGame(_game.Name);

            if (startDate.HasValue)
                sessions = sessions.Where(s => s.StartTime.Date >= startDate.Value.Date).ToList();

            if (endDate.HasValue)
                sessions = sessions.Where(s => s.EndTime.Date <= endDate.Value.Date).ToList();

            listBoxSessions.Items.Clear();

            foreach (var session in sessions)
            {
                listBoxSessions.Items.Add($"{session.StartTime:g} - {session.EndTime:g} ({session.DurationMinutes} min)");
            }

            int totalMinutes = sessions.Sum(s => s.DurationMinutes);

            var groupedByDay = sessions
                .GroupBy(s => s.StartTime.Date)
                .Select(g => new { Day = g.Key, TotalMinutes = g.Sum(s => s.DurationMinutes) })
                .ToList();

            var mostPlayedDay = groupedByDay.OrderByDescending(g => g.TotalMinutes).FirstOrDefault();

            var longestSession = sessions.OrderByDescending(s => s.DurationMinutes).FirstOrDefault();

            lblTotalPlayTime.Text = $"Total playtime: {FormatMinutes(totalMinutes)}";
            lblMostPlayedDay.Text = mostPlayedDay != null
                ? $"Most played day: {mostPlayedDay.Day:dd/MM/yyyy} - {FormatMinutes(mostPlayedDay.TotalMinutes)}"
                : "Most played day: None";

            lblLongestSession.Text = longestSession != null
                ? $"Longest session: {longestSession.StartTime:dd/MM/yyyy} - {FormatMinutes(longestSession.DurationMinutes)}"
                : "Longest session: None";
        }

        // Handle date picker changes and reload filtered sessions
        private void FilterDates_ValueChanged(object sender, EventArgs e)
        {
            if (dtpEndDate.Value.Date < dtpStartDate.Value.Date)
            {
                dtpEndDate.Value = dtpStartDate.Value;
            }

            LoadHistory(dtpStartDate.Value, dtpEndDate.Value);
        }

        // Open edit form and update game data on confirmation
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

        // Confirm and remove game with all related data
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

        // Format minutes as "Xh Ymin" or "Ymin"
        private string FormatMinutes(int totalMinutes)
        {
            int hours = totalMinutes / 60;
            int minutes = totalMinutes % 60;
            if (hours > 0)
                return $"{hours}h {minutes}min";
            else
                return $"{minutes}min";
        }
    }
}
