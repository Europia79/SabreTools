using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Models.OfflineList
{
    [XmlRoot("datURL")]
    public class DatUrl
    {
        [XmlAttribute("fileName")]
        public string? FileName { get; set; }

        [XmlText]
        public string? Content { get; set; }
    }
}