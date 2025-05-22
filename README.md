# 🎮 GameTimeMonitor - *In development*

**GameTimeMonitor** is a lightweight Windows application that tracks your gaming sessions, helping you monitor how much time you've spent playing each game. All data is stored locally using SQLite, and game information is managed via a simple JSON file.

---

## 📦 Features

- ⏱️ **Track Game Sessions**  
  Automatically logs the start and end time of your gameplay.

- 📊 **Statistics Dashboard**  
  View daily, weekly, monthly, and total playtime for each game.

- 📝 **Game Management**  
  Add, edit, and manage games by specifying their executable.

- 🗃️ **Session History**  
  See detailed stats like the longest session and the most played day.

- 🧹 **Data Cleanup**  
  - Detects and removes duplicate sessions.  
  - Deletes sessions with duration less than 1 minute.

- 💾 **Local Storage**  
  Uses `games.json` to store game info and `SQLite` for session data.

---

## 🖼️ Screenshots

~~*future*~~

---

## 🛠️ How It Works

1. Add a game and select its executable file.
2. GameTimeMonitor watches for that process running.
3. When it starts/stops, a session is saved in the local SQLite DB.
4. You can view stats, edit game info, or clean up invalid data.

---

## 🗂️ File Structure
GameTimeMonitor/
├── Views/ # Forms and UI
├── Services/ # Database and logic
├── Models/ # Game and Session models
├── Database/games.json # Stores game list and executable names
└── playtime.db # SQLite database with session logs

---

## 🔄 Roadmap

- [ ] Export statistics to CSV/Excel  
- [ ] Add system tray integration  
- [ ] Support for tagging or grouping games  
- [ ] Localization (multi-language support)  

---

## 💬 Contributing

Feel free to open issues or pull requests to suggest features or improvements.

---

## 📄 License

This project is open source and available under the [MIT License](LICENSE).

---

## 👨‍💻 Author

Developed by Wallace Gomes Correa.  
Feedback, ideas, and PRs are welcome!
