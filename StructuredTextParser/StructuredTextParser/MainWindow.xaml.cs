using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.IO;
using System;
using System.Windows.Controls;

namespace StructuredTextParser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        //This is the directory this program reads from
        public static string? inputDirectory;

        //This is the directory this program writes to
        public static string? outputDirectory;

        //This holds the text that becomes parse options for the user to select
        public static string[] parseOptions = { "Parse CSV File." , "Parse Pipe File.", "Parse XML Grocery File.", "Parse JSON Student File.", "Parse Produce to SQL.", "Parse Character to SQL."};


        //These are the valid extension for parsing used throughout the program
        public static List<string> validFileExtensions = new List<string>();
        public static List<string> validSqlFileExtensions = new List<string>();

        /// <summary>
        /// Called when the program loads.
        /// It initializes all important info to the program
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            //allows the user to select multiple choices at once
            TheList.SelectionMode = SelectionMode.Multiple;

            //populate the list of options with selections for the user
            //TheList.Items.Add(parseOptions[0]);
            //TheList.Items.Add(parseOptions[1]);
            //TheList.Items.Add(parseOptions[2]);
            //TheList.Items.Add(parseOptions[3]);
            //TheList.Items.Add(parseOptions[4]);
            TheList.Items.Add(parseOptions[5]);
        }

        public static string? testText;

        /// <summary>
        /// This is what happens when the user clicks The Button with the word "Activate"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Clear the output directory
            ClearOutputDirectory("..\\..\\..\\resources\\output");

            ParseUserSelections();

            //Stream data from input path (first value passed) to the output path (second value passed)
            Importer.StreamDataToSql("..\\..\\..\\resources\\DataToSQL", "..\\..\\..\\resources\\output");


            //Stream data from input path (first value passed) to the output path (second value passed)
            Importer.StreamDataToTxt("..\\..\\..\\resources\\ParseToTxt", "..\\..\\..\\resources\\output");


            //Inform the user that their files have been processed in The Textbox
            TheText.Text = "Data processed. Check \"..\\..\\..\\resources\\output\".";




        }

        /// <summary>
        /// A method used to ensure only the user's selected parse options are completed
        /// </summary>
        void ParseUserSelections()
        {
            validFileExtensions.Clear();
            validSqlFileExtensions.Clear();

            foreach (string item in TheList.SelectedItems)
            {
                if (item == parseOptions[0] && !validFileExtensions.Contains(".csv"))
                {
                    validFileExtensions.Add(".csv");
                }
                else if (item == parseOptions[1] && !validFileExtensions.Contains(".txt"))
                {
                    validFileExtensions.Add(".txt");
                }
                else if (item == parseOptions[2] && !validFileExtensions.Contains(".xml"))
                {
                    validFileExtensions.Add(".xml");
                }
                else if (item == parseOptions[3] && !validFileExtensions.Contains(".json"))
                {
                    validFileExtensions.Add(".json");
                }
                else if (item == parseOptions[4] && !validSqlFileExtensions.Contains(".txt"))
                {
                    validSqlFileExtensions.Add(".txt");
                }
                else if (item == parseOptions[5] && !validSqlFileExtensions.Contains(".csv"))
                {
                    validSqlFileExtensions.Add(".csv");
                }
            }
        }





        /// <summary>
        /// Delete all files currently in the output directory
        /// </summary>
        /// <param name="outputPath"></param>this is the directory that will be cleared
        static void ClearOutputDirectory(string outputPath)
        {
            try
            {
                var dir = Directory.GetFiles(outputPath);
                foreach (string file in dir)
                {
                    File.Delete(file);
                }
            }
            catch (Exception missingOutput)
            {
                ErrorLog.LogError(missingOutput.ToString(), "output directory missing.");
            }
        }



    }
}