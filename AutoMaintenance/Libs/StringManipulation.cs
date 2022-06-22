using System;
using System.IO;
using System.IO.Compression;


namespace AutoMaintenance
{
    public class StringManipulation
    {
        /// <summary>
        /// Allows to search for two strings in a text file and get the string in between them
        /// </summary>
        /// <param name="strSource">String to search in</param>
        /// <param name="strStart">Unique string that marks the start of the search</param>
        /// <param name="strEnd">Unique string that marks the end of the search</param>
        /// <returns>String between the selected start and end string</returns>
        public static string GetBetween(string strSource, string strStart, string strEnd = "default")
        {
            if (strSource.Contains(strStart) && (strSource.Contains(strEnd) || strEnd == "default"))
            {
                var Start = strSource.IndexOf(strStart, 0, StringComparison.Ordinal) + strStart.Length;
                var End = strSource.IndexOf(strEnd, Start, StringComparison.Ordinal);

                return strEnd == "default" ? strSource.Substring(Start) : strSource.Substring(Start, End - Start);
            }
            else
            {
                return "Limit strings not found";
            }

        }

        public static void DeleteDirectory(string target_dir)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(target_dir, false);
        }

        public static string[] GetRealLoadData(string rawLoadData)
        {
            string[] rawInLines = rawLoadData.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            string[] realLoadData = new string[4];
            int i = 0;

            foreach (string item in rawInLines)
            {
                if (i > 3)
                {
                    return realLoadData;
                }

                if (GetBetween(item, "{M ", ".") != "-1" && IsUnique(realLoadData, item, i))
                {
                    realLoadData[i] = item;
                    i++;
                }
            }

            return realLoadData;

        }

        public static string GetRealTechData(string rawTechData)
        {
            string[] rawInLines = rawTechData.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            string realTechData = null;
            int i = 0;

            foreach (string item in rawInLines)
            {
                if (i != 0)
                {
                    realTechData += item + " | ";        
                }
                i++;
            }

            return realTechData;

        }

        public static string[] SplitTechData(string rawTechData)
        {
            string[] rawInLines = rawTechData.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            string[] splitTechData = new string[10];
            int i = 0;

            foreach (string item in rawInLines)
            {
                splitTechData[i] = item;
                i++;
            }

            return splitTechData;

        }

        public static bool IsUnique(string[] lines, string currentLine, int currentPosition)
        {
          
            for (int i = 0; i <= currentPosition; i++)
            {
                if (i == currentPosition)
                {
                    return true;
                }

                if (GetBetween(lines[i], "]=", "}}") == GetBetween(currentLine, "]=", "}}"))
                {
                    return false;
                }
            }

            return true;

        }

    }
}
