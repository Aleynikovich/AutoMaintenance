using Microsoft.Win32;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;

namespace AutoMaintenance
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string[] filePath = new string[100];
        int zipCounter = 0;
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

        private void Import_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "ZIP Files (*.zip)|*.zip";
            

            if (openFileDialog.ShowDialog() == true)
            {
                zipCounter = 0;
                foreach (string item in openFileDialog.FileNames)
                {
                    _ = fileList.Items.Add(System.IO.Path.GetFileName(item));
                    filePath[zipCounter] = System.IO.Path.GetFullPath(item);
                    //Trace.WriteLine("added " + filePath[zipCounter]);
                    zipCounter++;
                }
            }

        }

        private void Clear_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            fileList.Items.Clear();
            zipCounter = 0;
        }

        private void Start_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            string krcName, krcSerialNo, krcVersion, krcTech;


            if (zipCounter > 0)
            {
                for (int i = 0; i < zipCounter; i++)
                {
                    //Trace.WriteLine(filePath[i]);
                    using (ZipArchive archive = ZipFile.OpenRead(filePath[i]))
                    {
                        ZipArchiveEntry entry = archive.GetEntry("am.ini");
                        if (entry != null)
                        {
                            //Trace.WriteLine("Data found");
                            string tempFile = Path.GetTempFileName();
                            entry.ExtractToFile(tempFile, true);
                            string content = File.ReadAllText(tempFile);
                            //Trace.Write(content);
                            krcName = content.Substring(content.LastIndexOf("IRSerialNr=") + "IRSerialNr=".Length, content.IndexOf("\n[Version]") - (content.LastIndexOf("IRSerialNr=") + "IRSerialNr=".Length));
                            Trace.WriteLine(krcName.TrimEnd('\n'));
                            File.Copy("plantillainforme.docx", krcName.TrimEnd('\r', '\n') + " informe de mantenimiento.docx");
                        }

                    }
                }
            }
           
        }




    }
}
