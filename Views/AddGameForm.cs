using System.ComponentModel;

namespace GameTimeMonitor.Views
{
    public partial class AddGameForm : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string GameName { get; private set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string GameProcess { get; private set; }

        // Guarda o nome do processo (executável) sem extensão
        private string selectedProcessName = string.Empty;

        public AddGameForm()
        {
            InitializeComponent();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            GameName = txtGameName.Text;
            GameProcess = selectedProcessName;  // salva somente o nome do executável sem extensão

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

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void BtnSelectGamePath_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Executables (*.exe)|*.exe|All files (*.*)|*.*";
                openFileDialog.Title = "Select the Game Executable File";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Nome do executável sem extensão
                    selectedProcessName = Path.GetFileNameWithoutExtension(openFileDialog.FileName);

                    // Mostra o caminho completo no TextBox, só pra referência visual
                    txtGamePath.Text = openFileDialog.FileName;
                }
            }
        }
    }
}
