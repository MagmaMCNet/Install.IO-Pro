using System.Diagnostics;
using System.Text.Json;
using Install.IO_Pro.WindowsForms;
using MagmaMc.JEF;
//using Microsoft.Toolkit.Uwp.Notifications;

namespace Install.IO_Pro
{
    partial class InstallForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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

        JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions()
        {
            AllowTrailingCommas = true,
            MaxDepth = 32,
            WriteIndented = true,
            IncludeFields = true,
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString,

        };
        public int CurrentMenu = 0;
        public List<object> DefualtObjects = new List<object>();

        bool ProductionMode = false;
        bool StillUpdating = false;
        private byte[] GetByteArray(int sizeInKb)
        {
            Random rnd = new Random();
            byte[] b = new byte[sizeInKb * 1024]; // convert kb to byte
            rnd.NextBytes(b);
            return b;
        }
        #nullable enable
        CommonUtils.ProjectConfig? Config = null;
#nullable disable

        public string ConfigFile = "config.json";
        private CommonUtils.ProjectConfig ConfigHandler(bool write = false)
        {
            try
            {

                if (write)
                    File.WriteAllText(ConfigFile, JsonSerializer.Serialize<CommonUtils.ProjectConfig>(Config,
                        jsonSerializerOptions), System.Text.Encoding.UTF8);
                return JsonSerializer.Deserialize<CommonUtils.ProjectConfig>(
                        File.ReadAllText(ConfigFile),
                        jsonSerializerOptions);

            } catch { return new();  }
        }

