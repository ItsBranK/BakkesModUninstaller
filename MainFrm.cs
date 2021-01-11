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
using System.Diagnostics;

namespace BakkesModUninstaller
{
    public partial class MainFrm : Form
    {
        string BAKKESMOD_FOLDER = "(null)";
        bool DIRECTORY_ERROR = false;
        bool REGISTRY_ERROR = false;
        bool LOG_ERROR = false;

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
                getDirectory(false);
            }
            else
            {
                Environment.Exit(1);
            }
        }

        public void getDirectory(bool manuallyChooseDir)
        {
            if (!manuallyChooseDir)
            {
                string directory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\bakkesmod\\bakkesmod";

                if (Directory.Exists(directory))
                    BAKKESMOD_FOLDER = directory;
            }
            else
            {
                OpenFileDialog ofd = new OpenFileDialog
                {
                    Title = "Select plugininstaller.exe",
                    Filter = "EXE Files (*.exe)|*.exe"
                };

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string filePath = ofd.FileName;
                    filePath = filePath.Replace("plugininstaller.exe", "");

                    if (Directory.Exists(filePath))
                        BAKKESMOD_FOLDER = filePath;
                }
            }

            removeDirectory();
        }

        public void removeDirectory()
        {
            if (!Directory.Exists(BAKKESMOD_FOLDER))
            {
                MessageBox.Show("Could not find your BakkesMod folder, please manually point to where your RocketLeague.exe is located.", "BakkesMod Uninstaller", MessageBoxButtons.OK, MessageBoxIcon.Error);
                getDirectory(true);
            }
            else
            {
                try
                {
                    Directory.Delete(BAKKESMOD_FOLDER, true);
                    DIRECTORY_ERROR = false;
                }
                catch (Exception)
                {
                    DIRECTORY_ERROR = true;
                }
            }

            removeLogs();
        }

        public void removeLogs()
        {
            string bmInjectorLog = Path.GetTempPath() + "\\injectorlog.log";
            string csInjectorLog = Path.GetTempPath() + "\\BakkesModInjectorCs.log";
            string branksmodLog = Path.GetTempPath() + "\\branksmod.log";

            if (File.Exists(bmInjectorLog))
            {
                try
                {
                    File.Delete(bmInjectorLog);
                    LOG_ERROR = false;
                }
                catch (Exception ex)
                {
                    LOG_ERROR = true;
                }
            }

            if (File.Exists(csInjectorLog))
            {
                try
                {
                    File.Delete(csInjectorLog);
                }
                catch (Exception ex)
                {

                }
            }

            if (File.Exists(branksmodLog))
            {
                try
                {
                    File.Delete(branksmodLog);
                }
                catch (Exception ex)
                {

                }
            }

            removeRegistry();
        }

        public void removeRegistry()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            string[] names = key.GetValueNames();

            if (names.Contains("BakkesMod"))
            {
                try
                {
                    key.DeleteValue("BakkesMod", false);
                    REGISTRY_ERROR = false;
                }
                catch (Exception ex)
                {
                    REGISTRY_ERROR = true;
                }
            }

            if (names.Contains("BakkesModInjectorCs"))
            {
                try
                {
                    key.DeleteValue("BakkesModInjectorCs", false);
                }
                catch (Exception ex)
                {

                }
            }

            confirmUninstall();
        }

        public void confirmUninstall()
        {
            if (DIRECTORY_ERROR)
            {
                DialogResult directoryResult = MessageBox.Show("There was an error trying to remove the directory, would you like to try again?", "BakkesMod Uninstaller", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
               
                if (directoryResult == DialogResult.Yes)
                    removeDirectory();
            }
            else if (REGISTRY_ERROR)
            {
                DialogResult registryResult = MessageBox.Show("There was an error trying to remove the startup registry keys, would you like to try again?", "BakkesMod Uninstaller", MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                if (registryResult == DialogResult.Yes)
                    removeRegistry();
            }
            else if (LOG_ERROR)
            {
                DialogResult registryResult = MessageBox.Show("There was an error trying to remove the log files, would you like to try again?", "BakkesMod Uninstaller", MessageBoxButtons.YesNo, MessageBoxIcon.Error);

                if (registryResult == DialogResult.Yes)
                    removeLogs();
            }
            else
            {
                MessageBox.Show("BakkesMod has successfully been uninstalled.", "BakkesMod Uninstaller", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Environment.Exit(1);
            }
        }
    }
}
