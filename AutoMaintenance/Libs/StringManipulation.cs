using System;

namespace AutoMaintenance.Libs
{
    public class StringManipulation
    {
        public static string GetBetween(string strSource, string strStart, string strEnd = "def")
        {
            if (strSource.Contains(strStart) && (strSource.Contains(strEnd) || strEnd == "def"))
            {
                var Start = strSource.IndexOf(strStart, 0, StringComparison.Ordinal) + strStart.Length;
                var End = strSource.IndexOf(strEnd, Start, StringComparison.Ordinal);

                return strEnd == "def" ? strSource.Substring(Start) : strSource.Substring(Start, End - Start);
            }
            else
            {
                return "Limit strings not found";
            }

        }
    }
}   
