using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace StructuredTextParser
{
    /// <summary>
    /// Defines student info stored in a json document
    /// </summary>
    public class Student
    {
        [JsonPropertyName("firstName")]
        public string FirstName;

        [JsonPropertyName("lastName")]
        public string LastName;

        [JsonPropertyName("isEnrolled")]
        public bool isEnrolled;

        [JsonPropertyName("YearsEnrolled")]
        public int YearsEnrolled;

        [JsonPropertyName("address1")]
        public Address Address1;

        [JsonPropertyName("address2")]
        public Address Address2;

        [JsonPropertyName("phoneNumbers")]
        public List<Phone> PhoneNumbers = new List<Phone>();
    }
}
