using System.ComponentModel;

namespace GameTimeMonitor.Views
{
    public partial class AddGameForm : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string GameName { get; private set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string GameProcess { get; private set; }

        // Constructor to initialize the AddGameForm
        public AddGameForm()
        {
            InitializeComponent();
        }

        // Event handler for the save button click
        private void BtnSave_Click(object sender, EventArgs e)
        {
            GameName = txtGameName.Text;

            if (string.IsNullOrEmpty(GameName) || string.IsNullOrEmpty(GameProcess))
            {
                MessageBox.Show("Please fill in all fields.");
            }
            else
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        // Event handler for the cancel button click
        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        // Function to open the OpenFileDialog and select the game's executable file
        private void BtnSelectGamePath_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Executables (*.exe)|*.exe|All files (*.*)|*.*";
                openFileDialog.Title = "Select the Game Executable File";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Extract the file name without the extension
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(openFileDialog.FileName);

                    // Fill in the path field with the full path, but save only the file name
                    txtGamePath.Text = openFileDialog.FileName;
                    GameProcess = fileNameWithoutExtension; // Save only the file name without the extension
                }
            }
        }
    }
}
