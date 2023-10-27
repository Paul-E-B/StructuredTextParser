﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;




namespace StructuredTextParser
{
    internal class Importer
    {
        //the extension of the file currently being parsed
        public static string? fileExtension;

        //a list of all files in the directory
        static List<string> all_FilesInDirectory = new List<string>();

        //this is used to hold an instance of a parsing engine derived from the base engine
        static BaseEngine? engine;

        //this is the info for the current file being parsed
        static FileInfo? currentFile;


        /// <summary>
        /// Stream data from desired delimited file and output it as a formatted txt file
        /// </summary>
        /// <param name="inputPaths"></param>This is the path to the folder the data is read-in from
        /// <param name="outputPath"></param>This is the path to the folder that the txt files will be output to
        public static void StreamDataToTxt(string inputPath, string outputPath)
        {
            //Get a list of all paths to all files in the directory
            all_FilesInDirectory = Directory.GetFiles(inputPath).ToList();

            //iterates through each file in the input directory
            foreach (string currentFilePath in all_FilesInDirectory)
            {
                //checks if the file extension of the current file is valid
                FilterFileByExtension(currentFilePath, MainWindow.validFileExtensions);
                
                try
                { 
                    switch(fileExtension)
                    {
                        //Parse Pipe files
                        case ".txt":
                            currentFile = new FileInfo(currentFilePath,fileExtension, Path.GetFileName(currentFilePath),'|', "ToTxt");
                            engine = new PipeParseEngine();
                            engine.Process(currentFile, outputPath);
                            break;

                        //Parse CSV files
                        case ".csv":
                            currentFile = new FileInfo(currentFilePath, fileExtension, Path.GetFileName(currentFilePath),',', "ToTxt");
                            engine = new CSVParseEngine();
                            engine.Process(currentFile, outputPath);
                            break;

                        //Parse XML files
                        case ".xml":
                            currentFile = new FileInfo(currentFilePath, fileExtension, Path.GetFileName(currentFilePath));
                            engine = new XMLParseEngine();
                            engine.Process(currentFile, outputPath);
                            break;

                        //Parse JSON files
                        case ".json":
                            currentFile = new FileInfo(currentFilePath, fileExtension, Path.GetFileName(currentFilePath));
                            engine = new JSONParseEngine();
                            engine.Process(currentFile, outputPath);
                            break;

                        default:
                            break;
                    }
                }
                catch
                {
                    ErrorLog.LogError("Parse error", currentFilePath);
                }
            }   
        }



        /// <summary>
        /// Stream data from desired file, input it into a SQL table, then output it as a formatted txt file
        /// </summary>
        /// <param name="inputPaths"></param>This is the path to the folder the data is read-in from
        /// <param name="outputPath"></param>This is the path to the folder that the txt files will be output to
        public static void StreamDataToSql(string inputPath, string outputPath)
        {
            //Get a list of all paths to all files in the directory
            all_FilesInDirectory = Directory.GetFiles(inputPath).ToList();

            //iterates through each file in the input directory
            foreach (string currentFilePath in all_FilesInDirectory)
            {
                //checks if the file extension of the current file is valid
                FilterFileByExtension(currentFilePath, MainWindow.validSqlFileExtensions);

                try
                {
                    switch(fileExtension)
                    {
                        //Parse produce data to sql
                        case ".txt":
                            currentFile = new FileInfo(currentFilePath, fileExtension, Path.GetFileName(currentFilePath), '|', "Produce", "ToSql");
                            engine = new PipeParseEngine();
                            engine.Process(currentFile, outputPath);
                            break;

                        //Parse character data to sql
                        case ".csv":
                            currentFile = new FileInfo(currentFilePath, fileExtension, Path.GetFileName(currentFilePath), ',', "Character", "ToSql");
                            engine = new CSVParseEngine();
                            engine.Process(currentFile, outputPath);
                            break;


                        default:
                            break;
                    }
                }
                catch(Exception e)
                {
                    ErrorLog.LogError(e.ToString(),"StreamDataToSql");
                }

            }
        }



        /// <summary>
        /// This filters a file by its extension. If it has a valid extension, the file is valid and its extension is logged
        /// </summary>
        /// <param name="path"></param>this is the path to the current file.
        static void FilterFileByExtension(string path, List<string> validExtensions)
        {
            fileExtension = null;
            
            try
            {
                //Check the array of valid file extensions agains the end of the current file extensions
                //If a match is found, the extension is logged for use in the StreamData() method above
                validExtensions.Any(ext => path.EndsWith(fileExtension = ext));
            }
            catch
            {
                ErrorLog.LogError("Invalid extension", path);
            }
        }
    }
}


    

