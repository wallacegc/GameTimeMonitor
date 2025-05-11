namespace GameTimeMonitor.Views
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelGames;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton addGameButton;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.labelStatus = new System.Windows.Forms.Label();
            this.flowLayoutPanelGames = new System.Windows.Forms.FlowLayoutPanel();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.addGameButton = new System.Windows.Forms.ToolStripButton();

            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(12, 40); // Ajustado para ficar abaixo do ToolStrip
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(0, 13);
            this.labelStatus.TabIndex = 0;

            // 
            // flowLayoutPanelGames
            // 
            this.flowLayoutPanelGames.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                                                                                    | System.Windows.Forms.AnchorStyles.Left)
                                                                                    | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanelGames.AutoScroll = true;
            this.flowLayoutPanelGames.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanelGames.Location = new System.Drawing.Point(12, 70); // Já estava correto
            this.flowLayoutPanelGames.Name = "flowLayoutPanelGames";
            this.flowLayoutPanelGames.Size = new System.Drawing.Size(760, 400);
            this.flowLayoutPanelGames.TabIndex = 1;
            this.flowLayoutPanelGames.WrapContents = false;

            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(784, 441);
            this.Controls.Add(this.flowLayoutPanelGames);
            this.Controls.Add(this.labelStatus);
            this.Name = "Form1";
            this.Text = "Game Time Monitor";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
