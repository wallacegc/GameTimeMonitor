using GameTimeMonitor.Models;
using GameTimeMonitor.Services;
using Newtonsoft.Json;

namespace GameTimeMonitor.Controllers
{
    public class GameController
    {
        private List<Game> _games;
        private readonly DatabaseService _databaseService;

        public GameController(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            _games = new List<Game>();
        }

        public void LoadGames()
        {
            // Carregar jogos do arquivo JSON
            try
            {
                string json = File.ReadAllText("database/games.json");
                _games = JsonConvert.DeserializeObject<List<Game>>(json) ?? new List<Game>();
            }
            catch
            {
                _games = new List<Game>();
                MessageBox.Show("Erro ao carregar database/games.json");
            }
        }

        public void AddGame(Game game)
        {
            _games.Add(game);
            // Salvar no arquivo JSON
            File.WriteAllText("database/games.json", JsonConvert.SerializeObject(_games, Formatting.Indented));
        }

        public void RemoveGame(string gameName)
        {
            _games.RemoveAll(game => game.Name == gameName);
            // Atualizar o arquivo JSON
            File.WriteAllText("database/games.json", JsonConvert.SerializeObject(_games, Formatting.Indented));
        }

        public void UpdateGame(Game oldGame, Game newGame)
        {
            var index = _games.FindIndex(g => g.Name == oldGame.Name);
            if (index >= 0)
            {
                _games[index] = newGame;
            }
            // Atualizar o arquivo JSON
            File.WriteAllText("database/games.json", JsonConvert.SerializeObject(_games, Formatting.Indented));
        }

        public List<Game> GetGames()
        {
            return _games;
        }
    }
}
