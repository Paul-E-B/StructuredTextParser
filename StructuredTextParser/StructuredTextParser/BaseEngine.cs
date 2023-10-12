using System.IO;
using System;

namespace StructuredTextParser
{
    /// <summary>
    /// An abstract parsing engine used as a basis for all other parsing engines in the program
    /// </summary>
    internal abstract class BaseEngine
    {
        //current line in a piece of parsed data
        string? currentLineInFile;

        //holds data after it's been split at its delimiter
        static string[]? splitData;

        //holds data as it is formatted
        static string? parsedData;

        //track the current line in a set of data
        int lineCounter = 0;
        
        //track the current segment of data being parsed in a delimited file
        int dataCounter = 0;


        //Holds the location of the last '.' in the current file name.
        //This allows for the original extension to be clipped off and
        //replaced with "_out.txt"
        int indexOfExtension = 0;

        //Resulting name for the output file
        string? outputFileName;

        /// <summary>
        /// An empty method overriden by the XML and JSON engines.
        /// </summary>
        /// <param name="file"></param>The current file info being parsed
        /// <param name="outputPath"></param>The directory to output parsed data to
        public virtual void Process(IFile file, string outputPath)
        {


        }

        /// <summary>
        /// A method used to parse all delimited text
        /// This is used by the CSVParseEngine and PipeParseEngine classes
        /// </summary>
        /// <param name="file"></param>the current file being parsed
        /// <param name="outputPath"></param>the path to output the file to
        /// <param name="delimiter"></param>the appropriate delimiter for passed file extension
        public virtual void Process(IFile file, string outputPath, char delimiter)
        {
            try
            {
                //File stream used to read xml file
                using (StreamReader inputFileReader = new StreamReader(file.Path))
                {

                    using (FileStream outputFile = new FileStream(GenerateOutputFileName(outputPath, file.Name), FileMode.OpenOrCreate))
                    {
                        using (StreamWriter outputWriter = new StreamWriter(outputFile))
                        {
                            lineCounter = 1;
                            dataCounter = 1;

                            while ((currentLineInFile = inputFileReader.ReadLine()) != null)
                            {
                                splitData = null;
                                parsedData = null;

                                splitData = currentLineInFile.Split(delimiter);

                                foreach (string data in splitData)
                                {
                                    parsedData += $"Field #{dataCounter}={data}==> ";
                                    dataCounter++;
                                }
                                dataCounter = 1;

                                parsedData = parsedData.Substring(0, parsedData.Length - 4);

                                outputWriter.WriteLine($"Line#{lineCounter} :{parsedData}");
                                outputWriter.WriteLine();

                                lineCounter++;
                            }
                            outputWriter.Close();
                        }
                        outputFile.Close();
                    }
                    inputFileReader.Close();
                }
            }
            catch(Exception err)
            {
                ErrorLog.LogError(err.ToString(), outputPath);
            }
        }

        /// <summary>
        /// A method used by every engine to generate the
        /// name of the output file
        /// </summary>
        /// <param name="outputPath"></param>The path to output the data
        /// <param name="fileName"></param>The current name of the file
        /// <returns></returns>
        public string GenerateOutputFileName(string outputPath, string fileName)
        {
            indexOfExtension = fileName.LastIndexOf('.');

            outputFileName = Path.Combine(outputPath, $"{fileName.Substring(0, indexOfExtension)}_out.txt");

            return outputFileName;
        }
    }
}
