using System.IO;
using System;
using System.Data.SqlClient;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Linq;

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

        /// <param name="delimiter"></param>the appropriate delimiter for passed file extension
        char delimiter;

        IFile? current_File;
        string? output_Path;



        

        //used for various tracking tasks
        int counter = 0;

        //holds the SQL commands to be executed
        string inlineSQL = " ";




        /// <summary>
        /// A method used to parse all delimited text
        /// This is used by the CSVParseEngine and PipeParseEngine classes
        /// </summary>
        /// <param name="file"></param>the current file being parsed
        /// <param name="outputPath"></param>the path to output the file to
        public virtual void Process(IFile file, string outputPath)
        {
            current_File = file;
            output_Path = outputPath;

            switch (current_File.ParseType)
            {
                case "ToTxt":
                    ProcessFileData_ToTxt();
                    break;


                case "ToSql":
                    ProcessTxtFile_ToSqlTable();
                    break;

                default:
                    break;
            }
        }




        void ProcessFileData_ToTxt()
        {
            delimiter = current_File.Delimiter;

            try
            {
                using (StreamReader inputFileReader = new StreamReader(current_File.Path))
                {
                    using (FileStream outputFile = new FileStream(GenerateOutputFileName(output_Path, current_File.Name), FileMode.OpenOrCreate))
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
            catch (Exception err)
            {
                ErrorLog.LogError(err.ToString(), output_Path);
            }
        }




        SqlConnection? connection;

        static SqlTableInfo? currentTable;

        List<SqlTableInfo> tables = new List<SqlTableInfo>();


        /// <summary>
        /// Process incoming data from a delimited txt file and write it to an SQL table
        /// </summary>
        void ProcessTxtFile_ToSqlTable()
        {


            using (StreamReader inputFileReader = new StreamReader(current_File.Path))
            {

                currentTable = new SqlTableInfo(@"(localdb)\MSSQLLocalDB", true, "SSPI", "PROG260FA23");

                using (connection = new SqlConnection(SqlUtility.GenerateSqlConnection(currentTable)))
                {
                    connection.Open();

                    delimiter = current_File.Delimiter;

                    
                    switch (current_File.SqlTemplate)
                    {
                        case "Produce":
                            currentTable.Table = "[dbo].[Produce]";
                            currentTable.DataNames = inputFileReader.ReadLine().Split(',');
                            currentTable.DataTypes = new string[] { "nvarchar(50)", "nvarchar(50)", "decimal(6,2)", "nvarchar(10)", "nvarchar(10)"};
                            currentTable.IsDataNull = new string[] { "NOT NULL","NOT NULL","NOT NULL","NOT NULL","NOT NULL"};

                            //Create SQL Table from data
                            SqlUtility.ClearTableFromSQL(currentTable, connection);
                            SqlUtility.CreateSQLTable(currentTable, connection);
                            SqlUtility.InsertTxtDataIntoSQLTable(currentTable, inputFileReader, delimiter, connection);

                            //Update produce info
                            SqlUtility.Execute_SqlCommand($@"UPDATE [{currentTable.InitialCatalog}].{currentTable.Table} SET Location = REPLACE(Location, 'F','Z')", connection);
                            SqlUtility.Execute_SqlCommand($@"DELETE FROM [{currentTable.InitialCatalog}].{currentTable.Table} WHERE Sell_by_Date < GETDATE()", connection);
                            SqlUtility.Execute_SqlCommand($@"UPDATE [{currentTable.InitialCatalog}].{currentTable.Table} SET Price=Price+1", connection);
                            ExportProduceSQL_ToDelimited();
                            break;


                        case "Character":
                            //Character(Name) 0    Type 1    Map_Location 2    Original_character 3    Sword_Fighter 4    Magic_User 5
                            inputFileReader.ReadLine();
                            tables.Clear();

                            //Character    Type
                            SqlTableInfo characterTable = new SqlTableInfo("[dbo].[CharacterInfo]");
                            characterTable.DataNames = new string[] { "Character", "Type"};
                            characterTable.DataTypes = new string[] { "nvarchar(50)", "nvarchar(50)"};
                            characterTable.IsDataNull = new string[] { "NOT NULL", "NULL"};
                            tables.Add(characterTable);
                            SqlUtility.ClearTableFromSQL(characterTable, connection);
                            SqlUtility.CreateSQLTable(tables[0], connection);

                            //CharacterID    Map_Location
                            SqlTableInfo characterLocationTable = new SqlTableInfo("[dbo].[CharacterLocation]");
                            characterLocationTable.DataNames = new string[] { "CharacterID", "Map_Location" };
                            characterLocationTable.DataTypes = new string[] { "int", "nvarchar(50)"};
                            characterLocationTable.IsDataNull = new string[] { "NOT NULL", "NULL" };
                            tables.Add(characterLocationTable);
                            SqlUtility.ClearTableFromSQL(characterLocationTable, connection);
                            SqlUtility.CreateSQLTable(tables[1], connection);

                            //CharacterID    Sword_Fighter    Magic_User
                            SqlTableInfo characterCombatInfoTable = new SqlTableInfo("[dbo].[CharacterCombatInfo]");
                            characterCombatInfoTable.DataNames = new string[] { "CharacterID", "Sword_Fighter", "Magic_User"};
                            characterCombatInfoTable.DataTypes = new string[] { "int","nvarchar(5)", "nvarchar(5)" };
                            characterCombatInfoTable.IsDataNull = new string[] { "NOT NULL", "NULL", "NULL" };
                            tables.Add(characterCombatInfoTable);
                            SqlUtility.ClearTableFromSQL(characterCombatInfoTable, connection);
                            SqlUtility.CreateSQLTable(tables[2], connection);

                            //CharacterID    Original_Character
                            SqlTableInfo characterOriginalityTable = new SqlTableInfo("[dbo].[CharacterOriginality]");
                            characterOriginalityTable.DataNames = new string[] { "CharacterID", "Original_character"};
                            characterOriginalityTable.DataTypes = new string[] { "int", "nvarchar(5)" };
                            characterOriginalityTable.IsDataNull = new string[] { "NOT NULL", "NULL"};
                            tables.Add(characterOriginalityTable);
                            SqlUtility.ClearTableFromSQL(characterOriginalityTable, connection);
                            SqlUtility.CreateSQLTable(tables[3], connection);

                            SqlUtility.InsertDataIntoCharacterSQLTables(tables, inputFileReader, delimiter, connection);
                            SQLQuery_Export_ToTxt("Full Report", SqlUtility.FullCharacterReport());
                            SQLQuery_Export_ToTxt("Lost", SqlUtility.LostCharacter());
                            SQLQuery_Export_ToTxt("SwordNonHuman", SqlUtility.NonHuman_SwordUsers());
                            break;

                        default:
                            break;
                    }

                    //ExportSql_ToDelimited();

                    connection.Close();
                }
                inputFileReader.Close();
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



        /// <summary>
        /// A method used for exporting produce data from an SQL table into a pipe delimited file
        /// </summary>
        void ExportProduceSQL_ToDelimited()
        {
            using (FileStream outputFile = new FileStream(GenerateOutputFileName(output_Path, current_File.Name), FileMode.OpenOrCreate))
            {
                using (StreamWriter outputWriter = new StreamWriter(outputFile))
                {
                    inlineSQL = @"Select * from Produce";

                    using (var command = new SqlCommand(inlineSQL, connection))
                    {
                        parsedData = "Name,Location,Price,UoM,Sell_by_Date";
                        outputWriter.WriteLine(parsedData);

                        var reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            counter = 1;

                            string name = reader.GetValue(counter++).ToString();
                            string location = reader.GetValue(counter++).ToString();
                            string price = reader.GetValue(counter++).ToString();
                            string uom = reader.GetValue(counter++).ToString();
                            string sellDate = reader.GetDateTime(counter).ToShortDateString();
                            string parsedData = $"{name}|{location}|{price}|{uom}|{sellDate}";

                            outputWriter.WriteLine(parsedData);
                        }
                        reader.Close();
                    }
                    outputWriter.Close();
                }
                outputFile.Close();
            }
        }


        bool first = true;

        /// <summary>
        /// This is used to export data from an SQL query of a table and export it as a txt file
        /// </summary>
        /// <param name="fileName"></param>The desired name of the output txt file
        /// <param name="inlineSqlCommand"></param>The query being executed
        void SQLQuery_Export_ToTxt(string fileName, string inlineSqlCommand)
        {
            fileName = output_Path + $"\\{fileName}.txt";

            using (FileStream outputFile = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                using (StreamWriter outputWriter = new StreamWriter(outputFile))
                {
                    using(var command = new SqlCommand(inlineSqlCommand, connection))
                    {
                        var reader = command.ExecuteReader();

                        first = true;
                        //output the names of each column data type
                        for(counter = 0; counter < reader.FieldCount; counter++)
                        {
                            if(!first)
                            {
                                outputWriter.Write(",");
                            }
                            else
                            {
                                first = false;
                            }
                            outputWriter.Write($"{reader.GetName(counter)}");
                        }
                        outputWriter.WriteLine();


                        first = true;
                        //output the rows of data
                        while(reader.Read())
                        {    
                            if(!first)
                            {
                                outputWriter.WriteLine();
                            }
                            else
                            {
                                first = false;
                            }

                            first = true;

                            for (counter = 0; counter < reader.FieldCount; counter++)
                            {
                                if (!first)
                                {
                                    outputWriter.Write(",");
                                }
                                else
                                {
                                    first = false;
                                }
                                outputWriter.Write($"{reader[counter]}");
                            }
                        }
                        reader.Close();
                    }
                    outputWriter.Close();
                }
                outputFile.Close();
            }

        }




    }
}






