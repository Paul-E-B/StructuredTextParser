using System.Collections.Generic;
using System.Xml.Serialization;

namespace StructuredTextParser
{
    /// <summary>
    /// A list of all grocery items. 
    /// Held inside a "menu" root in xml document
    /// </summary>
    [XmlRoot(ElementName = "menu")]
    public class Grocery
    {
        [XmlElement(ElementName = "item")]
        public List<GroceryInfo> Item { get; set; }
    }
}
