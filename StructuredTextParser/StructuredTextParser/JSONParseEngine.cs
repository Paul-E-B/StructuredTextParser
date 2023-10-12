using System.Collections.Generic;
using System.IO;
using System;
using Newtonsoft.Json;


namespace StructuredTextParser
{
    /// <summary>
    /// A class used for parsing JSON files for students at a school
    /// </summary>
    internal class JSONParseEngine : BaseEngine 
    {
        int lineCounter = 0;

        string? phoneData = null;
        int currentPhoneNumber = 1;

        /// <summary>
        /// Parse data in a file ending in .json
        /// </summary>
        /// <param name="file"></param>Current file being parsed
        /// <param name="outputPath"></param>Path the data is being output to
        public override void Process(IFile file, string outputPath)
        {
            try
            {
                //File stream used to read xml file
                using (StreamReader inputFileReader = new StreamReader(file.Path))
                {
                    Student currentStudent = JsonConvert.DeserializeObject<Student>(inputFileReader.ReadToEnd());

                    using (FileStream outputFile = new FileStream(GenerateOutputFileName(outputPath, file.Name), FileMode.OpenOrCreate))
                    {

                        using (StreamWriter outputWriter = new StreamWriter(outputFile))
                        {
                            lineCounter = 1;

                            outputWriter.Write($"Student#{lineCounter++} :");
                            outputWriter.Write($"First Name={currentStudent.FirstName}==> ");
                            outputWriter.Write($"Last Name={currentStudent.LastName}==> ");
                            outputWriter.Write($"IsEnrolled={currentStudent.isEnrolled.ToString()}==> ");
                            outputWriter.Write($"YearsEnrolled={currentStudent.YearsEnrolled}\n");

                            outputWriter.WriteLine($"Address 1 :{CheckAndFormatAddress(currentStudent.Address1)}");
                            outputWriter.WriteLine($"Address 2 :{CheckAndFormatAddress(currentStudent.Address2)}");

                            outputWriter.WriteLine(FormatPhoneInfo(currentStudent.PhoneNumbers)+"\n");

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
        /// Check if there is an Address for the current info section.
        /// If true, format the data and return it to the stream writer
        /// If false, return a negator to the stream writer
        /// </summary>
        /// <param name="address"></param>The info for the current address
        /// <returns>The formatted address or a message used as a stand in for null</returns>
        string CheckAndFormatAddress(Address address)
        {
            if(address != null)
            {
                return ($"Street Address={address.StreetAddress}==> City={address.City}==> State={address.State}==> Postal Code={address.PostalCode}");
            }
            else
            {
                return "No Address";
            }
        }

        /// <summary>
        /// For each phone number listed in the json file, format the data appropriately
        /// then return it to the stream writer
        /// </summary>
        /// <param name="phoneList"></param>A list of all phone numbers in the current student's records
        /// <returns>The formatted phone data</returns>
        string FormatPhoneInfo(List<Phone> phoneList)
        {
            phoneData = null;
            currentPhoneNumber = 1;

            foreach (Phone phone in phoneList)
            {
                phoneData += $"Phone Number {currentPhoneNumber} :Type={phone.Type}==> Number={phone.Number}==> CanContact={phone.CanContact}\n";
                currentPhoneNumber++;
            }
            return phoneData;
        }

    }

}
