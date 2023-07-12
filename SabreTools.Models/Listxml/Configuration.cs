using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Models.Listxml
{
    [XmlRoot("configuration")]
    public class Configuration
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("tag")]
        public string Tag { get; set; }

        [XmlAttribute("mask")]
        public string? Mask { get; set; }

        [XmlElement("condition")]
        public Condition? Condition { get; set; }

        [XmlElement("conflocation")]
        public ConfLocation[]? ConfLocation { get; set; }

        [XmlElement("confsetting")]
        public ConfSetting[]? ConfSetting { get; set; }
    }
}