using System.Text.Json.Serialization;

namespace StructuredTextParser
{
    /// <summary>
    /// Defines an address held in student info stored in a json document
    /// </summary>
    public class Address
    {
        [JsonPropertyName("streetAddress")]
        public string StreetAddress;

        [JsonPropertyName("city")]
        public string City;

        [JsonPropertyName("state")]
        public string State;

        [JsonPropertyName("postalCode")]
        public string PostalCode;
    }
}
