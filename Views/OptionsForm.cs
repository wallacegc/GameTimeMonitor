using Microsoft.Win32;
using System;
using System.Windows.Forms;

namespace GameTimeMonitor.Views
{
    public partial class OptionsForm : Form
    {
        private const string StartupRegistryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private const string AppName = "GameTimeMonitor";

        public OptionsForm()
        {
            InitializeComponent();
            LoadStartupSetting(); // Carrega o estado ao abrir a janela
        }

        private void ButtonSave_Click(object sender, EventArgs e)
        {
            
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void LoadStartupSetting()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(StartupRegistryKey, writable: false))
            {
                if (key != null)
                {
                    var value = key.GetValue(AppName) as string;
                    checkBoxStartWithWindows.Checked = !string.IsNullOrEmpty(value);
                }
                else
                {
                    checkBoxStartWithWindows.Checked = false;
                }
            }
        }
    }
}
