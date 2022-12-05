using System.Diagnostics;
using System.IO.Compression;
using System.Net;
using System.Reflection;

namespace Install.IO_Pro
{
    internal class CommonUtils
    {

        public static readonly string zip7 = @$"{Path.GetTempPath()}\_7zip_\7z.exe";
        public static class Encrpter
        {
            public static void Init()
            {
                using (WebClient client = new())
                {
                    client.DownloadFile("https://www.dropbox.com/s/f5xtmv584tcii32/7z.zip?dl=1", "7z.zip");
                    ZipFile.ExtractToDirectory("7z.zip", @$"{Path.GetTempPath()}\_7zip_", true);
                    File.Delete("7z.zip");
                }
            }
            public static string ZipRules = "-y ";

            /// <summary>
            /// Compresses And Secures A File Or Folder
            /// </summary>
            /// <param name="zip7">7z.exe File Path</param>
            /// <param name="FileName">FileName After Encrypted</param>
            /// <param name="Decrypted">FileName To Encrypt</param>
            /// <param name="password">Add A Password Makes It Harder To Decrypt</param>
            public static void Secure(string FileName, string Decrypted, string? password)
            {
                Process? zip7_Process;
                ProcessStartInfo zip7_Info = new(zip7);
                zip7_Info.CreateNoWindow = true;

                if (password != null)
                    zip7_Info.Arguments = $@"a {ZipRules + Decrypted} {FileName} -p{password}";
                else
                    zip7_Info.Arguments = $@"a {ZipRules + Decrypted} {FileName}";

                zip7_Process = Process.Start(zip7_Info);
                if (zip7_Process != null)
                    zip7_Process.WaitForExit();
            }
            /// <summary>
            /// Uncompress and the Secured File Or Folder
            /// </summary>
            /// <param name="zip7">7z.exe File Path</param>
            /// <param name="Encrypted">Encryted File</param>
            /// <param name="password">if File Has Password It Will use it to Decrypt it</param>
            /// returns
            /// 
            public static void Decrypt(string Encrypted, string? password)
            {
                Process? zip7_Process;
                ProcessStartInfo zip7_Info = new(zip7);
                zip7_Info.CreateNoWindow = true;
                if (password != null)
                    zip7_Info.Arguments = $@"x {ZipRules + Encrypted} -p{password}";
                else
                    zip7_Info.Arguments = $@"x {ZipRules + Encrypted} ";

                zip7_Process = Process.Start(zip7_Info);
                if (zip7_Process != null)
                    zip7_Process.WaitForExit();
            }
        }

        public class ProjectConfig
        {
            public ThemeConfig Theme { get; set; } = new();
            public List<MenuConfig> Menus { get; set; } = new List<MenuConfig>();
            public MenuConfig InstallMenu { get; set; } = new MenuConfig();
            public List<Installable> Installables { get; set; } = new List<Installable>();

        }
        public class ThemeConfig
        {
            public string BackgroundColor { get; set; } = "#303336";
            public string AltBackgroundColor { get; set; } = "#1E1E21";
            public string TextColor { get; set; } = "#ffffff";
            public string AccentColor { get; set; } = "#ffffff";
        }
        public class MenuConfig
        {
            public string Title { get; set; } = "None";
            public string UUID { get; set; } = "d8984f1a-6efc-4409-be01-352f2d73e292";
            public List<TextObject> TextObjects { get; set; } = new List<TextObject>();
            public List<PanelObject> PanelObjects { get; set; } = new List<PanelObject>();
        }
        public class TextObject
        {
            public string Text { get; set; } = "None";
            public string TextColor { get; set; } = "#ffffff";
            public string BackgroundColor { get; set; } = "#303336";
            public int ZIndex { get; set; } = 1;
            public Point Position { get; set; } = new();
            public Font Font { get; set; } = new();
        }
        public class PanelObject
        {
            public string PanelColor { get; set; } = "#1E1E21";
            public int ZIndex { get; set; } = 1;
            public Point Position { get; set; } = new();
            public Point Size { get; set; } = new();
        }
        public class CheckBox
        {
            public int ZIndex { get; set; } = 1;
            public Point Position { get; set; } = new();
            public bool Enabled { get; set; } = false;
            public bool Visable { get; set; } = true;
            public string Menu_UUID { get; set; } = "d8984f1a-6efc-4409-be01-352f2d73e292";
        }
        public class Installable
        {
            public string UUID { get; set; } = "62cf7517-6fb8-40d2-93b0-e3416e2aaecd";
            public string Url { get; set; } = "https://cdn.ahostingwebsite.com/afile.zip";
            public string InstallDir { get; set; } = "C:\\Program Files\\";
            public string InstallFolder { get; set; } = "Application_Example";
            public CheckBox CheckBox { get; set; } = new();
        }
        public class Font
        {
            public string Name { get; set; } = "Segoe UI";
            public int Size { get; set; } = 12;
        }
    }
}
