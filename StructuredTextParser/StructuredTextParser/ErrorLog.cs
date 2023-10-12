using System.Collections.Generic;
using System.IO;


namespace StructuredTextParser
{
    //A class for anything relating to Errors
    internal class ErrorLog
    {
        static List<string> errorLog = new List<string>();


        /// <summary>
        /// Update the error log with a specified error in a specific location
        /// </summary>
        /// <param name="error"></param>The error as a string value
        /// <param name="location"></param>The file or script name where the issue occured
        public static void LogError(string error, string location)
        {
            errorLog.Add(error + " in " + location);
        }


        //a method to get the error log from anywhere else in the program
        public static List<string> ReturnErrorLog()
        {
            return errorLog;
        }

        /// <summary>
        /// A method used for exporting a specific error to the output directory.
        /// Used for testing purposes
        /// </summary>
        /// <param name="outputPath"></param>Directory to output the data to
        /// <param name="data"></param>The error to be output
        public static void ExportError(string outputPath, string data)
        {
            using (FileStream outputFile = new FileStream(Path.Combine(outputPath, "error.txt"), FileMode.OpenOrCreate))
            {
                using (StreamWriter outputWriter = new StreamWriter(outputFile))
                {
                    outputWriter.WriteLine(data);

                    outputWriter.Close();
                }
                outputFile.Close();
            }
        }
    }
}
