using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Models.Listxml
{
    [XmlRoot("feature")]
    public class Feature
    {
        /// <remarks>(protection|timing|graphics|palette|sound|capture|camera|microphone|controls|keyboard|mouse|media|disk|printer|tape|punch|drum|rom|comms|lan|wan)</remarks>
        [XmlAttribute("type")]
        public string Type { get; set; }

        /// <remarks>(unemulated|imperfect)</remarks>
        [XmlAttribute("status")]
        public string? Status { get; set; }

        /// <remarks>(unemulated|imperfect)</remarks>
        [XmlAttribute("overall")]
        public string? Overall { get; set; }
    }
}