using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace StructuredTextParser
{
    internal class ParseDelimitedFile
    {
        static string? currentLineInParsedData;

        static List<string> parsedDataList = new List<string>();


        static string[]? splitData;
        public static string? parsedData;
        static int dataIncrement;
        static int lineIncrement;


        public static void ParseDelimterLine(string incomingData, char delimiter) 
        {
            splitData = null;
            parsedData = null;
            


            dataIncrement = 1;

            try
            { 
                splitData = incomingData.Split(delimiter);
            
            
                foreach(string data in splitData)
                {
                    parsedData += "Field#" + dataIncrement + "=" + data + "==> ";
                    dataIncrement++;
                }
                parsedData = parsedData.Substring(0,parsedData.Length-4);
            }
            catch(Exception parseError)
            {
                ErrorLog.LogError(parseError.ToString(), incomingData);
            }

        }
    }
}

















        


