using System.Collections.Generic;


namespace CSV_Pipe_To_TabDelimited
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



    }
}
