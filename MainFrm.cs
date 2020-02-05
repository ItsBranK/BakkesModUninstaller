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
        bool DIRECTORY_ERROR;
        bool REGISTRY_ERROR;
        bool LOG_ERROR;
        string WIN32_FOLDER;

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
            else if (dialogResult == DialogResult.No)
            {
                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\BakkesModInjectorCs.exe"))
                {
                    Process.Start(AppDomain.CurrentDomain.BaseDirectory + "\\BakkesModInjectorCs.exe");
                }

                Environment.Exit(1);
            }
        }

        public void getDirectory(bool pickDirectory)
        {
            string  myDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string logDirectory = myDocuments + "\\My Games\\Rocket League\\TAGame\\Logs\\launch.log";

            if (pickDirectory == false)
            {
                if (File.Exists(logDirectory))
                {
                    string line;

                    using (FileStream stream = File.Open(logDirectory, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        StreamReader file = new StreamReader(stream);
                        while ((line = file.ReadLine()) != null)
                        {
                            if (line.Contains("Init: Base directory: "))
                            {
                                line = line.Replace("Init: Base directory: ", "");
                                WIN32_FOLDER = line;
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                OpenFileDialog ofd = new OpenFileDialog
                {
                    Title = "Select RocketLeague.exe",
                    Filter = "EXE Files (*.exe)|*.exe"
                };

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string FilePath = ofd.FileName;

                    FilePath = FilePath.Replace("RocketLeague.exe", "");
                    WIN32_FOLDER = FilePath;
                }
            }

            removeDirectory();
        }

        public void removeDirectory()
        {
            string BAKKESMOD_FOLDER = WIN32_FOLDER + "\\bakkesmod";
            string WKSCLI_WIN32 = WIN32_FOLDER + "\\wkscli.dll";
            string WKSCLI_TEMP = Path.GetTempPath() + "\\wkscli.dll";

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

            if (File.Exists(WKSCLI_WIN32))
            {
                try
                {
                    File.Delete(WKSCLI_WIN32);
                }
                catch
                {

                }
            }

            if (File.Exists(WKSCLI_TEMP))
            {
                try
                {
                    File.Delete(WKSCLI_TEMP);
                }
                catch
                {

                }
            }

            removeLogs();
        }

        public void removeLogs()
        {
            string INJECTOR_LOG = Path.GetTempPath() + "\\injectorlog.log";
            string INJECTOR_CS_LOG = Path.GetTempPath() + "\\BakkesModInjectorCs.log";
            string BRANKS_LOG = Path.GetTempPath() + "\\branksmod.log";


            if (File.Exists(INJECTOR_LOG))
            {
                try
                {
                    File.Delete(INJECTOR_LOG);
                    LOG_ERROR = false;
                }
                catch (Exception)
                {
                    LOG_ERROR = true;
                }
            }

            if (File.Exists(INJECTOR_CS_LOG))
            {
                try
                {
                    File.Delete(INJECTOR_CS_LOG);
                }
                catch (Exception)
                {

                }
            }

            if (File.Exists(BRANKS_LOG))
            {
                try
                {
                    File.Delete(BRANKS_LOG);
                }
                catch (Exception)
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
                catch (Exception)
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
                catch (Exception)
                {

                }
            }

            if (names.Contains("BranksMod"))
            {
                try
                {
                    key.DeleteValue("BranksMod", false);
                }
                catch (Exception)
                {

                }
            }

            confirmUninstall();
        }

        public void confirmUninstall()
        {
            if (DIRECTORY_ERROR == true)
            {
                DialogResult directoryResult = MessageBox.Show("There was an error trying to remove the directory, would you like to try again?", "BakkesMod Uninstaller", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                
                if (directoryResult == DialogResult.Yes)
                {
                    removeDirectory();
                }
            }
            else if (REGISTRY_ERROR == true)
            {
                DialogResult registryResult = MessageBox.Show("There was an error trying to remove the startup registry keys, would you like to try again?", "BakkesMod Uninstaller", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                
                if (registryResult == DialogResult.Yes)
                {
                    removeRegistry();
                }
            }
            else if (LOG_ERROR == true)
            {
                DialogResult registryResult = MessageBox.Show("There was an error trying to remove the log files, would you like to try again?", "BakkesMod Uninstaller", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                
                if (registryResult == DialogResult.Yes)
                {
                    removeLogs();
                }
            }
            else
            {
                MessageBox.Show("BakkesMod has successfully been uninstalled.", "BakkesMod Uninstaller", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Environment.Exit(1);
            }
        }
    }
}
