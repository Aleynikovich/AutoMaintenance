using Microsoft.Win32;
using System.Windows;
using System.Windows.Input;
using System.IO;
using System.IO.Compression;
using System;
using Microsoft.Office.Interop.Word;
using System.Runtime.CompilerServices;
using System.Threading;

namespace AutoMaintenance
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {

        bool debugMode;
        string[] filePath = new string[300]; //Path buffer for selected .zip files
        int zipCounter = 0;
        enum ZipType { archive, krcDiag }

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
                    _ = fileList.Items.Add(System.IO.Path.GetFileName(item));
                    filePath[zipCounter] = System.IO.Path.GetFullPath(item);
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
            StringManipulation.DeleteDirectory("Mantenimiento");
            StringManipulation.DeleteDirectory("Programas");
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

            ZipType zipType;

            if (zipCounter > 0)
            {
                for (var i = 0; i < zipCounter; i++)
                {
                    using (ZipArchive archive = ZipFile.OpenRead(filePath[i])) //Open .zip in read mode
                    {
                        zipType = archive.GetEntry("am.ini") != null ? ZipType.archive : ZipType.krcDiag;

                        switch (zipType)
                        {
                            case ZipType.archive:

                                ArchiveMethod(filePath[i], archive);

                                break;

                            case ZipType.krcDiag:

                                //KrcDiagMethod(filePath[i], archive);
                                MessageBox.Show("KrcDiag no soportado en esta versión, utilizar sólo archivados.");

                                break;
                            default:
                                break;
                        }

                    }

                }

                MessageBox.Show("Informes generados");
                fileList.Items.Clear();

                if (debugMode)
                {
                    //StringManipulation.DeleteDirectory("Mantenimiento");
                    //StringManipulation.DeleteDirectory("Programas");
                }

                zipCounter = 0;

            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            debugMode = true;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            debugMode = false;
        }


        private void KrcDiagMethod(string filePath, ZipArchive archive)
        {
            //throw new NotImplementedException();

            Krc tempKrc = new Krc();
            ZipArchiveEntry configEntry, madaEntry, maintenanceLogEntry, KRCDiagLogEntry;
            string configContent, madaContent, maintenanceLogContent, KRCDiagLogContent;
            string configTempFile, madaTempFile, maintenanceLogTempFile , KRCDiagLogTempFile;


            KRCDiagLogEntry = archive.GetEntry("KRCDiag.log");
            if (KRCDiagLogEntry != null)
            {
                KRCDiagLogTempFile = System.IO.Path.GetTempFileName();
                KRCDiagLogEntry.ExtractToFile(KRCDiagLogTempFile, true);
                KRCDiagLogContent = File.ReadAllText(KRCDiagLogTempFile);

                tempKrc.Name = StringManipulation.GetBetween(KRCDiagLogContent, " Robot Name      ", "\n");
                tempKrc.SerialNo = StringManipulation.GetBetween(KRCDiagLogContent, "$KR_SERIALNO     ", "\n");
                tempKrc.Version = StringManipulation.GetBetween(KRCDiagLogContent, "KRC Version     KR C, ", " ");
                tempKrc.Tech = StringManipulation.GetRealTechData(StringManipulation.GetBetween(KRCDiagLogContent, "[TechPacks]"));
                tempKrc.RobRunTime = StringManipulation.GetBetween(KRCDiagLogContent, "$ROBRUNTIME      ", "\n");
            }


            configEntry = archive.GetEntry("Files/KRC/Roboter/KRC/R1/System/$config.dat");
            if (configEntry != null)
            {
                configTempFile = System.IO.Path.GetTempFileName();
                configEntry.ExtractToFile(configTempFile, true);
                configContent = File.ReadAllText(configTempFile);
                //TODO: Separate lines and only apply those that have any load DATA. If all have -1 then say so.
                tempKrc.LoadData = StringManipulation.GetRealLoadData(StringManipulation.GetBetween(configContent, "LOAD_DATA[16]", "\r\n\r\n"));

            }

            madaEntry = archive.GetEntry("Files/KRC/Roboter/KRC/R1/Mada/$machine.dat");
            if (madaEntry != null)
            {
                madaTempFile = System.IO.Path.GetTempFileName();
                madaEntry.ExtractToFile(madaTempFile, true);
                madaContent = File.ReadAllText(madaTempFile);
                tempKrc.Type = StringManipulation.GetBetween(madaContent, "$TRAFONAME[]=\"#", " ");
            }

            maintenanceLogEntry = archive.GetEntry("RDC/MaintenanceLog.xml");
            if (maintenanceLogEntry != null)
            {
                maintenanceLogTempFile = System.IO.Path.GetTempFileName();
                maintenanceLogEntry.ExtractToFile(maintenanceLogTempFile, true);
                maintenanceLogContent = File.ReadAllText(maintenanceLogTempFile);
            }

            //Trace.WriteLine(tempKrc.SerialNo.TrimEnd('\n'));
            Directory.CreateDirectory(@"Programas\" + tempKrc.SerialNo.TrimEnd('\r', '\n'));
            Directory.CreateDirectory(@"Programas\" + tempKrc.SerialNo.TrimEnd('\r', '\n') + "\\" + tempKrc.SerialNo.TrimEnd('\r', '\n') + " - " + System.DateTime.Now.Year + "-" + System.DateTime.Now.Month + "-" + System.DateTime.Now.Day);

            var programPath = @"Programas\" + tempKrc.SerialNo.TrimEnd('\r', '\n') + "\\" + tempKrc.SerialNo.TrimEnd('\r', '\n') + " - " + System.DateTime.Now.Year + "-" + System.DateTime.Now.Month + "-" + System.DateTime.Now.Day + "\\" + System.IO.Path.GetFileName(filePath);
            if (File.Exists(programPath))
            {
                File.Delete(programPath);
            }

            var templatePath = System.IO.Path.GetFullPath(@"plantillaAutoMaintenance.docx");
            var dirPath = System.IO.Path.GetFullPath(@"Mantenimiento\");
            File.Copy(filePath, @"Programas\" + tempKrc.SerialNo.TrimEnd('\r', '\n') + "\\" + tempKrc.SerialNo.TrimEnd('\r', '\n') + " - " + System.DateTime.Now.Year + "-" + System.DateTime.Now.Month + "-" + System.DateTime.Now.Day + "\\" + System.IO.Path.GetFileName(filePath));
            WordLibs.CreateWordDocument(templatePath, dirPath + tempKrc.SerialNo.TrimEnd('\r', '\n') + " - " + DateTime.Now.Year + "-" + System.DateTime.Now.Month + "-" + System.DateTime.Now.Day + ".docx", tempKrc);

        }

        public void ArchiveMethod(string filePath, ZipArchive archive)
        {

            //throw new NotImplementedException();
            Krc tempKrc = new Krc();
            ZipArchiveEntry amIniEntry, configEntry, madaEntry;
            string amIniContent, configContent, madaContent, amInitempFile, configTempFile, madaTempFile;

            amIniEntry = archive.GetEntry("am.ini");
            if (amIniEntry != null)
            {
                amInitempFile = System.IO.Path.GetTempFileName();
                amIniEntry.ExtractToFile(amInitempFile, true);
                amIniContent = File.ReadAllText(amInitempFile);

                tempKrc.Name = StringManipulation.GetBetween(amIniContent, "RobName=", "IRSerialNr=");
                tempKrc.SerialNo = StringManipulation.GetBetween(amIniContent, "IRSerialNr=", "[Version]");
                tempKrc.Version = StringManipulation.GetBetween(amIniContent, "Version=", "\r\n");
                tempKrc.Tech = StringManipulation.GetRealTechData(StringManipulation.GetBetween(amIniContent, "[TechPacks]"));

            }

            configEntry = archive.GetEntry("KRC/R1/System/$config.dat");
            if (configEntry != null)
            {
                configTempFile = System.IO.Path.GetTempFileName();
                configEntry.ExtractToFile(configTempFile, true);
                configContent = File.ReadAllText(configTempFile);
                //TODO: Separate lines and only apply those that have any load DATA. If all have -1 then say so.
                tempKrc.LoadData = StringManipulation.GetRealLoadData(StringManipulation.GetBetween(configContent, "LOAD_DATA[16]", "\r\n\r\n"));

            }

            madaEntry = archive.GetEntry("KRC/R1/Mada/$machine.dat");
            if (madaEntry != null)
            {
                madaTempFile = System.IO.Path.GetTempFileName();
                madaEntry.ExtractToFile(madaTempFile, true);
                madaContent = File.ReadAllText(madaTempFile);

                tempKrc.Type = StringManipulation.GetBetween(madaContent, "$TRAFONAME[]=\"#", " ");
            }

            tempKrc.AxisData = new string[7];
            Random random = new Random(DateTime.Now.Ticks.GetHashCode());
            for (int i = 1; i < 7; i++)
            {
                tempKrc.AxisData[i] = Math.Round((random.Next(-999, 999) / (Convert.ToDouble(random.Next(1,100)*10000))),5).ToString(); 
            }
           

            //Trace.WriteLine(tempKrc.SerialNo.TrimEnd('\n'));
            Directory.CreateDirectory(@"Programas\" + tempKrc.SerialNo.TrimEnd('\r', '\n'));
            Directory.CreateDirectory(@"Programas\" + tempKrc.SerialNo.TrimEnd('\r', '\n') + "\\" + tempKrc.SerialNo.TrimEnd('\r', '\n') + " - " + System.DateTime.Now.Year + "-" + System.DateTime.Now.Month + "-" + System.DateTime.Now.Day);

            var programPath = @"Programas\" + tempKrc.SerialNo.TrimEnd('\r', '\n') + "\\" + tempKrc.SerialNo.TrimEnd('\r', '\n') + " - " + System.DateTime.Now.Year + "-" + System.DateTime.Now.Month + "-" + System.DateTime.Now.Day + "\\" + System.IO.Path.GetFileName(filePath);
            if (File.Exists(programPath))
            {
                File.Delete(programPath);
            }

            var templatePath = System.IO.Path.GetFullPath(@"plantillaAutoMaintenance.docx");
            var dirPath = System.IO.Path.GetFullPath(@"Mantenimiento\");
            File.Copy(filePath, @"Programas\" + tempKrc.SerialNo.TrimEnd('\r', '\n') + "\\" + tempKrc.SerialNo.TrimEnd('\r', '\n') + " - " + System.DateTime.Now.Year + "-" + System.DateTime.Now.Month + "-" + System.DateTime.Now.Day + "\\" + System.IO.Path.GetFileName(filePath));
            WordLibs.CreateWordDocument(templatePath, dirPath + tempKrc.SerialNo.TrimEnd('\r', '\n') + " - " + DateTime.Now.Year + "-" + System.DateTime.Now.Month + "-" + System.DateTime.Now.Day + ".docx", tempKrc);

        }
    }

}
