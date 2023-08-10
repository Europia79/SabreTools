using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Models.OpenMSX
{
    [XmlRoot("software")]
    public class Software
    {
        [Required]
        [XmlElement("title")]
        public string? Title { get; set; }

        [XmlElement("genmsxid")]
        public string? GenMSXID { get; set; }

        [Required]
        [XmlElement("system")]
        public string? System { get; set; }

        [Required]
        [XmlElement("company")]
        public string? Company { get; set; }

        [Required]
        [XmlElement("year")]
        public string? Year { get; set; }

        [Required]
        [XmlElement("country")]
        public string? Country { get; set; }

        [XmlElement("dump")]
        public Dump[]? Dump { get; set; }

        #region DO NOT USE IN PRODUCTION

        /// <remarks>Should be empty</remarks>
        [XmlAnyAttribute]
        public XmlAttribute[]? ADDITIONAL_ATTRIBUTES { get; set; }

        /// <remarks>Should be empty</remarks>
        [XmlAnyElement]
        public object[]? ADDITIONAL_ELEMENTS { get; set; }

        #endregion
    }
}