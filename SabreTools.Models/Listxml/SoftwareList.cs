using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Models.Listxml
{
    [XmlRoot("softwarelist")]
    public class SoftwareList
    {
        [XmlAttribute("tag")]
        public string Tag { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <remarks>(original|compatible)</remarks>
        [XmlAttribute("status")]
        public string Status { get; set; }

        [XmlAttribute("filter")]
        public string? Filter { get; set; }

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