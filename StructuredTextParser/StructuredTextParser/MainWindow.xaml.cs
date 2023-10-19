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


        public static string[] parseOptions = { "Parse CSV File." , "Parse Pipe File.", "Parse XML Grocery File.", "Parse JSON Student File.", "Parse Txt Produce to SQL."};



        public static List<string> validFileExtensions = new List<string>();
        public static List<string> validSqlFileExtensions = new List<string>();

        /// <summary>
        /// Called when the program loads.
        /// It initializes all important info to the program
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            TheList.SelectionMode = SelectionMode.Multiple;

            TheList.Items.Add(parseOptions[0]);
            TheList.Items.Add(parseOptions[1]);
            TheList.Items.Add(parseOptions[2]);
            TheList.Items.Add(parseOptions[3]);
            TheList.Items.Add(parseOptions[4]);
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

            Importer.StreamDataToSql("..\\..\\..\\resources\\DataToSQL", "..\\..\\..\\resources\\output");


            //Stream data from input path (first value passed) to the output path (second value passed)
            Importer.StreamDataToTxt("..\\..\\..\\resources\\ParseToTxt", "..\\..\\..\\resources\\output");


            //Inform the user that their files have been processed in The Textbox
            TheText.Text = "Data processed. Check \"..\\..\\..\\resources\\output\".";




        }

        void ParseUserSelections()
        {
            validFileExtensions.Clear();
            validSqlFileExtensions.Clear();

            foreach (string item in TheList.SelectedItems)
            {
                if (item == parseOptions[0])
                {
                    validFileExtensions.Add(".csv");
                }
                else if (item == parseOptions[1])
                {
                    validFileExtensions.Add(".txt");
                }
                else if (item == parseOptions[2])
                {
                    validFileExtensions.Add(".xml");
                }
                else if (item == parseOptions[3])
                {
                    validFileExtensions.Add(".json");
                }
                else if (item == parseOptions[4])
                {
                    validSqlFileExtensions.Add(".txt");
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