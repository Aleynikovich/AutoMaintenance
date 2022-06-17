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

            if (zipCounter > 0)
            {               
                 for (int i = 0; i < zipCounter; i++)
                {
                    using (ZipArchive archive = ZipFile.OpenRead(filePath[i])) //Open .zip in read mode
                    {
                        ZipArchiveEntry entry = archive.GetEntry("am.ini");
                        if (entry != null)
                        {

                            string tempFile = Path.GetTempFileName();
                            entry.ExtractToFile(tempFile, true);
                            string content = File.ReadAllText(tempFile);

                            Krc tempKrc = new Krc
                            {
                                Name = Libs.StringManipulation.GetBetween(content, "RobName=", "IRSerialNr="),
                                SerialNo = Libs.StringManipulation.GetBetween(content, "IRSerialNr=", "[Version]"),
                                Version = Libs.StringManipulation.GetBetween(content, "[Version]", "[TechPacks]"),
                                Tech = Libs.StringManipulation.GetBetween(content, "[TechPacks]", "default"),
                                Type = "NoType"
                            };

                            //Trace.WriteLine(tempKrc.SerialNo.TrimEnd('\n'));
                            WordLibs.CreateWordDocument(@"C:\Users\XYZ\source\repos\Aleynikovich\AutoMaintenance\AutoMaintenance\Assets\plantillaAutoMaintenance.docx",
                                @"C:\Users\XYZ\source\repos\Aleynikovich\AutoMaintenance\AutoMaintenance\Assets\" + tempKrc.SerialNo.TrimEnd('\r', '\n') + " Informe de mantenimiento.docx", tempKrc);
                            
                        }

                    }
                }

                MessageBox.Show("Informes generados");
                fileList.Items.Clear();
                zipCounter = 0;
            }

        }

    }

}
