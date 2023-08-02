using System.Xml.Serialization;
using Newtonsoft.Json;

namespace SabreTools.Models.Internal
{
    [JsonObject("blank"), XmlRoot("blank")]
    public class Blank : DatItem
    {
        public Blank() => Type = ItemType.Blank;
    }
}
