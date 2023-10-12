using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.IO;
using System;


namespace StructuredTextParser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //This holds the pairings of valid extension and that file type's associated delimiter
        public static Dictionary<string, char> extensions_And_Delimiter_To_Parse = new Dictionary<string, char>();

        //This is the directory this program reads from
        public static string? inputDirectory;

        //This is the directory this program writes to
        public static string? outputDirectory;

        

        /// <summary>
        /// Called when the program loads.
        /// It initializes all important info to the program
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }



        /// <summary>
        /// This is what happens when the user clicks The Button with the word "Activate"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Clear the output directory
            ClearOutputDirectory("..\\..\\..\\resources\\output");

            //Stream data from input path (first value passed) to the output path (second value passed)
            Importer.StreamData("..\\..\\..\\resources", "..\\..\\..\\resources\\output");

            //Inform the user that their files have been processed in The Textbox
            TheText.Text = "Files Processed";
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