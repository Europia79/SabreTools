using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Models.Internal
{
    [JsonObject("slot"), XmlRoot("slot")]
    public class Slot : DatItem
    {
        #region Keys

        /// <remarks>string</remarks>
        public const string NameKey = "name";

        /// <remarks>SlotOption[]</remarks>
        [NoFilter]
        public const string SlotOptionKey = "slotoption";

        #endregion

        public Slot() => Type = ItemType.Slot;
    }
}
