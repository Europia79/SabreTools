using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Models.Internal
{
    [JsonObject("device_ref"), XmlRoot("device_ref")]
    public class DeviceRef : DatItem
    {
        #region Keys

        /// <remarks>string</remarks>
        public const string NameKey = "name";

        #endregion

        public DeviceRef() => Type = ItemType.DeviceRef;
    }
}
