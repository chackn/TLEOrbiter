﻿// Copyright (c) 2016 SolarLiner - Part of the TLE Orbiter Sceneraio Generator (TLEOSG)
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TLEOrbiter
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
            Log.Write("new SettingsForm()");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = TB_OrbPath.Text;

            if (fbd.ShowDialog() != DialogResult.OK) return;
            TB_OrbPath.Text = fbd.SelectedPath;
        }

        private void BT_OK_Click(object sender, EventArgs e)
        {
            string CultureName = Settings.Languages.First(k => k.Value == CB_Lang.Text).Key;
            Settings.Lang = new CultureInfo(CultureName);
            Settings.OrbiterPath = TB_OrbPath.Text;
            Settings.Save();
            this.Close();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            foreach(KeyValuePair<string,string> kvp in Settings.Languages)
            {
                CB_Lang.Items.Add(kvp.Value);
            }
            CB_Lang.SelectedItem = Settings.Languages[Settings.Lang.Name];

            TB_OrbPath.Text = Settings.OrbiterPath;
        }

        private void BT_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
    public static class Settings
    {
        public static CultureInfo Lang { get; set; }
        public static string OrbiterPath { get; set; }

        static readonly string _config = Path.Combine(Application.StartupPath, "Config.cfg");

        // List of available languages, 
        public static Dictionary<string, string> Languages
        {
            get
            {
                Dictionary<string, string> d = new Dictionary<string, string>();
                d.Add("en-UK", "English (Traditional)");
                d.Add("en-US", "English (Simplified)");
                d.Add("fr-FR", "Français");
                //TODO: Add more languages to translate

                return d;
            }
        }

        public static void Load()
        {
            CultureInfo _lang = Application.CurrentCulture;
            try
            {
                Log.Write("Settings.Load()");
                foreach (string line in File.ReadAllLines(_config))
                {
                    string[] content = line.Split('=');
                    if (content.Length != 2) continue;

                    switch (content[0].Trim())
                    {
                        case "OrbiterPath":
                            Log.Write("Settings: Found OrbiterPath: " + content[1]);
                            OrbiterPath = content[1].Trim();
                            break;
                        case "Language":
                            Log.Write("Settings: Found language: " + content[1]);
                            _lang = new CultureInfo(content[1].Trim());
                            break;
                    }
                }
            }
            catch { }

            Lang = _lang; // That way if no languages are found, use the system one
        }

        public static void Save()
        {
            StringBuilder sb = new StringBuilder();
            Log.Write("Settings.Save()");

            sb.AppendLine(string.Format("; TLE Scenario Generator - v {0}", Application.ProductVersion));
            sb.AppendLine();
            sb.AppendLine("; Path to Orbiter root");
            sb.AppendLine("OrbiterPath = " + OrbiterPath);
            sb.AppendLine("; Application language");
            sb.AppendLine("Language = " + Lang.Name);

            File.WriteAllText(_config, sb.ToString());
        }
    }
}
