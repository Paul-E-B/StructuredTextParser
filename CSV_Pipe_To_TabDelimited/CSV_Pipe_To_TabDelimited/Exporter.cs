using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CSV_Pipe_To_TabDelimited
{
    //a class for exporter related methods
    internal class Exporter
    {

        /// <summary>
        /// This takes a streamed line of data and writes it to a file
        /// </summary>
        /// <param name="lineCounter"></param>the current line of data being written to the file
        /// <param name="data"></param>the line of data being written to the file
        /// <param name="writer"></param>the stream writer used to write data to the file
        public static void ExportStream(int lineCounter, string data, StreamWriter writer)
        {
            
            try
            {
                writer.WriteLine("Line#"+lineCounter+" :" + data);
                writer.WriteLine();
                                        
            }
            catch(Exception exportError)
            {
                ErrorLog.LogError(exportError.ToString(), data);
            }
        }
    }
}
