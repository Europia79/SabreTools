using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Models.SoftwareList
{
    [XmlRoot("softwarelist")]
    public class SoftwareList
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("description")]
        public string? Description { get; set; }

        [XmlElement("notes")]
        public string? Notes { get; set; }

        [XmlElement("software")]
        public Software[] Software { get; set; }
    }
}