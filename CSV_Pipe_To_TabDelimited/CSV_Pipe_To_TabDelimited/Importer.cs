using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSV_Pipe_To_TabDelimited
{
    internal class Importer
    {

        public static string? fileExtension;

        public static char delimiter;

        static string? currentLineInFile;

        static int lineCounter;

        static string? fileName;

        /// <summary>
        /// Stream data from desired delimited file and output it as a formatted txt file
        /// </summary>
        /// <param name="inputPaths"></param>This is the path to the folder the data is read-in from
        /// <param name="outputPath"></param>This is the path to the folder that the txt files will be output to
        public static void StreamData(List<string> inputPaths, string outputPath)
        {
            ClearOutputDirectory(outputPath);

            foreach (string currentFilePath in inputPaths)
            {
                //checks if the file extension is valid
                FilterFileByExtension(currentFilePath);

                if(fileExtension != null)
                { 
                    fileName = Path.GetFileName(currentFilePath);

                    delimiter = MainWindow.extensions_And_Delimiter_To_Parse[fileExtension];


                    using (StreamReader reader = new StreamReader(currentFilePath))
                    {
                        using (FileStream fileStream = new FileStream(Path.Combine(outputPath,fileName), FileMode.OpenOrCreate))
                        {
                            using(StreamWriter writer = new StreamWriter(fileStream))
                            { 
                                lineCounter = 1;

                                //Read the script until null
                                while ((currentLineInFile = reader.ReadLine()) != null)
                                {
                                    
                                    //parse the line of data
                                    ParseDelimitedFile.ParseDelimterLine(currentLineInFile, delimiter);

                                    //Export that line of data
                                    Exporter.ExportStream(lineCounter, ParseDelimitedFile.parsedData, writer);

                                    lineCounter++;
                                }
                                writer.Close();
                            }
                            fileStream.Close();
                        }
                        reader.Close();
                    }
                }

            }   

        } 


        /// <summary>
        /// This filters a filed by its extension. If it has a valid extension, as defined in the MainWindow, the file
        /// is valid and its extension is logged
        /// </summary>
        /// <param name="path"></param>this is the directory that the file will be written to.
        static void FilterFileByExtension(string path)
        {
            if (MainWindow.validFileExtensions.Any(ext => path.EndsWith(fileExtension = ext)))
            {

            }
            else
            {
                fileExtension = null;
                ErrorLog.LogError("Invalid extension", path);
            }   
        }



        /// <summary>
        /// Clear the directory that the parsed files will be exported to
        /// </summary>
        /// <param name="outputPath"></param>this is the directory that will be cleared
        static void ClearOutputDirectory(string outputPath)
        {
            var dir = Directory.GetFiles(outputPath);
            foreach(string file in dir)
            {
                File.Delete(file);
            }
        }


    }

}


    

