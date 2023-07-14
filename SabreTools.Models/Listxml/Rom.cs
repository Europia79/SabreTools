using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Models.Listxml
{
    [XmlRoot("rom")]
    public class Rom
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("bios")]
        public string? Bios { get; set; }

        [XmlAttribute("size")]
        public long Size { get; set; }

        [XmlAttribute("crc")]
        public string? CRC { get; set; }

        [XmlAttribute("sha1")]
        public string? SHA1 { get; set; }

        [XmlAttribute("merge")]
        public string? Merge { get; set; }

        [XmlAttribute("region")]
        public string? Region { get; set; }

        /// <remarks>Numeric?</remarks>
        [XmlAttribute("offset")]
        public string? Offset { get; set; }

        /// <remarks>(baddump|nodump|good) "good"</remarks>
        [XmlAttribute("status")]
        public string? Status { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        [XmlAttribute("optional")]
        public string? Optional { get; set; }

        /// <remarks>(yes|no) "no", Only present in older versions</remarks>
        [XmlAttribute("dispose")]
        public string? Dispose { get; set; }

        /// <remarks>(yes|no) "no", Only present in older versions</remarks>
        [XmlAttribute("soundonly")]
        public string? SoundOnly { get; set; }

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