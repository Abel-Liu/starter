namespace Starter
{
    partial class StartButton
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StartButton));
            this.button1 = new System.Windows.Forms.Button();
            this.StartButtonMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.退出ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.重置位置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.StartButtonMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.AllowDrop = true;
            this.button1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.button1.ContextMenuStrip = this.StartButtonMenu;
            this.button1.Cursor = System.Windows.Forms.Cursors.Default;
            this.button1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold);
            this.button1.ForeColor = System.Drawing.Color.SkyBlue;
            this.button1.Image = ((System.Drawing.Image)(resources.GetObject("button1.Image")));
            this.button1.Location = new System.Drawing.Point(0, 0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(130, 131);
            this.button1.TabIndex = 0;
            this.button1.Text = "ES";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            this.button1.DragOver += new System.Windows.Forms.DragEventHandler(this.button1_DragOver);
            // 
            // StartButtonMenu
            // 
            this.StartButtonMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.重置位置ToolStripMenuItem,
            this.退出ToolStripMenuItem});
            this.StartButtonMenu.Name = "StartButtonMenu";
            this.StartButtonMenu.Size = new System.Drawing.Size(153, 70);
            // 
            // 退出ToolStripMenuItem
            // 
            this.退出ToolStripMenuItem.Name = "退出ToolStripMenuItem";
            this.退出ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.退出ToolStripMenuItem.Text = "退出";
            this.退出ToolStripMenuItem.Click += new System.EventHandler(this.退出ToolStripMenuItem_Click);
            // 
            // 重置位置ToolStripMenuItem
            // 
            this.重置位置ToolStripMenuItem.Name = "重置位置ToolStripMenuItem";
            this.重置位置ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.重置位置ToolStripMenuItem.Text = "重置位置";
            this.重置位置ToolStripMenuItem.Click += new System.EventHandler(this.重置位置ToolStripMenuItem_Click);
            // 
            // StartButton
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(130, 131);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "StartButton";
            this.ShowInTaskbar = false;
            this.Text = "StartButton";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.StartButton_FormClosing);
            this.Load += new System.EventHandler(this.StartButton_Load);
            this.StartButtonMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ContextMenuStrip StartButtonMenu;
        private System.Windows.Forms.ToolStripMenuItem 退出ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 重置位置ToolStripMenuItem;
    }
}