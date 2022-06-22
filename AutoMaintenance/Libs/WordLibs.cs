using System.Windows;
using System.IO;
using Word = Microsoft.Office.Interop.Word;
using System.Reflection;

namespace AutoMaintenance
{
    public partial class MainWindow
    {
        class WordLibs
        {
            public static void FindAndReplace(Word.Application wordApp, object ToFindText, object replaceWithText)
            {
                object matchCase = true;
                object matchWholeWord = true;
                object matchWildCards = false;
                object matchSoundLike = false;
                object nmatchAllforms = false;
                object forward = true;
                object format = false;
                object matchKashida = false;
                object matchDiactitics = false;
                object matchAlefHamza = false;
                object matchControl = false;
                object read_only = false;
                object visible = true;
                object replace = 2;
                object wrap = 1;

                wordApp.Selection.Find.Execute(ref ToFindText,
                    ref matchCase, ref matchWholeWord,
                    ref matchWildCards, ref matchSoundLike,
                    ref nmatchAllforms, ref forward,
                    ref wrap, ref format, ref replaceWithText,
                    ref replace, ref matchKashida,
                    ref matchDiactitics, ref matchAlefHamza,
                    ref matchControl);
            }


            public static void CreateWordDocument(object filename, object SaveAs, Krc krc)
            {
                Word.Application wordApp = new Word.Application();
                object missing = Missing.Value;
                Word.Document myWordDoc = null;

                if (File.Exists((string)filename))
                {
                    object readOnly = false;
                    object isVisible = false;
                    wordApp.Visible = false;

                    myWordDoc = wordApp.Documents.Open(ref filename, ref missing, ref readOnly,
                                            ref missing, ref missing, ref missing,
                                            ref missing, ref missing, ref missing,
                                            ref missing, ref missing, ref missing,
                                            ref missing, ref missing, ref missing, ref missing);
                    myWordDoc.Activate();

                    //find and replace
                    FindAndReplace(wordApp, "<KrSerialNo>", krc.SerialNo);
                    FindAndReplace(wordApp, "<KrType>", krc.Type);
                    FindAndReplace(wordApp, "<KrName>", krc.Name);
                    FindAndReplace(wordApp, "<KRCVersion>", krc.Version);
                    FindAndReplace(wordApp, "<Tech1>", krc.Tech);
                    //FindAndReplace(wordApp, "<Tech2>", krc.Tech[1]);
                    //FindAndReplace(wordApp, "<Tech3>", krc.Tech[2]);
                    //FindAndReplace(wordApp, "<Tech4>", krc.Tech[3]);
                    //FindAndReplace(wordApp, "<Tech5>", krc.Tech[4]);
                    FindAndReplace(wordApp, "<LoadData1>", krc.LoadData[0]);
                    FindAndReplace(wordApp, "<LoadData2>", krc.LoadData[1]);
                    FindAndReplace(wordApp, "<LoadData3>", krc.LoadData[2]);
                    FindAndReplace(wordApp, "<LoadData4>", krc.LoadData[3]);
                    //FindAndReplace(wordApp, "<LoadData5>", krc.LoadData[4]);
                    FindAndReplace(wordApp, "<MaintenanceDate>", System.DateTime.Now.Date);
                }

                //Save as
                myWordDoc.SaveAs2(ref SaveAs, ref missing, ref missing, ref missing,
                                ref missing, ref missing, ref missing,
                                ref missing, ref missing, ref missing,
                                ref missing, ref missing, ref missing,
                                ref missing, ref missing, ref missing);

                myWordDoc.Close();
                wordApp.Quit();
            }
        }

    }

}
