using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodingTracker.View
{
    public partial class ViewSessionsPage : Form
    {
        public ViewSessionsPage()
        {
            InitializeComponent();

        }

        private void viewSessionsButton_Click(object sender, EventArgs e)
        {
            Point location = new Point(viewSessionsButton.Left, viewSessionsButton.Bottom);
            filterSessionsMenu.Show(viewSessionsButton, location);
        }

        private void updateSessionButton_Click(object sender, EventArgs e)
        {

        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void Previous24Hours_Click(object sender, EventArgs e)
        {

        }

        private void viewSessionsButton_DragDrop(object sender, DragEventArgs e)
        {

        }

        private void cMenuStripWeek_Click(object sender, EventArgs e)
        {

        }
    }
}
