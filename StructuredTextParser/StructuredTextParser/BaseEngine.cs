using System.IO;
using System;
using static System.Net.WebRequestMethods;
using System.Data.SqlClient;
using System.Printing;
using System.Diagnostics;
using System.Xml.Linq;
using System.Windows.Markup;

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



        string initialCatalog = "PROG260FA23";
        string table = "[dbo].[Produce]";
        

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
                    ProcessFileData_ToSql();
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
                //File stream used to read xml file
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

        string? sqlConnectionString;


        void ProcessFileData_ToSql()
        {
            using (StreamReader inputFileReader = new StreamReader(current_File.Path))
            {
                
                sqlConnectionString = GenerateSqlConnection();

                using (connection = new SqlConnection(sqlConnectionString))
                {
                    connection.Open();

                    delimiter = current_File.Delimiter;

                    ClearCurrentTable_SqlCommand();

                    inputFileReader.ReadLine();

                    //skips the line that gives data info to the for the table
                    CreateTable_SqlCommand();


                    while ((currentLineInFile = inputFileReader.ReadLine()) != null)
                    {
                        splitData = null;

                        splitData = currentLineInFile.Split(delimiter);

                        InsertData_SqlCommand(splitData);                        
                    }


                    ReplaceData_SqlCommand();
                    DeleteData_SqlCommand();
                    IncreaseNumbericData_SqlCommand();



                    ExportSql_ToDelimited();

                    connection.Close();
                }
                inputFileReader.Close();
            }
        }

        void ExportSql_ToDelimited()
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

                            name = reader.GetValue(counter++).ToString();
                            location = reader.GetValue(counter++).ToString();
                            price = reader.GetValue(counter++).ToString();
                            uom = reader.GetValue(counter++).ToString().Trim();
                            splitSellDate = (reader.GetDateTime(counter).ToShortDateString()).Split('/');
                            //set the sell date to MM-DD-YYYY
                            sellDate = $"{splitSellDate[0]}-{splitSellDate[1]}-{splitSellDate[2]}";

                            parsedData = $"{name}|{location}|{price}|{uom}|{sellDate}";

                            outputWriter.WriteLine(parsedData);
                        }
                        reader.Close();
                    }
                    outputWriter.Close();
                }
                outputFile.Close();
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
        /// General use method for executing a SQL command
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commandStatement"></param>
        void Execute_SqlCommand(string commandStatement)
        {
            try
            {
                using (var command = new SqlCommand(commandStatement, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
            catch(Exception err)
            {
                ErrorLog.LogError(err.ToString(), "Execute_SqlCommand");
            }
        }


        void ReplaceData_SqlCommand()
        {
            //https://www.w3schools.com/sql/func_sqlserver_replace.asp
            inlineSQL = $@"UPDATE [{initialCatalog}].{table} SET Location = REPLACE(Location, 'F','Z')";
            Execute_SqlCommand(inlineSQL);
        }

        void DeleteData_SqlCommand()
        {
            //https://www.freecodecamp.org/news/how-to-delete-a-row-in-sql-example-query/
            //https://www.w3schools.com/sql/func_sqlserver_getdate.asp
            inlineSQL = $@"DELETE FROM [{initialCatalog}].{table} WHERE Sell_by_Date < GETDATE()";
            Execute_SqlCommand(inlineSQL);
        }



        void IncreaseNumbericData_SqlCommand()
        {
            //https://www.dbvis.com/thetable/mysql-incremental-value-update/
            inlineSQL = $@"UPDATE [{initialCatalog}].{table} SET Price=Price+1";
            Execute_SqlCommand(inlineSQL);
        }



        /// <summary>
        /// Deletes a table with the current name from the database
        /// </summary>
        void ClearCurrentTable_SqlCommand()
        {
            //https://www.w3schools.com/sql/sql_drop_table.asp
            inlineSQL = $@"DROP TABLE {table}";

            Execute_SqlCommand(inlineSQL);
        }

        




        /// <summary>
        /// Creates a table of a given name to the database
        /// </summary>
        void CreateTable_SqlCommand()
        {
            //https://www.w3schools.com/sql/sql_create_table.asp
            //https://www.w3schools.com/sql/sql_primarykey.ASP
            //https://www.tutorialsteacher.com/sqlserver/identity-column
            //sets the ID, Name, Location, Price, UoM, and Sell_by_Date of a product
            inlineSQL = $@"CREATE TABLE {table} (ID int PRIMARY KEY IDENTITY(1,1), Name nvarchar(50), Location nvarchar(50), Price decimal(6,2), UoM nchar(10), Sell_by_Date date)";

            Execute_SqlCommand(inlineSQL);
        }




        string? name;
        string? location;
        string? price;
        string? uom;
        string[]? splitSellDate;
        string? sellDate;

        
        /// <summary>
        /// Inserts data into the produce table
        /// </summary>
        /// <param name="data"></param>
        void InsertData_SqlCommand(string[] data)
        {
            counter = 0;
            
            name = data[counter++];
            location = data[counter++];
            price = data[counter++];
            uom = data[counter++];
            splitSellDate = data[counter].Split('-');
            //set the sell date to the correct format of YYYY-MM-DD
            sellDate = $"{splitSellDate[2]}-{splitSellDate[0]}-{splitSellDate[1]}";

            inlineSQL = $@"INSERT INTO {table} ([Name],[Location],[Price],[UoM],[Sell_by_Date]) VALUES ('{name}','{location}',{price},'{uom}','{sellDate}')";

            Execute_SqlCommand(inlineSQL);
        }



        /// <summary>
        /// Create a sql connection for this application
        /// </summary>
        /// <returns></returns>
        string GenerateSqlConnection()
        {
            SqlConnectionStringBuilder mySqlConnectionBuilder = new SqlConnectionStringBuilder();
            mySqlConnectionBuilder["server"] = @"(localdb)\MSSQLLocalDB";
            mySqlConnectionBuilder["Trusted_Connection"] = true;
            mySqlConnectionBuilder["Integrated Security"] = "SSPI";
            mySqlConnectionBuilder["Initial Catalog"] = "PROG260FA23";
            return mySqlConnectionBuilder.ToString();
        }
    }
}


