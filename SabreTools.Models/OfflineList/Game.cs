using System.Xml;
using System.Xml.Serialization;

namespace SabreTools.Models.OfflineList
{
    [XmlRoot("game")]
    public class Game
    {
        [XmlElement("imageNumber")]
        public string? ImageNumber { get; set; }

        [XmlElement("releaseNumber")]
        public string? ReleaseNumber { get; set; }

        [XmlElement("title")]
        public string? Title { get; set; }

        [XmlElement("saveType")]
        public string? SaveType { get; set; }

        [XmlElement("romSize")]
        public long? RomSize { get; set; }

        [XmlElement("publisher")]
        public string? Publisher { get; set; }

        [XmlElement("location")]
        public string? Location { get; set; }

        [XmlElement("sourceRom")]
        public string? SourceRom { get; set; }

        [XmlElement("language")]
        public string? Language { get; set; }

        [XmlElement("files")]
        public Files? Files { get; set; }

        [XmlElement("im1CRC")]
        public string? Im1CRC { get; set; }

        [XmlElement("im2CRC")]
        public string? Im2CRC { get; set; }

        [XmlElement("comment")]
        public string? Comment { get; set; }

        [XmlElement("duplicateId")]
        public string? DuplicateID { get; set; }
    }
}