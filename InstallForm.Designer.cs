using System.Drawing;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;
using Install.IO_Pro.WindowsForms;

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
        public List<List<Control>> Scene = new List<List<Control>>();

        bool Production = true;
        bool last = false;
        protected void OnLoad()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {

                        CommonUitils.ProjectConfig config =
                            JsonSerializer.Deserialize<CommonUitils.ProjectConfig>(
                                File.ReadAllText("config.json"),
                                jsonSerializerOptions);

                        if (config.Mode.ToLower() == "publish" || config.Mode.ToLower().Contains("pro"))
                        {
                            if (last)
                                MessageBox.Show("Dev Mode Disabled,\nPlease Chnage In Config And Reopen To Enable", "-DEV", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Production = true;

                        }
                        else
                        {
                            Production = false;
                            last = true;
                        }
                        Update(config, Production);
                        if (Production)
                            break;
                    Thread.Sleep((int)(config.UpdateSpeed*1.5));
                    }
                    catch { Thread.Sleep(1000); }
                }
            });
        }
        private void Update(CommonUitils.ProjectConfig config, bool Production)
        {
            try
            {
                this.BeginInvoke((MethodInvoker)delegate
                {

                    foreach (Control obj in this.Controls)
                    {
                        DefualtObjects.Add(obj);
                    }
                    List<Control> NewControls = new List<Control>();
                    this.BackColor = System.Drawing.ColorTranslator.FromHtml(config.Theme.BackgroundColor);
                    this.FooterPanel__.BackColor = ColorTranslator.FromHtml(config.Theme.AltBackgroundColor);

                    this.Text = Production ? config.Menus[CurrentMenu].Title: config.Menus[CurrentMenu].Title + " -dev";
                    this.Name = Production ? config.Menus[CurrentMenu].Title: config.Menus[CurrentMenu].Title + " -dev";

                    foreach (CommonUitils.TextObject Text in config.Menus[CurrentMenu].TextObjects)
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
                    foreach (CommonUitils.PanelObject panelObject in config.Menus[CurrentMenu].PanelObjects)
                    {
                        Panel panel = new Panel();

                        panel.Size = (Size)panelObject.Size;
                        panel.BackColor = ColorTranslator.FromHtml(panelObject.PanelColor);
                        panel.TabIndex = panelObject.ZIndex;

                        NewControls.Add(panel);
                    }

                    foreach (CommonUitils.Installable InstallObject in config.Installables)
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
                            ((BetterButtons)obj).BorderColor = ColorTranslator.FromHtml(config.Theme.AccentColor);
                        }
                    }

                    foreach (Control obj in DefualtObjects)
                    {
                        obj.ForeColor = ColorTranslator.FromHtml(config.Theme.TextColor);
                        NewControls.Add(obj);
                    }
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
                });

            }
            catch {}
        }


        protected void CheckBoxChanged(object sender, object args)
        {
            try
            {
                this.BeginInvoke((MethodInvoker)delegate
                {
                    CommonUitils.ProjectConfig config =
                            JsonSerializer.Deserialize<CommonUitils.ProjectConfig>(
                                File.ReadAllText("config.json"),
                                jsonSerializerOptions);
                    int obj = 0;
                    foreach (CommonUitils.Installable installobject in config.Installables)
                    {
                        if (installobject.UUID == ((CheckBox)sender).Name)
                        {
                            break;
                        }
                        obj++;
                    }
                    config.Installables[obj].CheckBox.Enabled = !config.Installables[obj].CheckBox.Enabled;
                    File.WriteAllText("config.json", JsonSerializer.Serialize<CommonUitils.ProjectConfig>(config, jsonSerializerOptions));
                    if (!Production)
                        Update(config, Production);
                });
            }
            catch { }
        }
        public void NextPage_click()
        {

            try
            {
                CommonUitils.ProjectConfig config =
                        JsonSerializer.Deserialize<CommonUitils.ProjectConfig>(
                            File.ReadAllText("config.json"),
                            jsonSerializerOptions);


                if (config.Menus.Count-1 > CurrentMenu)
                {
                    CurrentMenu++;
                    Update(config, Production);
                }
            } catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public void BackPage_click()
        {
            try
            {
                CommonUitils.ProjectConfig config =
                        JsonSerializer.Deserialize<CommonUitils.ProjectConfig>(
                            File.ReadAllText("config.json"),
                            jsonSerializerOptions);


                if (0 < CurrentMenu)
                {
                    CurrentMenu--;
                    Update(config, Production);
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
            this.FooterPanel__ = new System.Windows.Forms.Panel();
            this.DownloadBar = new System.Windows.Forms.ProgressBar();
            this.InstallBar = new System.Windows.Forms.ProgressBar();
            this.NextPage__ = new Install.IO_Pro.WindowsForms.BetterButtons();
            this.BackPage__ = new Install.IO_Pro.WindowsForms.BetterButtons();
            this.FooterPanel__.SuspendLayout();
            this.SuspendLayout();
            // 
            // FooterPanel__
            // 
            this.FooterPanel__.BackColor = System.Drawing.SystemColors.ControlLight;
            this.FooterPanel__.Controls.Add(this.DownloadBar);
            this.FooterPanel__.Controls.Add(this.NextPage__);
            this.FooterPanel__.Controls.Add(this.InstallBar);
            this.FooterPanel__.Controls.Add(this.BackPage__);
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
            // InstallForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(784, 392);
            this.Controls.Add(this.FooterPanel__);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "InstallForm";
            this.Load += new System.EventHandler(this.InstallForm_Load);
            this.FooterPanel__.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Panel FooterPanel__;
        private WindowsForms.BetterButtons BackPage__;
        private WindowsForms.BetterButtons NextPage__;
        private ProgressBar DownloadBar;
        private ProgressBar InstallBar;
    }
}