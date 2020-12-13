﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

using SabreTools.Core;
using SabreTools.Filtering;
using Newtonsoft.Json;

namespace SabreTools.DatItems
{
    /// <summary>
    /// Represents a single instance of another item
    /// </summary>
    [JsonObject("instance"), XmlRoot("instance")]
    public class Instance : DatItem
    {
        #region Fields

        /// <summary>
        /// Name of the item
        /// </summary>
        [JsonProperty("name")]
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// Short name for the instance
        /// </summary>
        [JsonProperty("briefname", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [XmlElement("briefname")]
        public string BriefName { get; set; }

        #endregion

        #region Accessors

        /// <inheritdoc/>
        public override string GetName()
        {
            return Name;
        }

        /// <inheritdoc/>
        public override void SetName(string name)
        {
            Name = name;
        }

        /// <inheritdoc/>
        public override void SetFields(
            Dictionary<DatItemField, string> datItemMappings,
            Dictionary<MachineField, string> machineMappings)
        {
            // Set base fields
            base.SetFields(datItemMappings, machineMappings);

            // Handle Instance-specific fields
            if (datItemMappings.Keys.Contains(DatItemField.Instance_Name))
                Name = datItemMappings[DatItemField.Instance_Name];

            if (datItemMappings.Keys.Contains(DatItemField.Instance_BriefName))
                BriefName = datItemMappings[DatItemField.Instance_BriefName];
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a default, empty Instance object
        /// </summary>
        public Instance()
        {
            Name = string.Empty;
            ItemType = ItemType.Instance;
        }

        #endregion

        #region Cloning Methods

        public override object Clone()
        {
            return new Instance()
            {
                ItemType = this.ItemType,
                DupeType = this.DupeType,

                Machine = this.Machine.Clone() as Machine,
                Source = this.Source.Clone() as Source,
                Remove = this.Remove,

                Name = this.Name,
                BriefName = this.BriefName,
            };
        }

        #endregion

        #region Comparision Methods

        public override bool Equals(DatItem other)
        {
            // If we don't have a Instance, return false
            if (ItemType != other.ItemType)
                return false;

            // Otherwise, treat it as a Instance
            Instance newOther = other as Instance;

            // If the Instance information matches
            return (Name == newOther.Name && BriefName == newOther.BriefName);
        }

        #endregion

        #region Filtering

        /// <inheritdoc/>
        public override bool PassesFilter(Cleaner cleaner, bool sub = false)
        {
            // Check common fields first
            if (!base.PassesFilter(cleaner, sub))
                return false;

            // Filter on item name
            if (!Filter.PassStringFilter(cleaner.DatItemFilter.Instance_Name, Name))
                return false;

            // Filter on brief name
            if (!Filter.PassStringFilter(cleaner.DatItemFilter.Instance_BriefName, BriefName))
                return false;

            return true;
        }

        /// <inheritdoc/>
        public override void RemoveFields(
            List<DatItemField> datItemFields,
            List<MachineField> machineFields)
        {
            // Remove common fields first
            base.RemoveFields(datItemFields, machineFields);

            // Remove the fields
            if (datItemFields.Contains(DatItemField.Instance_Name))
                Name = null;

            if (datItemFields.Contains(DatItemField.Instance_BriefName))
                BriefName = null;
        }

        /// <summary>
        /// Set internal names to match One Rom Per Game (ORPG) logic
        /// </summary>
        public override void SetOneRomPerGame()
        {
            string[] splitname = Name.Split('.');
            Machine.Name += $"/{string.Join(".", splitname.Take(splitname.Length > 1 ? splitname.Length - 1 : 1))}";
            Name = Path.GetFileName(Name);
        }

        #endregion

        #region Sorting and Merging

        /// <inheritdoc/>
        public override void ReplaceFields(
            DatItem item,
            List<DatItemField> datItemFields,
            List<MachineField> machineFields)
        {
            // Replace common fields first
            base.ReplaceFields(item, datItemFields, machineFields);

            // If we don't have a Instance to replace from, ignore specific fields
            if (item.ItemType != ItemType.Instance)
                return;

            // Cast for easier access
            Instance newItem = item as Instance;

            // Replace the fields
            if (datItemFields.Contains(DatItemField.Instance_Name))
                Name = newItem.Name;

            if (datItemFields.Contains(DatItemField.Instance_BriefName))
                BriefName = newItem.BriefName;
        }

        #endregion
    }
}
