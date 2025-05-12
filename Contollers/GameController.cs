using GameTimeMonitor.Models;
using GameTimeMonitor.Services;
using Newtonsoft.Json;

namespace GameTimeMonitor.Controllers
{
    public class GameController
    {
        private List<Game> _games;
        private readonly DatabaseService _databaseService;

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
                string json = File.ReadAllText("Database/games.json");
                _games = JsonConvert.DeserializeObject<List<Game>>(json) ?? new List<Game>();
            }
            catch
            {
                _games = new List<Game>();
                MessageBox.Show("Error loading Database/games.json");
            }
        }

        // Adds a new game to the list and saves the updated list to the JSON file
        public void AddGame(Game game)
        {
            _games.Add(game);
            File.WriteAllText("Database/games.json", JsonConvert.SerializeObject(_games, Formatting.Indented));
        }

        // Removes a game by name from the list and updates the JSON file
        public void RemoveGame(string gameName)
        {
            _games.RemoveAll(game => game.Name == gameName);
            File.WriteAllText("Database/games.json", JsonConvert.SerializeObject(_games, Formatting.Indented));
        }

        // Updates an existing game's information in the list and updates the JSON file
        public void UpdateGame(Game oldGame, Game newGame)
        {
            var index = _games.FindIndex(g => g.Name == oldGame.Name);
            if (index >= 0)
            {
                _games[index] = newGame;
            }
            File.WriteAllText("Database/games.json", JsonConvert.SerializeObject(_games, Formatting.Indented));
        }

        // Returns the list of games
        public List<Game> GetGames()
        {
            return _games;
        }
    }
}
