using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.IO;
using System;


namespace CSV_Pipe_To_TabDelimited
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //This holds the pairings of valid extension and that file type's associated delimiter
        public static Dictionary<string, char> extensions_And_Delimiter_To_Parse = new Dictionary<string, char>();

        //An array of the valid file extension
        public static string[]? validFileExtensions;

        //This is the directory this program reads from
        public static string? inputDirectory;

        //This is the directory this program writes to
        public static string? outputDirectory;

        //a list of all files in the directory
        List<string> all_FilesInDirectory = new List<string>();

       
        /// <summary>
        /// Called when the program loads.
        /// It initializes all important info to the program
        /// </summary>
        public MainWindow()
        {
            InitializeDesired_ExtensionDelimiterPairing();

            InitializeDirectories("..\\..\\..\\resources", "..\\..\\..\\resources\\output");

            InitializeComponent();
        }



        /// <summary>
        /// This is what happens when the user clicks The Button with the word "Activate"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Get a list of all paths to all files in the directory
                all_FilesInDirectory = Directory.GetFiles(inputDirectory).ToList();

                //Stream the data into a txt file held in the output directory
                Importer.StreamData(all_FilesInDirectory, outputDirectory);

                //Inform the user that their files have been processed in The Textbox
                TheText.Text = "Files Processed";
            }
            catch(Exception IO_error)
            {
                ErrorLog.LogError(IO_error.ToString(), "Button");
            }
        }



        /// <summary>
        /// This initialized the extensions and delimiters that are going
        /// to be parsed. It's currently hard-coded, but it's not hard to
        /// modify for a user to pass in a string and char.
        /// </summary>
        private void InitializeDesired_ExtensionDelimiterPairing()
        {
            //Adds the parse parameters for csv files
            extensions_And_Delimiter_To_Parse.Add(".csv", ',');


            //Adds the parse parameters for pipe files
            extensions_And_Delimiter_To_Parse.Add(".txt", '|');

            //Gets a string array of the correct file extensions to parse
            validFileExtensions = extensions_And_Delimiter_To_Parse.Keys.ToArray();
        }



        /// <summary>
        /// A method to set the input and output directories
        /// </summary>
        /// <param name="inDirectory">The input directory
        /// <param name="outDirectory">The output directory
        private void InitializeDirectories(string inDirectory, string outDirectory)
        {
            inputDirectory = inDirectory;

            outputDirectory = outDirectory;
        }



        


    }
}
