using System;

namespace AutoMaintenance.Libs
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
    }
}   
