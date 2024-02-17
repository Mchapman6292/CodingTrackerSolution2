namespace CodingTracker.View
{
    partial class ViewSessionsPage
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
            components = new System.ComponentModel.Container();
            viewSessionsButton = new Button();
            updateSessionButton = new Button();
            filterSessionsMenu = new ContextMenuStrip(components);
            cMenuStripDay = new ToolStripMenuItem();
            cMenuStripWeek = new ToolStripMenuItem();
            cMenuStripYear = new ToolStripMenuItem();
            toolStrip1 = new ToolStrip();
            daysAsc = new ToolStripMenuItem();
            daysDesc = new ToolStripMenuItem();
            weeksAsc = new ToolStripMenuItem();
            weeksDesc = new ToolStripMenuItem();
            yearsAsc = new ToolStripMenuItem();
            yearsDesc = new ToolStripMenuItem();
            filterSessionsMenu.SuspendLayout();
            SuspendLayout();
            // 
            // viewSessionsButton
            // 
            viewSessionsButton.Location = new Point(758, 193);
            viewSessionsButton.Name = "viewSessionsButton";
            viewSessionsButton.Size = new Size(275, 66);
            viewSessionsButton.TabIndex = 0;
            viewSessionsButton.Text = "View sessions";
            viewSessionsButton.UseVisualStyleBackColor = true;
            viewSessionsButton.Click += viewSessionsButton_Click;
            viewSessionsButton.DragDrop += viewSessionsButton_DragDrop;
            // 
            // updateSessionButton
            // 
            updateSessionButton.Location = new Point(752, 329);
            updateSessionButton.Name = "updateSessionButton";
            updateSessionButton.Size = new Size(281, 81);
            updateSessionButton.TabIndex = 1;
            updateSessionButton.Text = "Update Session";
            updateSessionButton.UseVisualStyleBackColor = true;
            updateSessionButton.Click += updateSessionButton_Click;
            // 
            // filterSessionsMenu
            // 
            filterSessionsMenu.Items.AddRange(new ToolStripItem[] { cMenuStripDay, cMenuStripWeek, cMenuStripYear });
            filterSessionsMenu.Name = "contextMenuStrip1";
            filterSessionsMenu.Size = new Size(181, 92);
            filterSessionsMenu.Opening += contextMenuStrip1_Opening;
            // 
            // cMenuStripDay
            // 
            cMenuStripDay.CheckOnClick = true;
            cMenuStripDay.DropDownItems.AddRange(new ToolStripItem[] { daysAsc, daysDesc });
            cMenuStripDay.Name = "cMenuStripDay";
            cMenuStripDay.Size = new Size(180, 22);
            cMenuStripDay.Text = "Previous day";
            cMenuStripDay.Click += Previous24Hours_Click;
            // 
            // cMenuStripWeek
            // 
            cMenuStripWeek.DropDownItems.AddRange(new ToolStripItem[] { weeksAsc, weeksDesc });
            cMenuStripWeek.Name = "cMenuStripWeek";
            cMenuStripWeek.Size = new Size(180, 22);
            cMenuStripWeek.Text = "Previous week";
            cMenuStripWeek.Click += cMenuStripWeek_Click;
            // 
            // cMenuStripYear
            // 
            cMenuStripYear.DropDownItems.AddRange(new ToolStripItem[] { yearsAsc, yearsDesc });
            cMenuStripYear.Name = "cMenuStripYear";
            cMenuStripYear.Size = new Size(180, 22);
            cMenuStripYear.Text = "Previous year";
            // 
            // toolStrip1
            // 
            toolStrip1.Location = new Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(2481, 25);
            toolStrip1.TabIndex = 2;
            toolStrip1.Text = "toolStrip1";
            // 
            // daysAsc
            // 
            daysAsc.Name = "daysAsc";
            daysAsc.Size = new Size(180, 22);
            daysAsc.Text = "Ascending";
            // 
            // daysDesc
            // 
            daysDesc.Name = "daysDesc";
            daysDesc.Size = new Size(180, 22);
            daysDesc.Text = "Descending";
            // 
            // weeksAsc
            // 
            weeksAsc.Name = "weeksAsc";
            weeksAsc.Size = new Size(136, 22);
            weeksAsc.Text = "Ascending";
            // 
            // weeksDesc
            // 
            weeksDesc.Name = "weeksDesc";
            weeksDesc.Size = new Size(136, 22);
            weeksDesc.Text = "Descending";
            // 
            // yearsAsc
            // 
            yearsAsc.Name = "yearsAsc";
            yearsAsc.Size = new Size(180, 22);
            yearsAsc.Text = "Ascending";
            // 
            // yearsDesc
            // 
            yearsDesc.Name = "yearsDesc";
            yearsDesc.Size = new Size(180, 22);
            yearsDesc.Text = "toolStripMenuItem1";
            // 
            // ViewSessionsPage
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(2481, 713);
            Controls.Add(toolStrip1);
            Controls.Add(updateSessionButton);
            Controls.Add(viewSessionsButton);
            Name = "ViewSessionsPage";
            Text = "ViewSessionsPage";
            filterSessionsMenu.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button viewSessionsButton;
        private Button updateSessionButton;
        private ContextMenuStrip filterSessionsMenu;
        private ToolStripMenuItem cMenuStripDay;
        private ToolStripMenuItem cMenuStripWeek;
        private ToolStripMenuItem cMenuStripYear;
        private ToolStrip toolStrip1;
        private ToolStripMenuItem daysAsc;
        private ToolStripMenuItem daysDesc;
        private ToolStripMenuItem weeksAsc;
        private ToolStripMenuItem weeksDesc;
        private ToolStripMenuItem yearsAsc;
        private ToolStripMenuItem yearsDesc;
    }
}