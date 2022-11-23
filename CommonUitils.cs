using Install.IO_Pro.WindowsForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Install.IO_Pro
{
    internal class CommonUitils
    {
        public class ProjectConfig
        {
            public string Title { get; set; } = "FormTitle";
            public string Mode { get; set; } = "Production";
            public int UpdateSpeed { get; set; } = 1000;
            public ThemeConfig Theme { get; set; } = new();
            public List<MenuConfig> Menus { get; set; } = new List<MenuConfig>();
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
            public bool Enabled { get; set; } = false;
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
