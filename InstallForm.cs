using System.Diagnostics;
using System.Text.Json;

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

        private void button1_Click(object sender, EventArgs e)
        {
            CommonUtils.ProjectConfig c = new CommonUtils.ProjectConfig();
            c.Menus.Add(new CommonUtils.MenuConfig());
            c.Installables.Add(new CommonUtils.Installable());
            c.Installables[0].CheckBox.Position = new Point(100, 200);
            c.Menus[0].TextObjects.Add(new CommonUtils.TextObject());
            c.Menus[0].TextObjects[0].Text = "Test Title";
            c.Menus[0].TextObjects[0].Font.Size = 30;
            c.Menus[0].TextObjects[0].Position = new Point(300, 0);
            c.Menus[0].PanelObjects.Add(new CommonUtils.PanelObject());
            File.WriteAllText("config.json",
                JsonSerializer.Serialize(c, jsonSerializerOptions));
            ProcessStartInfo psi = new("config.json");
            psi.UseShellExecute = true;
            Process.Start(psi);
        }
    }
}