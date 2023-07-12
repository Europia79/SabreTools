using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Models.SoftwareList
{
    [XmlRoot("diskarea")]
    public class DiskArea
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlElement("disk")]
        public Disk[]? Disk { get; set; }
    }
}