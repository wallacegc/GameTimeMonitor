namespace GameTimeMonitor.Views
{
    partial class GameHistoryForm
    {
        private Label lblTitle;
        private ListBox listBoxSessions;
        private Button btnEdit;
        private Button btnRemove;
        private DateTimePicker dtpStartDate;
        private DateTimePicker dtpEndDate;
        private Label lblTotalPlayTime;
        private Label lblMostPlayedDay;
        private Label lblLongestSession;

        private void InitializeComponent()
        {
            lblTitle = new Label();
            listBoxSessions = new ListBox();
            btnEdit = new Button();
            btnRemove = new Button();
            dtpStartDate = new DateTimePicker();
            dtpEndDate = new DateTimePicker();
            lblTotalPlayTime = new Label();
            lblMostPlayedDay = new Label();
            lblLongestSession = new Label();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblTitle.Location = new Point(20, 20);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(0, 38);
            lblTitle.TabIndex = 0;
            // 
            // listBoxSessions
            // 
            listBoxSessions.FormattingEnabled = true;
            listBoxSessions.Location = new Point(20, 70);
            listBoxSessions.Name = "listBoxSessions";
            listBoxSessions.Size = new Size(500, 279);
            listBoxSessions.TabIndex = 1;
            // 
            // btnEdit
            // 
            btnEdit.Location = new Point(313, 544);
            btnEdit.Name = "btnEdit";
            btnEdit.Size = new Size(100, 35);
            btnEdit.TabIndex = 2;
            btnEdit.Text = "Edit";
            btnEdit.Click += BtnEdit_Click;
            // 
            // btnRemove
            // 
            btnRemove.ForeColor = SystemColors.ControlText;
            btnRemove.Location = new Point(430, 544);
            btnRemove.Name = "btnRemove";
            btnRemove.Size = new Size(100, 35);
            btnRemove.TabIndex = 3;
            btnRemove.Text = "Remove";
            btnRemove.Click += BtnRemove_Click;
            // 
            // dtpStartDate
            // 
            dtpStartDate.Format = DateTimePickerFormat.Short;
            dtpStartDate.Location = new Point(20, 370);
            dtpStartDate.Name = "dtpStartDate";
            dtpStartDate.Size = new Size(232, 31);
            dtpStartDate.TabIndex = 4;
            dtpStartDate.ValueChanged += FilterDates_ValueChanged;
            // 
            // dtpEndDate
            // 
            dtpEndDate.Format = DateTimePickerFormat.Short;
            dtpEndDate.Location = new Point(288, 370);
            dtpEndDate.Name = "dtpEndDate";
            dtpEndDate.Size = new Size(232, 31);
            dtpEndDate.TabIndex = 5;
            dtpEndDate.ValueChanged += FilterDates_ValueChanged;
            // 
            // lblTotalPlayTime
            // 
            lblTotalPlayTime.BorderStyle = BorderStyle.FixedSingle;
            lblTotalPlayTime.Location = new Point(20, 417);
            lblTotalPlayTime.Name = "lblTotalPlayTime";
            lblTotalPlayTime.Size = new Size(510, 25);
            lblTotalPlayTime.TabIndex = 6;
            // 
            // lblMostPlayedDay
            // 
            lblMostPlayedDay.BorderStyle = BorderStyle.FixedSingle;
            lblMostPlayedDay.Location = new Point(20, 458);
            lblMostPlayedDay.Name = "lblMostPlayedDay";
            lblMostPlayedDay.Size = new Size(510, 25);
            lblMostPlayedDay.TabIndex = 7;
            // 
            // lblLongestSession
            // 
            lblLongestSession.BorderStyle = BorderStyle.FixedSingle;
            lblLongestSession.Location = new Point(20, 494);
            lblLongestSession.Name = "lblLongestSession";
            lblLongestSession.Size = new Size(510, 25);
            lblLongestSession.TabIndex = 8;
            // 
            // GameHistoryForm
            // 
            ClientSize = new Size(544, 600);
            Controls.Add(lblTitle);
            Controls.Add(listBoxSessions);
            Controls.Add(dtpStartDate);
            Controls.Add(dtpEndDate);
            Controls.Add(btnEdit);
            Controls.Add(btnRemove);
            Controls.Add(lblTotalPlayTime);
            Controls.Add(lblMostPlayedDay);
            Controls.Add(lblLongestSession);
            Name = "GameHistoryForm";
            Text = "Game Details";
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