        protected void OnLoad()
        {
            if (!File.Exists(JEF.Utils.Folders.LocalUserAppData + "\\Install.IO-Pro\\ran.dat"))
            {
                //new ToastContentBuilder()
                //    .AddText("Install.IO Pro - Developer Mode")
                //    .AddText("Current App Completion")
                //    .AddProgressBar("Project", 0.15, false, "15%", "Alpha")
                //    .Show();
                    

                try { Directory.CreateDirectory(JEF.Utils.Folders.LocalUserAppData + "\\Install.IO-Pro\\"); } catch { }
                //File.WriteAllBytes(JEF.Utils.Folders.LocalUserAppData + "\\Install.IO-Pro\\ran.dat", GetByteArray(69));
            }
            ConfigFile = Path.GetTempPath() + "\\Install.Io.conf";
            try { File.Delete(ConfigFile); } catch { };
            try { CommonUtils.Encrpter.Decrypt("config.db", "Install.IO"); File.Copy("config.json", Path.GetTempPath() + "\\Install.Io.conf"); } catch { }
            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        Config = ConfigHandler();
                    }
                    catch { }
                    Thread.Sleep(500);
                }
            });
            Task.Run(() =>
            {
                Thread.Sleep(50); // Let Program Start
                while (Config == null)
                    Thread.Sleep(10);
                while (true)
                {
                    try
                    {


                        ProductionMode = true;
                        Update(Config, ProductionMode);
                        while (StillUpdating)
                            Thread.Sleep(100);
                        splash.Kill();
                        Thread.Sleep(200);
                        JEF.Windows.RestoreWindow(Process.GetCurrentProcess().MainWindowHandle);
                        break;
                    }
                    catch { Thread.Sleep(2000); /* Config Not Found */ }
                }
            });
        }

        private void Update(CommonUtils.ProjectConfig config, bool Production)
        {
            StillUpdating = true;
                this.BeginInvoke((MethodInvoker)delegate
                {

                try
                {
                    foreach (Control obj in this.Controls)
                    {
                        DefualtObjects.Add(obj);
                    }
                    List<Control> NewControls = new List<Control>();
                    this.BackColor = System.Drawing.ColorTranslator.FromHtml(config.Theme.BackgroundColor);
                    this.FooterPanel__.BackColor = ColorTranslator.FromHtml(config.Theme.AltBackgroundColor);

                    this.Text = config.Menus[CurrentMenu].Title;
                    this.Name = config.Menus[CurrentMenu].Title;

                    #region Get Config Objects
                    foreach (CommonUtils.TextObject Text in config.Menus[CurrentMenu].TextObjects)
                    {
                        Label label = new Label();
                        label.Text = Text.Text;
                        
                        label.Location = (Point)Text.Position;
                        label.ForeColor = ColorTranslator.FromHtml(Text.TextColor);
                        label.BackColor = ColorTranslator.FromHtml(Text.BackgroundColor);
                        label.Font = new System.Drawing.Font(
                            Text.Font.Name,
                            Text.Font.Size,
                            System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
                        label.TabIndex = Text.ZIndex;
                        label.Size = new System.Drawing.Size((int)((Text.Font.Size * Text.Text.Length)*0.8f), (int)(Text.Font.Size * 1.7f));
                        NewControls.Add(label);
                    }
                    foreach (CommonUtils.PanelObject panelObject in config.Menus[CurrentMenu].PanelObjects)
                    {
                        Panel panel = new Panel();

                        panel.Size = (Size)panelObject.Size;
                        panel.BackColor = ColorTranslator.FromHtml(panelObject.PanelColor);
                        panel.TabIndex = panelObject.ZIndex;

                        NewControls.Add(panel);
                    }

                    foreach (CommonUtils.Installable InstallObject in config.Installables)
                    {
                        if (config.Menus[CurrentMenu].UUID == InstallObject.CheckBox.Menu_UUID && InstallObject.CheckBox.Visable)
                        {
                            CheckBox CheckBox = new();

                            CheckBox.TabIndex = InstallObject.CheckBox.ZIndex;
                            CheckBox.Location = InstallObject.CheckBox.Position;
                            CheckBox.Checked = InstallObject.CheckBox.Enabled;
                            CheckBox.Text = InstallObject.InstallFolder;
                            CheckBox.FlatStyle = FlatStyle.System;
                            CheckBox.AutoSize = true;
                            CheckBox.Name = InstallObject.UUID;
                            CheckBox.CheckStateChanged += CheckBoxChanged;
                            NewControls.Add(CheckBox);
                        }

                       
                    }

                    foreach (Control obj in FooterPanel__.Controls)
                    {

                        if (obj.BackColor == Color.Azure)
                        {
                            obj.BackColor = ColorTranslator.FromHtml(config.Theme.AltBackgroundColor);
                            try
                            {
                                ((BetterButtons)obj).BorderColor = ColorTranslator.FromHtml(config.Theme.AccentColor);
                            } catch
                            {
                                obj.BackColor = ColorTranslator.FromHtml(config.Theme.AccentColor);
                            }
                            }
                    }
                    #endregion

                    foreach (Control obj in DefualtObjects)
                    {
                        obj.ForeColor = ColorTranslator.FromHtml(config.Theme.TextColor);
                        NewControls.Add(obj);
                    }

                    Thread.Sleep(5); // Stop High CPU Usage
                    int rid = 0;
                    while (this.Controls.Count > rid)
                    {
                        if (!this.Controls[rid].Name.Contains("__")) // Defualt Objects !Important!
                            this.Controls[rid].Dispose();
                        else
                        {
                            rid++;
                        }
                    }
                    this.Controls.Clear();
                    Application.DoEvents();
                    foreach (Control c in NewControls)
                    {
                        this.Controls.Add(c);
                        }
                    }
                    catch { }
                });

            StillUpdating = false;
        }


        protected void CheckBoxChanged(object sender, object args)
        {
            try
            {
                this.BeginInvoke((MethodInvoker)delegate
                {
                    CommonUtils.ProjectConfig config =
                            JsonSerializer.Deserialize<CommonUtils.ProjectConfig>(
                                File.ReadAllText("config.json"),
                                jsonSerializerOptions);
                    int obj = 0;
                    foreach (CommonUtils.Installable installobject in config.Installables)
                    {
                        if (installobject.UUID == ((CheckBox)sender).Name)
                        {
                            break;
                        }
                        obj++;
                    }
                    config.Installables[obj].CheckBox.Enabled = !config.Installables[obj].CheckBox.Enabled;
                    ConfigHandler(true);
                });
            }
            catch{}
        }
        public void NextPage_click()
        {

            try
            {


                if (Config.Menus.Count - 1 > CurrentMenu)
                {
                    CurrentMenu++;
                    Update(Config, ProductionMode);
                }
            }
            catch { }
        }

        public void BackPage_click()
        {
            try
            {


                if (0 < CurrentMenu)
                {
                    CurrentMenu--;
                    Update(Config, ProductionMode);
                }
            }
            catch { }
        }


        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InstallForm));
            this.FooterPanel__ = new System.Windows.Forms.Panel();
            this.DownloadBar = new System.Windows.Forms.ProgressBar();
            this.NextPage__ = new Install.IO_Pro.WindowsForms.BetterButtons();
            this.InstallBar = new System.Windows.Forms.ProgressBar();
            this.BackPage__ = new Install.IO_Pro.WindowsForms.BetterButtons();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.LoadingLabel = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.FooterPanel__.SuspendLayout();
            this.SuspendLayout();
            // 
            // FooterPanel__
            // 
            this.FooterPanel__.BackColor = System.Drawing.Color.Indigo;
            this.FooterPanel__.Controls.Add(this.DownloadBar);
            this.FooterPanel__.Controls.Add(this.NextPage__);
            this.FooterPanel__.Controls.Add(this.InstallBar);
            this.FooterPanel__.Controls.Add(this.BackPage__);
            this.FooterPanel__.Controls.Add(this.panel1);
            this.FooterPanel__.Controls.Add(this.panel2);
            this.FooterPanel__.Location = new System.Drawing.Point(-9, 311);
            this.FooterPanel__.Name = "FooterPanel__";
            this.FooterPanel__.Size = new System.Drawing.Size(804, 90);
            this.FooterPanel__.TabIndex = 0;
            // 
            // DownloadBar
            // 
            this.DownloadBar.BackColor = System.Drawing.SystemColors.ControlText;
            this.DownloadBar.Location = new System.Drawing.Point(182, 18);
            this.DownloadBar.MarqueeAnimationSpeed = 10;
            this.DownloadBar.Name = "DownloadBar";
            this.DownloadBar.Size = new System.Drawing.Size(438, 23);
            this.DownloadBar.Step = 5;
            this.DownloadBar.TabIndex = 13;
            // 
            // NextPage__
            // 
            this.NextPage__.BackColor = System.Drawing.Color.Azure;
            this.NextPage__.BackgroundColor = System.Drawing.Color.Azure;
            this.NextPage__.BorderColor = System.Drawing.Color.Indigo;
            this.NextPage__.BorderRadius = 10;
            this.NextPage__.BorderSize = 5;
            this.NextPage__.FlatAppearance.BorderSize = 0;
            this.NextPage__.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.NextPage__.Font = new System.Drawing.Font("Comic Mono", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.NextPage__.ForeColor = System.Drawing.Color.White;
            this.NextPage__.Location = new System.Drawing.Point(626, 18);
            this.NextPage__.Name = "NextPage__";
            this.NextPage__.Size = new System.Drawing.Size(155, 51);
            this.NextPage__.TabIndex = 10;
            this.NextPage__.Text = "Next";
            this.NextPage__.TextColor = System.Drawing.Color.White;
            this.NextPage__.UseVisualStyleBackColor = false;
            this.NextPage__.Click += new System.EventHandler(this.NextPage_Click);
            // 
            // InstallBar
            // 
            this.InstallBar.BackColor = System.Drawing.SystemColors.ControlText;
            this.InstallBar.Location = new System.Drawing.Point(182, 46);
            this.InstallBar.MarqueeAnimationSpeed = 10;
            this.InstallBar.Name = "InstallBar";
            this.InstallBar.Size = new System.Drawing.Size(438, 23);
            this.InstallBar.Step = 5;
            this.InstallBar.TabIndex = 12;
            // 
            // BackPage__
            // 
            this.BackPage__.BackColor = System.Drawing.Color.Azure;
            this.BackPage__.BackgroundColor = System.Drawing.Color.Azure;
            this.BackPage__.BorderColor = System.Drawing.Color.Indigo;
            this.BackPage__.BorderRadius = 10;
            this.BackPage__.BorderSize = 5;
            this.BackPage__.FlatAppearance.BorderSize = 0;
            this.BackPage__.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BackPage__.Font = new System.Drawing.Font("Comic Mono", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.BackPage__.ForeColor = System.Drawing.Color.White;
            this.BackPage__.Location = new System.Drawing.Point(21, 18);
            this.BackPage__.Name = "BackPage__";
            this.BackPage__.Size = new System.Drawing.Size(155, 51);
            this.BackPage__.TabIndex = 10;
            this.BackPage__.Text = "Back";
            this.BackPage__.TextColor = System.Drawing.Color.White;
            this.BackPage__.UseVisualStyleBackColor = false;
            this.BackPage__.Click += new System.EventHandler(this.BackPage_Click_1);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Azure;
            this.panel1.Location = new System.Drawing.Point(179, 15);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(444, 29);
            this.panel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Azure;
            this.panel2.Location = new System.Drawing.Point(179, 43);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(444, 29);
            this.panel2.TabIndex = 1;
            // 
            // LoadingLabel
            // 
            this.LoadingLabel.AutoSize = true;
            this.LoadingLabel.BackColor = System.Drawing.Color.Black;
            this.LoadingLabel.Font = new System.Drawing.Font("Segoe UI", 40F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LoadingLabel.ForeColor = System.Drawing.SystemColors.Control;
            this.LoadingLabel.Location = new System.Drawing.Point(282, 84);
            this.LoadingLabel.Name = "LoadingLabel";
            this.LoadingLabel.Size = new System.Drawing.Size(257, 72);
            this.LoadingLabel.TabIndex = 1;
            this.LoadingLabel.Text = "LOADING";
            this.LoadingLabel.Click += new System.EventHandler(this.LoadingLabel_Click);
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.Snow;
            this.button1.Location = new System.Drawing.Point(314, 233);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(196, 31);
            this.button1.TabIndex = 2;
            this.button1.Text = "Create Config.json";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.SystemColors.Desktop;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.ForeColor = System.Drawing.Color.Snow;
            this.label1.Location = new System.Drawing.Point(355, 156);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 28);
            this.label1.TabIndex = 3;
            this.label1.Text = "config.json";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.Indigo;
            this.panel3.Location = new System.Drawing.Point(-6, -2);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(798, 400);
            this.panel3.TabIndex = 4;
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.notifyIcon1.BalloonTipText = "a1";
            this.notifyIcon1.BalloonTipTitle = "b2";
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "notifyIcon1";
            this.notifyIcon1.Visible = true;
            // 
            // InstallForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Thistle;
            this.ClientSize = new System.Drawing.Size(784, 392);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.LoadingLabel);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.FooterPanel__);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "InstallForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.InstallForm_Load);
            this.FooterPanel__.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Panel FooterPanel__;
        private WindowsForms.BetterButtons BackPage__;
        private WindowsForms.BetterButtons NextPage__;
        private ProgressBar DownloadBar;
        private ProgressBar InstallBar;
        private Panel panel1;
        private Panel panel2;
        private Label LoadingLabel;
        private Button button1;
        private Label label1;
        private Panel panel3;
        public NotifyIcon notifyIcon1;
    }
}