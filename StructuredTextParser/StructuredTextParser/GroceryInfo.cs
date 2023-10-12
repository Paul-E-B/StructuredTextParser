using System.Xml.Serialization;

namespace StructuredTextParser
{
    /// <summary>
    /// Contains info for a single grocery item stored in an XML format
    /// </summary>
    [XmlRoot(ElementName = "item")]
    public class GroceryInfo
    {
        [XmlElement(ElementName = "name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "price")]
        public string Price { get; set; }

        [XmlElement(ElementName = "uom")]
        public string Uom { get; set; }
    }
}
