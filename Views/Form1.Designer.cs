namespace GameTimeMonitor.Views
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelGames;
        private System.Windows.Forms.ToolStripButton addGameButton;
        private System.Windows.Forms.Label labelFooter;  // Label para rodapé

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            labelStatus = new Label();
            flowLayoutPanelGames = new FlowLayoutPanel();
            addGameButton = new ToolStripButton();
            labelFooter = new Label();
            SuspendLayout();

            // labelStatus
            labelStatus.AutoSize = true;
            labelStatus.Location = new Point(12, 40);
            labelStatus.Name = "labelStatus";
            labelStatus.Size = new Size(0, 25);
            labelStatus.TabIndex = 0;

            // flowLayoutPanelGames
            flowLayoutPanelGames.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            flowLayoutPanelGames.AutoScroll = true;
            flowLayoutPanelGames.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanelGames.Location = new Point(12, 70);
            flowLayoutPanelGames.Name = "flowLayoutPanelGames";
            flowLayoutPanelGames.Size = new Size(760, 359);
            flowLayoutPanelGames.TabIndex = 1;
            flowLayoutPanelGames.WrapContents = false;

            // addGameButton
            addGameButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            addGameButton.Name = "addGameButton";
            addGameButton.Size = new Size(23, 23);
            addGameButton.Text = "Add Game";

            // labelFooter
            labelFooter.AutoSize = false;
            labelFooter.Dock = DockStyle.Bottom;
            labelFooter.Height = 25;
            labelFooter.TextAlign = ContentAlignment.MiddleCenter;
            labelFooter.Text = "Wallace Gomes Correa - Version 0.0.1 Alpha";

            // Deixa o texto meio opaco (50% de transparência)
            labelFooter.ForeColor = Color.FromArgb(128, Color.Black);

            labelFooter.Font = new Font("Segoe UI", 9F, FontStyle.Italic);

            // Form1
            ClientSize = new Size(784, 441);
            Controls.Add(labelFooter);
            Controls.Add(flowLayoutPanelGames);
            Controls.Add(labelStatus);
            Name = "Form1";
            Text = "Game Time Monitor";
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
