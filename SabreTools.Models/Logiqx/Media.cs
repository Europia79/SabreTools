using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Models.Logiqx
{
    [XmlRoot("media")]
    public class Media : ItemBase
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("md5")]
        public string? MD5 { get; set; }

        [XmlAttribute("sha1")]
        public string? SHA1 { get; set; }

        [XmlAttribute("sha256")]
        public string? SHA256 { get; set; }

        [XmlAttribute("spamsum")]
        public string? SpamSum { get; set; }
    }
}