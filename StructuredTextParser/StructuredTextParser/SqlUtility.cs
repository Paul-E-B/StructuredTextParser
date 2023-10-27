using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Xml.Linq;

namespace StructuredTextParser
{
    internal class SqlUtility
    {
        static string? inlineSQL;


        /// <summary>
        /// Create a sql connection for this application
        /// </summary>
        /// <returns>the string used to connect to the SQL table</returns>
        public static string GenerateSqlConnection(SqlTableInfo currentTable)
        {
            SqlConnectionStringBuilder mySqlConnectionBuilder = new SqlConnectionStringBuilder();
            mySqlConnectionBuilder["server"] = currentTable.Server;
            mySqlConnectionBuilder["Trusted_Connection"] = currentTable.TrustedConnection;
            mySqlConnectionBuilder["Integrated Security"] = currentTable.IntegratedSecurity;
            mySqlConnectionBuilder["Initial Catalog"] = currentTable.InitialCatalog;
            return mySqlConnectionBuilder.ToString();
        }



        /// <summary>
        /// Deletes a table with the current name from the database
        /// </summary>
        public static void ClearTableFromSQL(SqlTableInfo currentTable, SqlConnection connection)
        {
            //https://www.w3schools.com/sql/sql_drop_table.asp
            inlineSQL = $@"DROP TABLE {currentTable.Table}";

            Execute_SqlCommand(inlineSQL, connection);
        }






