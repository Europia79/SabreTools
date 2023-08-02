using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Models.Internal
{
    [JsonObject("extension"), XmlRoot("extension")]
    public class Extension : DatItem
    {
        #region Keys

        /// <remarks>string</remarks>
        public const string NameKey = "name";

        #endregion

        public Extension() => Type = ItemType.Extension;
    }
}
