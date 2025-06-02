using GameTimeMonitor.Models;
using GameTimeMonitor.Services;
using Newtonsoft.Json;
using System.IO;
using System.Windows.Forms;

namespace GameTimeMonitor.Controllers
{
    public class GameController
    {
        private List<Game> _games;
        private readonly DatabaseService _databaseService;
        private readonly string _gamesFilePath = "Database/games.json";

        // Constructor to initialize the GameController with the database service
        public GameController(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            _games = new List<Game>();
        }

        // Loads games from the JSON file into the _games list
        public void LoadGames()
        {
            try
            {
                if (File.Exists(_gamesFilePath))
                {
                    string json = File.ReadAllText(_gamesFilePath);
                    _games = JsonConvert.DeserializeObject<List<Game>>(json) ?? new List<Game>();
                }
                else
                {
                    File.WriteAllText(_gamesFilePath, JsonConvert.SerializeObject(new List<Game>(), Formatting.Indented));
                    _games = new List<Game>();
                }
            }
            catch (Exception ex)
            {
                _games = new List<Game>();
                MessageBox.Show($"Error loading games.json: {ex.Message}", "Load Error");
            }
        }

        // Adds a new game and saves the updated list to the JSON file
        public void AddGame(Game game)
        {
            _games.Add(game);
            SaveGamesToFile();
        }

        // Removes a game by its name and updates the JSON file
        public void RemoveGame(string gameName)
        {
            _games.RemoveAll(game => game.Name == gameName);
            SaveGamesToFile();
        }

        // Updates a game and also reflects the change in the database
        public void UpdateGame(Game oldGame, Game newGame)
        {
            var index = _games.FindIndex(g => g.Name == oldGame.Name);
            if (index >= 0)
            {
                _games[index] = newGame;
                _databaseService.UpdateGameNameInDatabase(oldGame.Name, newGame.Name);
                SaveGamesToFile();
            }
        }

        // Saves the _games list to the JSON file
        private void SaveGamesToFile()
        {
            try
            {
                File.WriteAllText(_gamesFilePath, JsonConvert.SerializeObject(_games, Formatting.Indented));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving games.json: {ex.Message}", "Save Error");
            }
        }

        // Returns the list of games
        public List<Game> GetGames()
        {
            return _games;
        }
    }
}
