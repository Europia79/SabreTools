using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Models.Listxml
{
    [XmlRoot("ramoption")]
    public class RamOption
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("default")]
        public string? Default { get; set; }
    }
}