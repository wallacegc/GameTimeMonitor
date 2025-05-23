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
        private System.Windows.Forms.Button btnSelectGamePath;  // New button to select the file

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            labelGameName = new Label();
            labelGamePath = new Label();
            txtGameName = new TextBox();
            txtGamePath = new TextBox();
            btnSave = new Button();
            btnCancel = new Button();
            btnSelectGamePath = new Button();
            SuspendLayout();
            // 
            // labelGameName
            // 
            labelGameName.AutoSize = true;
            labelGameName.Location = new Point(12, 15);
            labelGameName.Name = "labelGameName";
            labelGameName.Size = new Size(132, 25);
            labelGameName.TabIndex = 0;
            labelGameName.Text = "Game Name";
            // 
            // labelGamePath
            // 
            labelGamePath.AutoSize = true;
            labelGamePath.Location = new Point(12, 46);
            labelGamePath.Name = "labelGamePath";
            labelGamePath.Size = new Size(154, 25);
            labelGamePath.TabIndex = 2;
            labelGamePath.Text = "Game Path";
            // 
            // txtGameName
            // 
            txtGameName.Location = new Point(163, 12);
            txtGameName.Name = "txtGameName";
            txtGameName.Size = new Size(273, 31);
            txtGameName.TabIndex = 1;
            // 
            // txtGamePath
            // 
            txtGamePath.Location = new Point(163, 42);
            txtGamePath.Name = "txtGamePath";
            txtGamePath.ReadOnly = true;
            txtGamePath.Size = new Size(273, 31);
            txtGamePath.TabIndex = 3;
            // 
            // btnSave
            // 
            btnSave.Location = new Point(396, 163);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(75, 37);
            btnSave.TabIndex = 4;
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += BtnSave_Click;
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(477, 163);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(75, 37);
            btnCancel.TabIndex = 5;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += BtnCancel_Click;
            // 
            // btnSelectGamePath
            // 
            btnSelectGamePath.Location = new Point(442, 42);
            btnSelectGamePath.Name = "btnSelectGamePath";
            btnSelectGamePath.Size = new Size(110, 33);
            btnSelectGamePath.TabIndex = 6;
            btnSelectGamePath.Text = "Select";
            btnSelectGamePath.UseVisualStyleBackColor = true;
            btnSelectGamePath.Click += BtnSelectGamePath_Click;
            // 
            // AddGameForm
            // 
            ClientSize = new Size(564, 212);
            Controls.Add(btnSelectGamePath);
            Controls.Add(btnCancel);
            Controls.Add(btnSave);
            Controls.Add(txtGamePath);
            Controls.Add(labelGamePath);
            Controls.Add(txtGameName);
            Controls.Add(labelGameName);
            Name = "AddGameForm";
            Text = "Add Game";
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
