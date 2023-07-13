using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Models.OfflineList
{
    [XmlRoot("dat")]
    public class Dat
    {
        [XmlAttribute(Namespace = "http://www.w3.org/2001/XMLSchema-instance", AttributeName = "noNamespaceSchemaLocation")]
        public string? NoNamespaceSchemaLocation { get; set; }

        [XmlElement("configuration")]
        public Configuration? Configuration { get; set; }

        [XmlElement("games")]
        public Games? Games { get; set; }

        [XmlElement("gui")]
        public GUI? GUI { get; set; }

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