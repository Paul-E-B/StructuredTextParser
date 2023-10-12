using System;
using System.IO;
using System.Xml.Serialization;

namespace StructuredTextParser
{
    internal class XMLParseEngine : BaseEngine
    {
        int lineCounter = 0;
        
        string? data;

        /// <summary>
        /// Parse data in a file ending in .xml
        /// </summary>
        /// <param name="file"></param>Current file being parsed
        /// <param name="outputPath"></param>Path the data is being output to
        public override void Process(IFile file, string outputPath)
        {
            try
            {
                //File stream used to read xml file
                using (FileStream inputFileReader = File.Open(file.Path, FileMode.Open))
                {
                    //Tell the program that each node is of type Grocery
                    XmlSerializer serializer = new XmlSerializer(typeof(Grocery));

                    //Creates an instance of the grocery class, which is a list of grocery info, based on the xml file
                    var inventory = (Grocery)serializer.Deserialize(inputFileReader);

                    using (FileStream outputFile = new FileStream(GenerateOutputFileName(outputPath, file.Name), FileMode.OpenOrCreate))
                    {
                        using (StreamWriter outputWriter = new StreamWriter(outputFile))
                        {
                            lineCounter = 1;
                            data = null;

                            foreach (var item in inventory.Item)
                            {
                                data = $"{item.Name} {item.Price}/{item.Uom}";

                                outputWriter.WriteLine($"Line#{lineCounter} : Item Info => {data}");
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
    }



}

