using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Models.SoftwareList
{
    [XmlRoot("dipswitch")]
    public class DipSwitch
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("tag")]
        public string Tag { get; set; }

        [XmlAttribute("mask")]
        public string? Mask { get; set; }

        [XmlElement("dipvalue")]
        public DipValue[]? DipValue { get; set; }
    }
}