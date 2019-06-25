using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;

namespace BakkesModUninstaller
{
    public partial class MainFrm : Form
    {
        Boolean DirectoryError = false;
        Boolean RegistryError = false;
        Boolean LogError = false;
        Boolean PickDirectory = false;
        string DirectoryPath;

        public MainFrm()
        {
            InitializeComponent();
        }

        private void MainFrm_Load(object sender, EventArgs e)
        {
            this.Hide();
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to uninstall BakkesMod? This will remove all files and registry keys.", "BakkesMod Uninstaller", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                GetDirectory();
            }
            else if (dialogResult == DialogResult.No)
            {
                this.Close();
            }
        }

        public void GetDirectory()
        {
            string MyDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string LogDir = MyDocuments + "\\My Games\\Rocket League\\TAGame\\Logs\\";
            string LogFile = LogDir + "launch.log";
            if (!PickDirectory == true)
            {
                if (File.Exists(LogFile))
                {
                    string Line;
                    using (FileStream Stream = File.Open(LogFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        StreamReader File = new StreamReader(Stream);
                        while ((Line = File.ReadLine()) != null)
                        {
                            if (Line.Contains("Init: Base directory: "))
                            {
                                Line = Line.Replace("Init: Base directory: ", "");
                                DirectoryPath = Line;
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                OpenFileDialog OFD = new OpenFileDialog
                {
                    Title = "Select RocketLeague.exe",
                    Filter = "EXE Files (*.exe)|*.exe"
                };
                if (OFD.ShowDialog() == DialogResult.OK)
                {
                    string FilePath = OFD.FileName;
                    FilePath = FilePath.Replace("RocketLeague.exe", "");
                    DirectoryPath = FilePath;
                }
            }
            RemoveDirectory();
        }

        public void RemoveDirectory()
        {
            string FolderPath = DirectoryPath + "bakkesmod";
            if (!Directory.Exists(FolderPath))
            {
                MessageBox.Show("Could not find the directory, please manually point to where your RocketLeague.exe is located.", "BakkesMod Uninstaller", MessageBoxButtons.OK, MessageBoxIcon.Error);
                PickDirectory = true;
                GetDirectory();
            }
            else
            {
                try
                {
                    Directory.Delete(FolderPath, true);
                }
                catch (Exception)
                {
                    DirectoryError = true;
                }
            }
            RemoveLog();
        }

        public void RemoveLog()
        {
            string BakkesLog = Path.GetTempPath() + "injectorlog.log";
            string BranksLog = Path.GetTempPath() + "branksmod.log";
            if (File.Exists(BakkesLog))
            {
                try
                {
                    File.Delete(BakkesLog);
                }
                catch (Exception)
                {
                    LogError = true;
                }
            }
            if (File.Exists(BranksLog))
            {
                try
                {
                    File.Delete(BranksLog);
                }
                catch (Exception)
                {

                }
            }
            RemoveRegistry();
        }

        public void RemoveRegistry()
        {
            RegistryKey Key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            try
            {
                Key.DeleteValue("BakkesMod", false);
            }
            catch (Exception)
            {
                RegistryError = true;
            }
            ConfirmUninstall();
        }

        public void ConfirmUninstall()
        {
            if (DirectoryError == true || RegistryError == true || LogError == true)
            {
                if (DirectoryError == true)
                {
                    DialogResult DirectoryResult = MessageBox.Show("There was an error trying to remove the directory, would you like to try again?", "BakkesMod Uninstaller", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                    if (DirectoryResult == DialogResult.Yes)
                    {
                        RemoveDirectory();
                    }
                }
                else if (RegistryError == true)
                {
                    DialogResult RegistryResult = MessageBox.Show("There was an error trying to remove the startup registry, would you like to try again?", "BakkesMod Uninstaller", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                    if (RegistryResult == DialogResult.Yes)
                    {
                        RemoveRegistry();
                    }
                }
                else if (LogError == true)
                {
                    DialogResult RegistryResult = MessageBox.Show("There was an error trying to remove the log files, would you like to try again?", "BakkesMod Uninstaller", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                    if (RegistryResult == DialogResult.Yes)
                    {
                        RemoveLog();
                    }
                }
                else
                {
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("BakkesMod has successfully been uninstalled.", "BakkesMod Uninstaller", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
        }
    }
}
