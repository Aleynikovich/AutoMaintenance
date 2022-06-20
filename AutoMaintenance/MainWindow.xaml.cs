using Microsoft.Win32;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System;
using System.Windows.Controls;
using Microsoft.Office.Interop.Word;

namespace AutoMaintenance
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {


        string[] filePath = new string[100]; //Path buffer for selected .zip files
        int zipCounter = 0;

        /// <summary>
        /// General functions
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }


        private void Close_OnClick_(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Import file button, opens file search dialog to allow selection of .zip
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void Import_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = true,                 //Allow multiple zips to be imported
                Filter = "ZIP Files (*.zip)|*.zip"  //Allow only .zip file type
            };


            if (openFileDialog.ShowDialog() == true)
            {
                zipCounter = 0;
                foreach (string item in openFileDialog.FileNames)       //Fill filePath strings with the selected .zip file paths
                {
                    _ = fileList.Items.Add(Path.GetFileName(item));
                    filePath[zipCounter] = Path.GetFullPath(item);
                    //Trace.WriteLine("added " + filePath[zipCounter]);
                    zipCounter++;
                }
            }

        }

        /// <summary>
        /// Clear imported .zip file list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void Clear_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            fileList.Items.Clear();
            zipCounter = 0;
        }


        /// <summary>
        /// Transform imported .zip files into maintenance report in docx file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param

        private void Start_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            Directory.CreateDirectory("Mantenimiento");
            Directory.CreateDirectory("Programas");

            ZipArchiveEntry amIniEntry, configEntry, madaEntry;
            string amIniContent, configContent, madaContent, amInitempFile, configTempFile, madaTempFile;
            Krc tempKrc = new Krc();

            if (zipCounter > 0)
            {
                for (var i = 0; i < zipCounter; i++)
                {
                    using (ZipArchive archive = ZipFile.OpenRead(filePath[i])) //Open .zip in read mode
                    {
                        amIniEntry = archive.GetEntry("am.ini");
                        if (amIniEntry != null)
                        {
                            amInitempFile = Path.GetTempFileName();
                            amIniEntry.ExtractToFile(amInitempFile, true);
                            amIniContent = File.ReadAllText(amInitempFile);

                            tempKrc.Name = Libs.StringManipulation.GetBetween(amIniContent, "RobName=", "IRSerialNr=");
                            tempKrc.SerialNo = Libs.StringManipulation.GetBetween(amIniContent, "IRSerialNr=", "[Version]");
                            tempKrc.Version = Libs.StringManipulation.GetBetween(amIniContent, "[Version]", "[TechPacks]");
                            tempKrc.Tech = Libs.StringManipulation.GetBetween(amIniContent, "[TechPacks]", "default");

                        }

                        configEntry = archive.GetEntry("KRC/R1/System/$config.dat");
                        if (configEntry != null)
                        {
                            configTempFile = Path.GetTempFileName();
                            configEntry.ExtractToFile(configTempFile, true);
                            configContent = File.ReadAllText(configTempFile);

                            tempKrc.LoadData = "kek";
                            //tempKrc.LoadData = Libs.StringManipulation.GetBetween(configContent, "LOAD_DATA[16]\r\n", "\r\n\r\n");
                        }

                        madaEntry = archive.GetEntry("KRC/R1/Mada/$machine.dat");
                        if (madaEntry != null)
                        {
                            madaTempFile = Path.GetTempFileName();
                            madaEntry.ExtractToFile(madaTempFile, true);
                            madaContent = File.ReadAllText(madaTempFile);

                            tempKrc.Type = Libs.StringManipulation.GetBetween(madaContent, "$TRAFONAME[]=\"#", " ");
                        }

                        //Trace.WriteLine(tempKrc.SerialNo.TrimEnd('\n'));
                        Directory.CreateDirectory(@"Programas\" + tempKrc.SerialNo.TrimEnd('\r', '\n'));
                        Directory.CreateDirectory(@"Programas\" + tempKrc.SerialNo.TrimEnd('\r', '\n') + "\\" + tempKrc.SerialNo.TrimEnd('\r', '\n') + " - " + System.DateTime.Now.Year + "-" + System.DateTime.Now.Month + "-" + System.DateTime.Now.Day);

                        var programPath = @"Programas\" + tempKrc.SerialNo.TrimEnd('\r', '\n') + "\\" + tempKrc.SerialNo.TrimEnd('\r', '\n') + " - " + System.DateTime.Now.Year + "-" + System.DateTime.Now.Month + "-" + System.DateTime.Now.Day + "\\" + Path.GetFileName(filePath[i]);
                        if (File.Exists(programPath))
                        {
                            File.Delete(programPath);
                        }


                        var templatePath = Path.GetFullPath(@"plantillaAutoMaintenance.docx");
                        var dirPath = Path.GetFullPath(@"Mantenimiento\");
                        File.Copy(filePath[i], @"Programas\" + tempKrc.SerialNo.TrimEnd('\r', '\n') + "\\" + tempKrc.SerialNo.TrimEnd('\r', '\n') + " - " + System.DateTime.Now.Year + "-" + System.DateTime.Now.Month + "-" + System.DateTime.Now.Day + "\\" + Path.GetFileName(filePath[i]));
                        WordLibs.CreateWordDocument(templatePath, dirPath + tempKrc.SerialNo.TrimEnd('\r', '\n') + " - " + System.DateTime.Now.Year + "-" + System.DateTime.Now.Month + "-" + System.DateTime.Now.Day + ".docx", tempKrc);

                    }

                }

                MessageBox.Show("Informes generados");
                fileList.Items.Clear();
                zipCounter = 0;

            }
        }
    }
}
