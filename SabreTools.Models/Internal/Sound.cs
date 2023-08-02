using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Models.Internal
{
    [JsonObject("sound"), XmlRoot("sound")]
    public class Sound : DatItem
    {
        #region Keys

        /// <remarks>long</remarks>
        public const string ChannelsKey = "channels";

        #endregion

        public Sound() => Type = ItemType.Sound;
    }
}
