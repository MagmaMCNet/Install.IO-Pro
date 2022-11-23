namespace Install.IO_Pro
{
    public partial class InstallForm : Form
    {
        public InstallForm()
        {
            TransparencyKey = Color.Thistle;
            InitializeComponent();
        }
        private void InstallForm_Load(object sender, EventArgs e)
        {
            OnLoad();

        }

        private void NextPage_Click(object sender, EventArgs e)
        {
            NextPage_click();
        }

        private void BackPage_Click_1(object sender, EventArgs e)
        {
            BackPage_click();
        }
    }
}