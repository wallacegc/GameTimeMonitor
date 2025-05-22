using GameTimeMonitor.Models;
using GameTimeMonitor.Services;
using Newtonsoft.Json;
using System.IO;

namespace GameTimeMonitor.Controllers
{
    public class GameController
    {
        private List<Game> _games;
        private readonly DatabaseService _databaseService;
        private readonly string _gamesFilePath = "Database/games.json";

        // Constructor to initialize GameController with DatabaseService
        public GameController(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            _games = new List<Game>();
        }

        // Loads the games from the JSON file into the _games list
        public void LoadGames()
        {
            try
            {
                if (File.Exists(_gamesFilePath))
                {
                    // Read the existing file
                    string json = File.ReadAllText(_gamesFilePath);
                    _games = JsonConvert.DeserializeObject<List<Game>>(json) ?? new List<Game>();
                }
                else
                {
                    // If the file does not exist, create a new file with an empty list
                    File.WriteAllText(_gamesFilePath, JsonConvert.SerializeObject(new List<Game>(), Formatting.Indented));
                    _games = new List<Game>(); // Initialize as an empty list
                }
            }
            catch (Exception ex)
            {
                _games = new List<Game>();
                MessageBox.Show($"Error loading Database/games.json: {ex.Message}");
            }
        }

        // Adds a new game to the list and saves the updated list to the JSON file
        public void AddGame(Game game)
        {
            _games.Add(game);
            SaveGamesToFile();
        }

        // Removes a game by name from the list and updates the JSON file
        public void RemoveGame(string gameName)
        {
            _games.RemoveAll(game => game.Name == gameName);
            SaveGamesToFile();
        }

        // Updates an existing game's information in the list and updates the JSON file
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

        // Helper method to save games to the JSON file
        private void SaveGamesToFile()
        {
            try
            {
                File.WriteAllText(_gamesFilePath, JsonConvert.SerializeObject(_games, Formatting.Indented));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving Database/games.json: {ex.Message}");
            }
        }

        // Returns the list of games
        public List<Game> GetGames()
        {
            return _games;
        }
    }
}