        /// <summary>
        /// General use method for executing a SQL command
        /// </summary>
        /// <param name="commandStatement"></param>The statement to execute
        public static void Execute_SqlCommand(string commandStatement, SqlConnection connection)
        {
            try
            {
                using (var command = new SqlCommand(commandStatement, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception err)
            {
                ErrorLog.LogError(err.ToString(), "Execute_SqlCommand");
            }
        }


        /// <summary>
        /// General use method for executing a series of SQL commands
        /// </summary>
        /// <param name="commandStatement"></param>The statement to execute
        public static void Execute_SqlCommands(List<string> commandStatements, SqlConnection connection)
        {
            try
            {
                foreach (string commandStatement in commandStatements)
                {
                    using (var command = new SqlCommand(commandStatement, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception err)
            {
                ErrorLog.LogError(err.ToString(), "Execute_SqlCommand");
            }
        }


        static int c = 0;

        /// <summary>
        /// Creates a table of a given name to the database
        /// </summary>
        public static void CreateSQLTable(SqlTableInfo currentTable, SqlConnection connection)
        {
            

            //sets the ID, Name, Location, Price, UoM, and Sell_by_Date of a product
            inlineSQL = $@"CREATE TABLE {currentTable.Table} (ID int PRIMARY KEY IDENTITY(1,1), ";

            for (c = 0; c + 1 < currentTable.DataNames.Length; c++)
            {
                inlineSQL += $"{currentTable.DataNames[c]} {currentTable.DataTypes[c]} {currentTable.IsDataNull[c]}, ";
                
            }

            inlineSQL += $"{currentTable.DataNames[c]} {currentTable.DataTypes[c]} {currentTable.IsDataNull[c]})";


            Execute_SqlCommand(inlineSQL, connection);
        }


        //current line in a piece of parsed data
        static string? currentLineInFile;
        //holds data after it's been split at its delimiter
        static string[]? splitData;

        public static void InsertTxtDataIntoSQLTable(SqlTableInfo currentTable, StreamReader inputFileReader, char delimiter, SqlConnection connection)
        {

            while ((currentLineInFile = inputFileReader.ReadLine()) != null)
            {
                splitData = null;

                splitData = currentLineInFile.Split(delimiter);

                inlineSQL = $@"INSERT INTO {currentTable.Table} (";

                for (c = 0; c + 1 < currentTable.DataNames.Length;c++)
                {
                    inlineSQL += $"[{currentTable.DataNames[c]}],";
                }

                inlineSQL += $"[{currentTable.DataNames[c]}]) VALUES (";
                
                for (c = 0; c + 1 < splitData.Length;c++)
                {
                    inlineSQL += $"{FormatDataForSQL(currentTable.DataTypes[c], splitData[c])},";
                }
                inlineSQL += $"{FormatDataForSQL(currentTable.DataTypes[c], splitData[c])})";


                Execute_SqlCommand(inlineSQL, connection);
            }



        }


        static List<string> inlineSqlCommands = new List<string>();

        static int i = 0;
        static int characterId = 1;

        /// <summary>
        /// Read in data from a txt file and input it into an SQL table
        /// </summary>
        /// <param name="currentTables"></param>The list of all tables that hold character data
        /// <param name="inputFileReader"></param>the file reader that reads in data from the char.csv file
        /// <param name="delimiter"></param>the delimiter used for parsing the txt info
        /// <param name="connection"></param>the sql connect
        public static void InsertDataIntoCharacterSQLTables(List<SqlTableInfo> currentTables, StreamReader inputFileReader, char delimiter, SqlConnection connection)
        {


            while ((currentLineInFile = inputFileReader.ReadLine()) != null)
            {
                inlineSqlCommands.Clear();

                //used here to iterate through the inline sql commands and populate with data names appropriate for each column
                i = 0;
                foreach (SqlTableInfo currentTable in currentTables)
                {
                    inlineSqlCommands.Add($@"INSERT INTO {currentTable.Table} (");


                    for (c = 0; c + 1 < currentTable.DataNames.Length; c++)
                    {
                        inlineSqlCommands[i] += $"[{currentTable.DataNames[c]}],";
                    }
                    inlineSqlCommands[i] += $"[{currentTable.DataNames[c]}]) ";

                    i++;

                }



                splitData = null;

                splitData = currentLineInFile.Split(delimiter);


                //Character table
                inlineSqlCommands[0] += "VALUES (";

                //Character Location table
                inlineSqlCommands[1] += "VALUES (";

                //CharacterCombatInfo table
                inlineSqlCommands[2] += "VALUES (";

                //CharacterOriginality table
                inlineSqlCommands[3] += "VALUES (";


                //used to track current segment of data
                c = 0;

                //0 - name
                inlineSqlCommands[0] += $"{FormatDataForSQL(currentTables[0].DataTypes[0], DataAtIndex(splitData, c++))},";

                //0 - Set CharacterID
                inlineSqlCommands[1] += $"{FormatDataForSQL(currentTables[1].DataTypes[0], $"{characterId}")},";
                inlineSqlCommands[2] += $"{FormatDataForSQL(currentTables[2].DataTypes[0], $"{characterId}")},";
                inlineSqlCommands[3] += $"{FormatDataForSQL(currentTables[3].DataTypes[0], $"{characterId}")},";

                //1 - type
                inlineSqlCommands[0] += $"{FormatDataForSQL(currentTables[0].DataTypes[1], DataAtIndex(splitData, c++))})";

                //2 - map_location
                inlineSqlCommands[1] += $"{FormatDataForSQL(currentTables[1].DataTypes[1], DataAtIndex(splitData, c++))})";

                //3 original_character
                inlineSqlCommands[3] += $"{FormatDataForSQL(currentTables[3].DataTypes[1], DataAtIndex(splitData, c++))})";

                //4 -sword_fighter
                inlineSqlCommands[2] += $"{FormatDataForSQL(currentTables[2].DataTypes[1], DataAtIndex(splitData, c++))},";

                //5 - magic_user
                inlineSqlCommands[2] += $"{FormatDataForSQL(currentTables[2].DataTypes[2], DataAtIndex(splitData, c++))})";

                characterId++;
                

                Execute_SqlCommands(inlineSqlCommands, connection);        
            }

        }

        /// <summary>
        /// Check is there is data at an index in an array. If none, return a null value
        /// </summary>
        /// <param name="data"></param>A string array of split data from an read txt line
        /// <param name="index"></param>the current data increment in the string array
        /// <returns>data at that index</returns>
        public static string? DataAtIndex(string[] data, int index)
        {
            if(index < data.Length)
            {
                return data[index];
            }

            return null;
        }


        static string formattedData = "NULL";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        static string FormatDataForSQL(string dataType, string? data)
        {

            if (string.IsNullOrEmpty(data))
            {
                return "NULL";
            }

            if (dataType == "nvarchar(5)" || dataType == "nvarchar(50)" || dataType == "nvarchar(10)" || dataType == "date")
            {
                data = data.Replace("'", "''");
                return $"'{data}'";
            }
            else if (dataType == "decimal(6,2)" || dataType == "int")
            {
                return $"{data}";
            }

            return formattedData;
        }

        /// <summary>
        /// Create a SQL query that joins all character tables data
        /// </summary>
        /// <returns>the SQL query that joins all character tables data</returns>
        public static string FullCharacterReport()
        {
            inlineSQL = $@"SELECT c.ID, c.Character, c.Type, cl.Map_Location, co.Original_Character, cci.Sword_Fighter, cci.Magic_User ";
            inlineSQL += "FROM CharacterInfo AS c ";
            inlineSQL += "INNER JOIN CharacterLocation AS cl ON c.ID = cl.CharacterID ";
            inlineSQL += "INNER JOIN CharacterOriginality AS co ON c.ID = co.CharacterID ";
            inlineSQL += "INNER JOIN CharacterCombatInfo AS cci ON c.ID = cci.CharacterID";
            return inlineSQL;
        }

        /// <summary>
        /// Create a SQL query that finds all characters without a map location
        /// </summary>
        /// <returns>the SQL query that finds all characters without a map location</returns>
        public static string LostCharacter()
        {
            inlineSQL = $@"SELECT ci.Character FROM CharacterInfo AS ci ";
            inlineSQL += "LEFT JOIN CharacterLocation AS cl ";
            inlineSQL += "ON ci.ID = cl.CharacterID ";
            inlineSQL += "WHERE cl.Map_Location IS NULL";
            return inlineSQL;
        }


        /// <summary>
        /// Create a SQL query that finds all Non-Human Sword-Fighter characters
        /// </summary>
        /// <returns>the SQL query that finds all Non-Human Sword-Fighter characters</returns>
        public static string NonHuman_SwordUsers()
        {
            inlineSQL = $@"SELECT ci.Character FROM CharacterInfo AS ci ";
            inlineSQL += "INNER JOIN CharacterCombatInfo AS cci ON ci.ID = cci.CharacterID ";
            inlineSQL += "WHERE ci.Type != 'Human' AND cci.Sword_Fighter = 'TRUE'";
            return inlineSQL;
        }





    }
}
