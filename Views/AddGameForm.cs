using System;
using System.Windows.Forms;
using System.IO;
using System.ComponentModel;

namespace GameTimeMonitor.Views
{
    public partial class AddGameForm : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string GameName { get; private set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string GameProcess { get; private set; }

        public AddGameForm()
        {
            InitializeComponent();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            GameName = txtGameName.Text;

            if (string.IsNullOrEmpty(GameName) || string.IsNullOrEmpty(GameProcess))
            {
                MessageBox.Show("Por favor, preencha todos os campos.");
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

        // Função para abrir o OpenFileDialog e selecionar o arquivo do jogo
        private void BtnSelectGamePath_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Executáveis (*.exe)|*.exe|Todos os arquivos (*.*)|*.*";
                openFileDialog.Title = "Selecione o Arquivo Executável do Jogo";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Extrair apenas o nome do arquivo sem a extensão
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(openFileDialog.FileName);

                    // Preenche o campo do caminho com o caminho completo, mas salva apenas o nome do arquivo
                    txtGamePath.Text = openFileDialog.FileName;
                    GameProcess = fileNameWithoutExtension; // Salva apenas o nome do arquivo sem a extensão
                }
            }
        }
    }
}
