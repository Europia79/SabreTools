using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Models.OfflineList
{
    [XmlRoot("find")]
    public class Find
    {
        [XmlAttribute("operation")]
        public string? Operation { get; set; }

        /// <remarks>Numeric?</remarks>
        [XmlAttribute("value")]
        public string? Value { get; set; }

        public string? Content { get; set; }
    }
}