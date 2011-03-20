namespace NeatTicTacToe
{
    partial class GameForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.startHereToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newGameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadNEATPlayerToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.setAIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.randomPlayerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optimalPlayerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.neatPlayerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setYourMarkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.xToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.oToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.loadHyperNEATPlayerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startHereToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(771, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // startHereToolStripMenuItem
            // 
            this.startHereToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newGameToolStripMenuItem,
            this.loadNEATPlayerToolStripMenuItem1,
            this.loadHyperNEATPlayerToolStripMenuItem,
            this.setAIToolStripMenuItem,
            this.setYourMarkToolStripMenuItem});
            this.startHereToolStripMenuItem.Name = "startHereToolStripMenuItem";
            this.startHereToolStripMenuItem.Size = new System.Drawing.Size(69, 20);
            this.startHereToolStripMenuItem.Text = "Start Here";
            // 
            // newGameToolStripMenuItem
            // 
            this.newGameToolStripMenuItem.Name = "newGameToolStripMenuItem";
            this.newGameToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.newGameToolStripMenuItem.Text = "New Game";
            this.newGameToolStripMenuItem.Click += new System.EventHandler(this.newGameToolStripMenuItem_Click);
            // 
            // loadNEATPlayerToolStripMenuItem1
            // 
            this.loadNEATPlayerToolStripMenuItem1.Name = "loadNEATPlayerToolStripMenuItem1";
            this.loadNEATPlayerToolStripMenuItem1.Size = new System.Drawing.Size(211, 22);
            this.loadNEATPlayerToolStripMenuItem1.Text = "Load NEAT Player...";
            this.loadNEATPlayerToolStripMenuItem1.Click += new System.EventHandler(this.loadNEATPlayerToolStripMenuItem1_Click);
            // 
            // setAIToolStripMenuItem
            // 
            this.setAIToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.randomPlayerToolStripMenuItem,
            this.optimalPlayerToolStripMenuItem,
            this.neatPlayerToolStripMenuItem});
            this.setAIToolStripMenuItem.Name = "setAIToolStripMenuItem";
            this.setAIToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.setAIToolStripMenuItem.Text = "Set AI";
            // 
            // randomPlayerToolStripMenuItem
            // 
            this.randomPlayerToolStripMenuItem.Checked = true;
            this.randomPlayerToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.randomPlayerToolStripMenuItem.Name = "randomPlayerToolStripMenuItem";
            this.randomPlayerToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.randomPlayerToolStripMenuItem.Text = "Random Player";
            this.randomPlayerToolStripMenuItem.Click += new System.EventHandler(this.randomPlayerToolStripMenuItem_Click);
            // 
            // optimalPlayerToolStripMenuItem
            // 
            this.optimalPlayerToolStripMenuItem.Name = "optimalPlayerToolStripMenuItem";
            this.optimalPlayerToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.optimalPlayerToolStripMenuItem.Text = "Optimal Player";
            this.optimalPlayerToolStripMenuItem.Click += new System.EventHandler(this.optimalPlayerToolStripMenuItem_Click);
            // 
            // neatPlayerToolStripMenuItem
            // 
            this.neatPlayerToolStripMenuItem.Enabled = false;
            this.neatPlayerToolStripMenuItem.Name = "neatPlayerToolStripMenuItem";
            this.neatPlayerToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.neatPlayerToolStripMenuItem.Text = "NEAT Player";
            this.neatPlayerToolStripMenuItem.Click += new System.EventHandler(this.neatPlayerToolStripMenuItem_Click);
            // 
            // setYourMarkToolStripMenuItem
            // 
            this.setYourMarkToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.xToolStripMenuItem,
            this.oToolStripMenuItem});
            this.setYourMarkToolStripMenuItem.Name = "setYourMarkToolStripMenuItem";
            this.setYourMarkToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.setYourMarkToolStripMenuItem.Text = "Set Your Mark";
            // 
            // xToolStripMenuItem
            // 
            this.xToolStripMenuItem.Checked = true;
            this.xToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.xToolStripMenuItem.Name = "xToolStripMenuItem";
            this.xToolStripMenuItem.Size = new System.Drawing.Size(93, 22);
            this.xToolStripMenuItem.Text = "X";
            this.xToolStripMenuItem.Click += new System.EventHandler(this.xToolStripMenuItem_Click);
            // 
            // oToolStripMenuItem
            // 
            this.oToolStripMenuItem.Name = "oToolStripMenuItem";
            this.oToolStripMenuItem.Size = new System.Drawing.Size(93, 22);
            this.oToolStripMenuItem.Text = "O";
            this.oToolStripMenuItem.Click += new System.EventHandler(this.oToolStripMenuItem_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "tictactoe_champion.xml";
            // 
            // loadHyperNEATPlayerToolStripMenuItem
            // 
            this.loadHyperNEATPlayerToolStripMenuItem.Name = "loadHyperNEATPlayerToolStripMenuItem";
            this.loadHyperNEATPlayerToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.loadHyperNEATPlayerToolStripMenuItem.Text = "Load HyperNEAT Player...";
            this.loadHyperNEATPlayerToolStripMenuItem.Click += new System.EventHandler(this.loadHyperNEATPlayerToolStripMenuItem_Click);
            // 
            // GameForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(771, 537);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "GameForm";
            this.Text = "NEAT Tic Tac Toe - v1.0 by Wesley Tansey";
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.GameForm_MouseClick);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem startHereToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setAIToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem randomPlayerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optimalPlayerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem neatPlayerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadNEATPlayerToolStripMenuItem1;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.ToolStripMenuItem setYourMarkToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem xToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem oToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newGameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadHyperNEATPlayerToolStripMenuItem;
    }
}

