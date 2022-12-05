using MagmaMc.JEF;
using System.Diagnostics;
using System.Text.Json;

namespace Install.IO_Pro
{
    public partial class InstallForm : Form
    {
        public Process splash;
        public InstallForm()
        {
            CommonUtils.Encrpter.Init();
            splash = Process.Start("SplashScreen.exe", "'Install.IO Pro'");
            splash.WaitForInputIdle();
            int a = new Random().Next(1000, 3500);
            Thread.Sleep(a);
            TransparencyKey = Color.Thistle;
            InitializeComponent();
            JEF.Windows.MinimizeWindow(Process.GetCurrentProcess().MainWindowHandle);
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
            c.InstallMenu.TextObjects.Add(new CommonUtils.TextObject());
            File.WriteAllText("config.json",
                JsonSerializer.Serialize(c, jsonSerializerOptions));
            BeginInvoke((MethodInvoker)delegate
            {
                LoadingLabel.Text = "Restarting";
                LoadingLabel.Refresh();
            });
            Process ConfigProcess = Process.Start((ProcessStartInfo)new("config.json") { UseShellExecute = true, });
            Process RestartProcess = Process.Start((ProcessStartInfo)new(AppDomain.CurrentDomain.FriendlyName+".exe") { UseShellExecute = true, });
            RestartProcess.WaitForInputIdle();
            Thread.Sleep(750); // Let Render
            Environment.Exit(0);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void LoadingLabel_Click(object sender, EventArgs e)
        {

        }
    }
}