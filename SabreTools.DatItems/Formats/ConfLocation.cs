using System.Xml.Serialization;
using Newtonsoft.Json;
using SabreTools.Core;
using SabreTools.Filter;

namespace SabreTools.DatItems.Formats
{
    /// <summary>
    /// Represents one conflocation
    /// </summary>
    [JsonObject("conflocation"), XmlRoot("conflocation")]
    public class ConfLocation : DatItem
    {
        #region Fields

        /// <summary>
        /// Location name
        /// </summary>
        [JsonProperty("name"), XmlElement("name")]
        public string? Name
        {
            get => _internal.ReadString(Models.Metadata.ConfLocation.NameKey);
            set => _internal[Models.Metadata.ConfLocation.NameKey] = value;
        }

        /// <summary>
        /// Location ID
        /// </summary>
        [JsonProperty("number", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("number")]
        public long? Number
        {
            get => _internal.ReadLong(Models.Metadata.ConfLocation.NumberKey);
            set => _internal[Models.Metadata.ConfLocation.NumberKey] = value;
        }

        [JsonIgnore]
        public bool NumberSpecified { get { return Number != null; } }

        /// <summary>
        /// Determines if location is inverted or not
        /// </summary>
        [JsonProperty("inverted", DefaultValueHandling = DefaultValueHandling.Ignore), XmlElement("inverted")]
        public bool? Inverted
        {
            get => _internal.ReadBool(Models.Metadata.ConfLocation.InvertedKey);
            set => _internal[Models.Metadata.ConfLocation.InvertedKey] = value;
        }

        [JsonIgnore]
        public bool InvertedSpecified { get { return Inverted != null; } }

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string? GetName() => Name;

        /// <inheritdoc/>
        public override void SetName(string? name) => Name = name;

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty ConfLocation object
        /// </summary>
        public ConfLocation()
        {
            _internal = new Models.Metadata.ConfLocation();
            Machine = new Machine();

            Name = string.Empty;
            ItemType = ItemType.ConfLocation;
        }

        #endregion

        #region Cloning Methods

        /// <inheritdoc/>
        public override object Clone()
        {
            return new ConfLocation()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine ?? new Machine(),
                Source = this.Source?.Clone() as Source,
                Remove = this.Remove,

                _internal = this._internal?.Clone() as Models.Metadata.ConfLocation ?? [],
            };
        }

        #endregion
    
        #region Manipulation

        /// <inheritdoc/>
        public override bool RemoveField(DatItemField datItemField)
        {
            // Get the correct internal field name
            string? fieldName = datItemField switch
            {
                DatItemField.Inverted => Models.Metadata.ConfLocation.InvertedKey,
                DatItemField.Location_Inverted => Models.Metadata.ConfLocation.InvertedKey,
                DatItemField.Number => Models.Metadata.ConfLocation.NumberKey,
                DatItemField.Location_Number => Models.Metadata.ConfLocation.NumberKey,
                _ => null,
            };

            // Remove the field and return
            return FieldManipulator.RemoveField(_internal, fieldName);
        }

        /// <inheritdoc/>
        public override bool SetField(DatItemField datItemField, string value)
        {
            // Get the correct internal field name
            string? fieldName = datItemField switch
            {
                DatItemField.Inverted => Models.Metadata.ConfLocation.InvertedKey,
                DatItemField.Location_Inverted => Models.Metadata.ConfLocation.InvertedKey,
                DatItemField.Number => Models.Metadata.ConfLocation.NumberKey,
                DatItemField.Location_Number => Models.Metadata.ConfLocation.NumberKey,
                _ => null,
            };

            // Set the field and return
            return FieldManipulator.SetField(_internal, fieldName, value);
        }

        #endregion
    }
}
