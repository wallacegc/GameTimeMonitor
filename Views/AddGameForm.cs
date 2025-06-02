using System.ComponentModel;
using System.IO;  // Needed for Path methods
using System.Windows.Forms;

namespace GameTimeMonitor.Views
{
    public partial class AddGameForm : Form
    {
        // Stores the game name entered by the user
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string GameName { get; private set; }

        // Stores the process name (executable name without extension)
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string GameProcess { get; private set; }

        // Holds the executable name without extension after user selects it
        private string selectedProcessName = string.Empty;

        public AddGameForm()
        {
            InitializeComponent();
        }

        // Save button click event: validates inputs and closes form with OK result
        private void BtnSave_Click(object sender, EventArgs e)
        {
            GameName = txtGameName.Text;
            GameProcess = selectedProcessName;  // Save executable name without extension

            if (string.IsNullOrEmpty(GameName) || string.IsNullOrEmpty(GameProcess))
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        // Cancel button click event: closes form with Cancel result
        private void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        // Opens a file dialog to select the game executable
        private void BtnSelectGamePath_Click(object sender, EventArgs e)
        {
            using var openFileDialog = new OpenFileDialog
            {
                Filter = "Executables (*.exe)|*.exe|All files (*.*)|*.*",
                Title = "Select the Game Executable File"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Extract executable name without extension
                selectedProcessName = Path.GetFileNameWithoutExtension(openFileDialog.FileName);

                // Display full path in the TextBox for visual reference
                txtGamePath.Text = openFileDialog.FileName;
            }
        }
    }
}
