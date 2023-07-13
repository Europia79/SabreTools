using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Models.Listxml
{
    [XmlRoot("display")]
    public class Display
    {
        [XmlAttribute("tag")]
        public string? Tag { get; set; }

        /// <remarks>(raster|vector|lcd|svg|unknown)</remarks>
        [XmlAttribute("type")]
        public string Type { get; set; }

        /// <remarks>(0|90|180|270)</remarks>
        [XmlAttribute("rotate")]
        public string? Rotate { get; set; }

        /// <remarks>(yes|no) "no"</remarks>
        [XmlAttribute("flipx")]
        public string? FlipX { get; set; }

        /// <remarks>Numeric?</remarks>
        [XmlAttribute("width")]
        public string? Width { get; set; }

        /// <remarks>Numeric?</remarks>
        [XmlAttribute("height")]
        public string? Height { get; set; }

        /// <remarks>Numeric?</remarks>
        [XmlAttribute("refresh")]
        public string Refresh { get; set; }

        /// <remarks>Numeric?</remarks>
        [XmlAttribute("pixclock")]
        public string? PixClock { get; set; }

        /// <remarks>Numeric?</remarks>
        [XmlAttribute("htotal")]
        public string? HTotal { get; set; }

        /// <remarks>Numeric?</remarks>
        [XmlAttribute("hbend")]
        public string? HBEnd { get; set; }

        /// <remarks>Numeric?</remarks>
        [XmlAttribute("hbstart")]
        public string? HBStart { get; set; }

        /// <remarks>Numeric?</remarks>
        [XmlAttribute("vtotal")]
        public string? VTotal { get; set; }

        /// <remarks>Numeric?</remarks>
        [XmlAttribute("vbend")]
        public string? VBEnd { get; set; }

        /// <remarks>Numeric?</remarks>
        [XmlAttribute("vbstart")]
        public string? VBStart { get; set; }

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