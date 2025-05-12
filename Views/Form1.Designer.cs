namespace GameTimeMonitor.Views
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelGames;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton addGameButton;

        /// <summary>
        /// Releases the resources used when the form is disposed.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        /// <summary>
        /// Initializes the form components, including controls and visual properties.
        /// </summary>
        private void InitializeComponent()
        {
            labelStatus = new Label();
            flowLayoutPanelGames = new FlowLayoutPanel();
            addGameButton = new ToolStripButton();
            SuspendLayout();

            // Setting up labelStatus
            labelStatus.AutoSize = true;
            labelStatus.Location = new Point(12, 40);
            labelStatus.Name = "labelStatus";
            labelStatus.Size = new Size(0, 25);
            labelStatus.TabIndex = 0;

            // Setting up flowLayoutPanelGames
            flowLayoutPanelGames.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            flowLayoutPanelGames.AutoScroll = true;
            flowLayoutPanelGames.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanelGames.Location = new Point(12, 70);
            flowLayoutPanelGames.Name = "flowLayoutPanelGames";
            flowLayoutPanelGames.Size = new Size(760, 359);
            flowLayoutPanelGames.TabIndex = 1;
            flowLayoutPanelGames.WrapContents = false;

            // Setting up addGameButton
            addGameButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            addGameButton.Name = "addGameButton";
            addGameButton.Size = new Size(23, 23);
            addGameButton.Text = "Add Game";

            // Form setup
            ClientSize = new Size(784, 441);
            Controls.Add(flowLayoutPanelGames);
            Controls.Add(labelStatus);
            Name = "Form1";
            Text = "Game Time Monitor";
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
