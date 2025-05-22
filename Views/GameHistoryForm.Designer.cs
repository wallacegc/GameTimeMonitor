namespace GameTimeMonitor.Views
{
    partial class GameHistoryForm
    {
        private Label lblTitle;
        private ListBox listBoxSessions;
        private Button btnEdit;
        private Button btnRemove;

        private void InitializeComponent()
        {
            lblTitle = new Label();
            listBoxSessions = new ListBox();
            btnEdit = new Button();
            btnRemove = new Button();

            SuspendLayout();

            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Point);
            lblTitle.Location = new Point(20, 20);
            lblTitle.Size = new Size(150, 32);

            listBoxSessions.FormattingEnabled = true;
            listBoxSessions.ItemHeight = 25;
            listBoxSessions.Location = new Point(20, 70);
            listBoxSessions.Size = new Size(500, 300);

            btnEdit.Text = "Edit";
            btnEdit.Location = new Point(20, 390);
            btnEdit.Size = new Size(100, 35);
            btnEdit.Click += BtnEdit_Click;

            btnRemove.Text = "Remove";
            btnRemove.Location = new Point(130, 390);
            btnRemove.Size = new Size(100, 35);
            btnRemove.Click += BtnRemove_Click;

            ClientSize = new Size(550, 450);
            Controls.Add(lblTitle);
            Controls.Add(listBoxSessions);
            Controls.Add(btnEdit);
            Controls.Add(btnRemove);
            Text = "Game Details";
            ResumeLayout(false);
            PerformLayout();
        }
    }
}