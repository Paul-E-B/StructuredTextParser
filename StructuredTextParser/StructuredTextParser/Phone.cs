using System.Text.Json.Serialization;

namespace StructuredTextParser
{
    /// <summary>
    /// Defines a phone number held in student info stored in a json document
    /// </summary>
    public class Phone
    {
        [JsonPropertyName("type")]
        public string Type;

        [JsonPropertyName("number")]
        public string Number;

        [JsonPropertyName("CanContact")]
        public bool CanContact;
    }
}
