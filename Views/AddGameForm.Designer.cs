namespace GameTimeMonitor.Views
{
    partial class AddGameForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label labelGameName;
        private System.Windows.Forms.Label labelGamePath;
        private System.Windows.Forms.TextBox txtGameName;
        private System.Windows.Forms.TextBox txtGamePath;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSelectGamePath;  // Novo botão para selecionar o arquivo

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.labelGameName = new System.Windows.Forms.Label();
            this.labelGamePath = new System.Windows.Forms.Label();
            this.txtGameName = new System.Windows.Forms.TextBox();
            this.txtGamePath = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSelectGamePath = new System.Windows.Forms.Button();  // Inicialização do botão

            this.SuspendLayout();

            // 
            // labelGameName
            // 
            this.labelGameName.AutoSize = true;
            this.labelGameName.Location = new System.Drawing.Point(12, 15);
            this.labelGameName.Name = "labelGameName";
            this.labelGameName.Size = new System.Drawing.Size(60, 13);
            this.labelGameName.TabIndex = 0;
            this.labelGameName.Text = "Nome do Jogo";

            // 
            // txtGameName
            // 
            this.txtGameName.Location = new System.Drawing.Point(120, 12);
            this.txtGameName.Name = "txtGameName";
            this.txtGameName.Size = new System.Drawing.Size(200, 20);
            this.txtGameName.TabIndex = 1;

            // 
            // labelGamePath
            // 
            this.labelGamePath.AutoSize = true;
            this.labelGamePath.Location = new System.Drawing.Point(12, 45);
            this.labelGamePath.Name = "labelGamePath";
            this.labelGamePath.Size = new System.Drawing.Size(87, 13);
            this.labelGamePath.TabIndex = 2;
            this.labelGamePath.Text = "Caminho do Jogo";

            // 
            // txtGamePath
            // 
            this.txtGamePath.Location = new System.Drawing.Point(120, 42);
            this.txtGamePath.Name = "txtGamePath";
            this.txtGamePath.Size = new System.Drawing.Size(200, 20);
            this.txtGamePath.TabIndex = 3;
            this.txtGamePath.ReadOnly = true;  // Apenas leitura, já que é preenchido pelo botão

            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(120, 80);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "Salvar";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);

            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(205, 80);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancelar";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);

            // 
            // btnSelectGamePath
            // 
            this.btnSelectGamePath.Location = new System.Drawing.Point(330, 40);
            this.btnSelectGamePath.Name = "btnSelectGamePath";
            this.btnSelectGamePath.Size = new System.Drawing.Size(75, 23);
            this.btnSelectGamePath.TabIndex = 6;
            this.btnSelectGamePath.Text = "Selecionar";
            this.btnSelectGamePath.UseVisualStyleBackColor = true;
            this.btnSelectGamePath.Click += new System.EventHandler(this.BtnSelectGamePath_Click);

            // 
            // AddGameForm
            // 
            this.ClientSize = new System.Drawing.Size(434, 121);
            this.Controls.Add(this.btnSelectGamePath);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtGamePath);
            this.Controls.Add(this.labelGamePath);
            this.Controls.Add(this.txtGameName);
            this.Controls.Add(this.labelGameName);
            this.Name = "AddGameForm";
            this.Text = "Adicionar Jogo";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
