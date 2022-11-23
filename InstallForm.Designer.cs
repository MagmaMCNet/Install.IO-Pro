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

        bool ProductionMode = false;
        bool StillUpdating = false;
        #nullable enable
        CommonUtils.ProjectConfig? config = null;
        #nullable disable

        private void ConfigUpdate()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        config =
            JsonSerializer.Deserialize<CommonUtils.ProjectConfig>(
                File.ReadAllText("config.json"),
                jsonSerializerOptions);
                        Thread.Sleep(5000);
                    }
                    catch { };
                }
            });
        }
        protected void OnLoad()
        {
            ConfigUpdate();
            Task.Run(() =>
            {
                Thread.Sleep(50); // Let Program Start
                while (true)
                {
                    try
                    {

                        while (config == null)
                            Thread.Sleep(50);

                        ProductionMode = !config.DeveloperMode;
                        Update(config, ProductionMode);
                        Thread.Sleep(50); // Load

                        while (StillUpdating)
                            Thread.Sleep(50); // Wait For Frame To Finish Rendering

                        if (ProductionMode)
                            break;

                    Thread.Sleep( (int)(config.UpdateSpeed) ); // Sleep For User Defined Time
                    }
                    catch { Thread.Sleep(2000); /* Config Not Found */ }
                }
            });
        }

        private void Update(CommonUtils.ProjectConfig config, bool Production)
        {
            try
            {
                StillUpdating = true;
                List<Control> NewControls = new List<Control>();


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
                    label.Size = new System.Drawing.Size((int)((Text.Font.Size * Text.Text.Length) * 0.8f), (int)(Text.Font.Size * 1.7f));
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
                        }
                        catch
                        {
                            obj.BackColor = ColorTranslator.FromHtml(config.Theme.AccentColor);
                        }
                    }
                }
                #endregion


                foreach (Control obj in this.Controls)
                {
                    DefualtObjects.Add(obj);
                }

                foreach (Control obj in DefualtObjects)
                {
                    obj.ForeColor = ColorTranslator.FromHtml(config.Theme.TextColor);
                    NewControls.Add(obj);
                }

                //

                this.BeginInvoke((MethodInvoker)delegate
                {
                    this.SuspendLayout();
                    this.BackColor = System.Drawing.ColorTranslator.FromHtml(config.Theme.BackgroundColor);
                    this.FooterPanel__.BackColor = ColorTranslator.FromHtml(config.Theme.AltBackgroundColor);

                    this.Text = Production ? config.Menus[CurrentMenu].Title : config.Menus[CurrentMenu].Title + " -dev";
                    this.Name = Production ? config.Menus[CurrentMenu].Title : config.Menus[CurrentMenu].Title + " -dev";



                    Thread.Sleep(15); // Stop High CPU/RAM Usage
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
                    this.ResumeLayout();
                });

            }
            catch { }
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
                    File.WriteAllText("config.json", JsonSerializer.Serialize<CommonUtils.ProjectConfig>(config, jsonSerializerOptions));
                });
            }
            catch { }
        }
        public void NextPage_click()
        {

            try
            {
                CommonUtils.ProjectConfig config =
                        JsonSerializer.Deserialize<CommonUtils.ProjectConfig>(
                            File.ReadAllText("config.json"),
                            jsonSerializerOptions);


                if (config.Menus.Count-1 > CurrentMenu)
                {
                    CurrentMenu++;
                    Update(config, ProductionMode);
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
                CommonUtils.ProjectConfig config =
                        JsonSerializer.Deserialize<CommonUtils.ProjectConfig>(
                            File.ReadAllText("config.json"),
                            jsonSerializerOptions);


                if (0 < CurrentMenu)
                {
                    CurrentMenu--;
                    Update(config, ProductionMode);
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
            this.NextPage__ = new Install.IO_Pro.WindowsForms.BetterButtons();
            this.InstallBar = new System.Windows.Forms.ProgressBar();
            this.BackPage__ = new Install.IO_Pro.WindowsForms.BetterButtons();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.LoadingLabel = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.FooterPanel__.SuspendLayout();
            this.panel4.SuspendLayout();
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
            this.NextPage__.ForeColor = System.Drawing.Color.Azure;
            this.NextPage__.Location = new System.Drawing.Point(626, 18);
            this.NextPage__.Name = "NextPage__";
            this.NextPage__.Size = new System.Drawing.Size(155, 51);
            this.NextPage__.TabIndex = 10;
            this.NextPage__.Text = "Next";
            this.NextPage__.TextColor = System.Drawing.Color.Azure;
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
            this.BackPage__.ForeColor = System.Drawing.Color.Azure;
            this.BackPage__.Location = new System.Drawing.Point(21, 18);
            this.BackPage__.Name = "BackPage__";
            this.BackPage__.Size = new System.Drawing.Size(155, 51);
            this.BackPage__.TabIndex = 10;
            this.BackPage__.Text = "Back";
            this.BackPage__.TextColor = System.Drawing.Color.Azure;
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
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.Indigo;
            this.panel3.Location = new System.Drawing.Point(-6, 326);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(804, 72);
            this.panel3.TabIndex = 14;
            // 
            // LoadingLabel
            // 
            this.LoadingLabel.AutoSize = true;
            this.LoadingLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(17)))));
            this.LoadingLabel.Font = new System.Drawing.Font("Roboto Mono", 39.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.LoadingLabel.ForeColor = System.Drawing.SystemColors.Control;
            this.LoadingLabel.Location = new System.Drawing.Point(282, 99);
            this.LoadingLabel.Name = "LoadingLabel";
            this.LoadingLabel.Size = new System.Drawing.Size(254, 70);
            this.LoadingLabel.TabIndex = 1;
            this.LoadingLabel.Text = "LOADING";
            // 
            // button1
            // 
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Roboto Mono", 13F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.button1.ForeColor = System.Drawing.SystemColors.Highlight;
            this.button1.Location = new System.Drawing.Point(299, 235);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(218, 43);
            this.button1.TabIndex = 2;
            this.button1.Text = "Create Config.json";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(17)))));
            this.label1.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.ForeColor = System.Drawing.Color.Snow;
            this.label1.Location = new System.Drawing.Point(58, 70);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(146, 37);
            this.label1.TabIndex = 3;
            this.label1.Text = "config.json";
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(17)))));
            this.panel4.Controls.Add(this.label1);
            this.panel4.Location = new System.Drawing.Point(282, 99);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(254, 116);
            this.panel4.TabIndex = 15;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Roboto Mono", 30F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(255)))));
            this.label2.Location = new System.Drawing.Point(227, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(359, 53);
            this.label2.TabIndex = 16;
            this.label2.Text = "Install.IO Pro";
            // 
            // InstallForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ClientSize = new System.Drawing.Size(784, 392);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.LoadingLabel);
            this.Controls.Add(this.FooterPanel__);
            this.Controls.Add(this.panel4);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "InstallForm";
            this.Load += new System.EventHandler(this.InstallForm_Load);
            this.FooterPanel__.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
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
        private Panel panel4;
        private Label label2;
    }
}