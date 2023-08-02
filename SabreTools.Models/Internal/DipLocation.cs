using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Models.Internal
{
    [JsonObject("diplocation"), XmlRoot("diplocation")]
    public class DipLocation : DatItem
    {
        #region Keys

        /// <remarks>(yes|no) "no"</remarks>
        public const string InvertedKey = "inverted";

        /// <remarks>string</remarks>
        public const string NameKey = "name";

        /// <remarks>string, possibly long</remarks>
        public const string NumberKey = "number";

        #endregion

        public DipLocation() => Type = ItemType.DipLocation;
    }
}
