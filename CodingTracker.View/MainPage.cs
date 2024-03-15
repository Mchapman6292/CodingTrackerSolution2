using CodingTracker.View.IFormControllers;

namespace CodingTrackerSolution
{
    public partial class MainPage : Form
    {
        private readonly IFormController _formController;
        public MainPage(IFormController formController)
        {
            _formController = formController;
            InitializeComponent();
        }

        private void StartCodingSessionPage_Click(object sender, EventArgs e)
        {
            _formController.ShowCodingSessionPage();
        }
        private void EditSession_Click(object sender, EventArgs e)
        {
            _formController.ShowEditSessionPage();
        }

        private void ViewSession_Click(object sender, EventArgs e)
        {
            _formController.ShowViewSessionPage();
        }

        private void MainPageSettingsButton_Click(object sender, EventArgs e)
        {
            _formController.ShowSettingsPage();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2GradientPanel2_Paint(object sender, PaintEventArgs e)
        {

        }


        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2HtmlLabel15_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel26_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel23_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel22_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel2_Click(object sender, EventArgs e)
        {

        }

        private void guna2GradientPanel6_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2GradientPanel8_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2HtmlLabel9_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel10_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel11_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel13_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel16_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel17_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel25_Click(object sender, EventArgs e)
        {

        }

        private void guna2HtmlLabel28_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {

        }

        private void MainPageProgressLabel_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
