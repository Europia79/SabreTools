using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using SabreTools.Core;
using SabreTools.Core.Tools;

namespace SabreTools.DatItems
{
    /// <summary>
    /// Manipulate DatItems
    /// </summary>
    public static class DatItemTool
    {
        /// <summary>
        /// Replace fields with given values
        /// </summary>
        /// <param name="datItem">DatItem to replace fields in</param>
        /// <param name="repMachine">DatItem to pull new information from</param>
        /// <param name="datItemFields">List of fields representing what should be updated</param>
        public static void ReplaceFields(DatItem datItem, DatItem repDatItem, List<DatItemField> datItemFields)
        {
            if (datItem == null || repDatItem == null || datItemFields == null)
                return;

            #region Common

            if (datItem.ItemType != repDatItem.ItemType)
                return;

            if (datItemFields.Contains(DatItemField.Name))
                datItem.SetName(repDatItem.GetName());

            #endregion

            #region Adjuster

            if (datItem is Adjuster adjuster)
            {
                Adjuster newItem = repDatItem as Adjuster;

                if (datItemFields.Contains(DatItemField.Default))
                    adjuster.Default = newItem.Default;

                // Condition_* doesn't make sense here
                // since not every condition under the other item
                // can replace every condition under this item
            }

            #endregion

            #region Analog

            else if (datItem is Analog analog)
            {
                Analog newItem = repDatItem as Analog;

                if (datItemFields.Contains(DatItemField.Analog_Mask))
                    analog.Mask = newItem.Mask;
            }

            #endregion

            #region BiosSet

            else if (datItem is BiosSet biosSet)
            {
                BiosSet newItem = repDatItem as BiosSet;

                if (datItemFields.Contains(DatItemField.Description))
                    biosSet.Description = newItem.Description;

                if (datItemFields.Contains(DatItemField.Default))
                    biosSet.Default = newItem.Default;
            }

            #endregion

            #region Chip

            else if (datItem is Chip chip)
            {
                Chip newItem = repDatItem as Chip;

                if (datItemFields.Contains(DatItemField.Tag))
                    chip.Tag = newItem.Tag;

                if (datItemFields.Contains(DatItemField.ChipType))
                    chip.ChipType = newItem.ChipType;

                if (datItemFields.Contains(DatItemField.Clock))
                    chip.Clock = newItem.Clock;
            }

            #endregion

            #region Condition

            else if (datItem is Condition condition)
            {
                Condition newItem = repDatItem as Condition;

                if (datItemFields.Contains(DatItemField.Tag))
                    condition.Tag = newItem.Tag;
                else if (datItemFields.Contains(DatItemField.Condition_Tag))
                    condition.Tag = newItem.Tag;

                if (datItemFields.Contains(DatItemField.Mask))
                    condition.Mask = newItem.Mask;
                else if (datItemFields.Contains(DatItemField.Condition_Mask))
                    condition.Mask = newItem.Mask;

                if (datItemFields.Contains(DatItemField.Relation))
                    condition.Relation = newItem.Relation;
                else if (datItemFields.Contains(DatItemField.Condition_Relation))
                    condition.Relation = newItem.Relation;

                if (datItemFields.Contains(DatItemField.Value))
                    condition.Value = newItem.Value;
                else if (datItemFields.Contains(DatItemField.Condition_Value))
                    condition.Value = newItem.Value;
            }

            #endregion

            #region Configuration

            else if (datItem is Configuration configuration)
            {
                Configuration newItem = repDatItem as Configuration;

                if (datItemFields.Contains(DatItemField.Tag))
                    configuration.Tag = newItem.Tag;

                if (datItemFields.Contains(DatItemField.Mask))
                    configuration.Mask = newItem.Mask;

                // Condition_* doesn't make sense here
                // since not every condition under the other item
                // can replace every condition under this item

                // Location_* doesn't make sense here
                // since not every location under the other item
                // can replace every location under this item

                // Setting_* doesn't make sense here
                // since not every setting under the other item
                // can replace every setting under this item
            }

            #endregion

            #region Control

            else if (datItem is Control control)
            {
                Control newItem = repDatItem as Control;

                if (datItemFields.Contains(DatItemField.Control_Type))
                    control.ControlType = newItem.ControlType;

                if (datItemFields.Contains(DatItemField.Control_Player))
                    control.Player = newItem.Player;

                if (datItemFields.Contains(DatItemField.Control_Buttons))
                    control.Buttons = newItem.Buttons;

                if (datItemFields.Contains(DatItemField.Control_RequiredButtons))
                    control.RequiredButtons = newItem.RequiredButtons;

                if (datItemFields.Contains(DatItemField.Control_Minimum))
                    control.Minimum = newItem.Minimum;

                if (datItemFields.Contains(DatItemField.Control_Maximum))
                    control.Maximum = newItem.Maximum;

                if (datItemFields.Contains(DatItemField.Control_Sensitivity))
                    control.Sensitivity = newItem.Sensitivity;

                if (datItemFields.Contains(DatItemField.Control_KeyDelta))
                    control.KeyDelta = newItem.KeyDelta;

                if (datItemFields.Contains(DatItemField.Control_Reverse))
                    control.Reverse = newItem.Reverse;

                if (datItemFields.Contains(DatItemField.Control_Ways))
                    control.Ways = newItem.Ways;

                if (datItemFields.Contains(DatItemField.Control_Ways2))
                    control.Ways2 = newItem.Ways2;

                if (datItemFields.Contains(DatItemField.Control_Ways3))
                    control.Ways3 = newItem.Ways3;
            }

            #endregion

            #region DataArea

            else if (datItem is DataArea dataArea)
            {
                DataArea newItem = repDatItem as DataArea;

                if (datItemFields.Contains(DatItemField.AreaName))
                    dataArea.Name = newItem.Name;

                if (datItemFields.Contains(DatItemField.AreaSize))
                    dataArea.Size = newItem.Size;

                if (datItemFields.Contains(DatItemField.AreaWidth))
                    dataArea.Width = newItem.Width;

                if (datItemFields.Contains(DatItemField.AreaEndianness))
                    dataArea.Endianness = newItem.Endianness;
            }

            #endregion

            #region Device

            else if (datItem is Device device)
            {
                Device newItem = repDatItem as Device;

                if (datItemFields.Contains(DatItemField.DeviceType))
                    device.DeviceType = newItem.DeviceType;

                if (datItemFields.Contains(DatItemField.Tag))
                    device.Tag = newItem.Tag;

                if (datItemFields.Contains(DatItemField.FixedImage))
                    device.FixedImage = newItem.FixedImage;

                if (datItemFields.Contains(DatItemField.Mandatory))
                    device.Mandatory = newItem.Mandatory;

                if (datItemFields.Contains(DatItemField.Interface))
                    device.Interface = newItem.Interface;

                // Instance_* doesn't make sense here
                // since not every instance under the other item
                // can replace every instance under this item

                // Extension_* doesn't make sense here
                // since not every extension under the other item
                // can replace every extension under this item
            }

            #endregion
        
            #region DipSwitch

            else if (datItem is DipSwitch dipSwitch)
            {
                DipSwitch newItem = repDatItem as DipSwitch;

                #region Common

                if (datItemFields.Contains(DatItemField.Tag))
                    dipSwitch.Tag = newItem.Tag;

                if (datItemFields.Contains(DatItemField.Mask))
                    dipSwitch.Mask = newItem.Mask;

                // Condition_* doesn't make sense here
                // since not every condition under the other item
                // can replace every condition under this item

                // Location_* doesn't make sense here
                // since not every location under the other item
                // can replace every location under this item

                // Setting_* doesn't make sense here
                // since not every value under the other item
                // can replace every value under this item

                #endregion

                #region SoftwareList

                if (dipSwitch.PartSpecified && newItem.PartSpecified)
                    ReplaceFields(dipSwitch.Part, newItem.Part, datItemFields);

                #endregion
            }

            #endregion

            #region Disk

            else if (datItem is Disk disk)
            {
                Disk newItem = repDatItem as Disk;

                #region Common

                if (datItemFields.Contains(DatItemField.MD5))
                {
                    if (string.IsNullOrEmpty(disk.MD5) && !string.IsNullOrEmpty(newItem.MD5))
                        disk.MD5 = newItem.MD5;
                }

                if (datItemFields.Contains(DatItemField.SHA1))
                {
                    if (string.IsNullOrEmpty(disk.SHA1) && !string.IsNullOrEmpty(newItem.SHA1))
                        disk.SHA1 = newItem.SHA1;
                }

                if (datItemFields.Contains(DatItemField.Merge))
                    disk.MergeTag = newItem.MergeTag;

                if (datItemFields.Contains(DatItemField.Region))
                    disk.Region = newItem.Region;

                if (datItemFields.Contains(DatItemField.Index))
                    disk.Index = newItem.Index;

                if (datItemFields.Contains(DatItemField.Writable))
                    disk.Writable = newItem.Writable;

                if (datItemFields.Contains(DatItemField.Status))
                    disk.ItemStatus = newItem.ItemStatus;

                if (datItemFields.Contains(DatItemField.Optional))
                    disk.Optional = newItem.Optional;

                #endregion

                #region SoftwareList

                if (disk.DiskAreaSpecified && newItem.DiskAreaSpecified)
                    ReplaceFields(disk.DiskArea, newItem.DiskArea, datItemFields);

                if (disk.PartSpecified && newItem.PartSpecified)
                    ReplaceFields(disk.Part, newItem.Part, datItemFields);

                #endregion
            }

            #endregion

            #region DiskArea

            else if (datItem is DiskArea diskArea)
            {
                DiskArea newItem = repDatItem as DiskArea;

                if (datItemFields.Contains(DatItemField.AreaName))
                    diskArea.Name = newItem.Name;
            }

            #endregion

            #region Display

            else if (datItem is Display display)
            {
                Display newItem = repDatItem as Display;

                if (datItemFields.Contains(DatItemField.Tag))
                    display.Tag = newItem.Tag;

                if (datItemFields.Contains(DatItemField.DisplayType))
                    display.DisplayType = newItem.DisplayType;

                if (datItemFields.Contains(DatItemField.Rotate))
                    display.Rotate = newItem.Rotate;

                if (datItemFields.Contains(DatItemField.FlipX))
                    display.FlipX = newItem.FlipX;

                if (datItemFields.Contains(DatItemField.Width))
                    display.Width = newItem.Width;

                if (datItemFields.Contains(DatItemField.Height))
                    display.Height = newItem.Height;

                if (datItemFields.Contains(DatItemField.Refresh))
                    display.Refresh = newItem.Refresh;

                if (datItemFields.Contains(DatItemField.PixClock))
                    display.PixClock = newItem.PixClock;

                if (datItemFields.Contains(DatItemField.HTotal))
                    display.HTotal = newItem.HTotal;

                if (datItemFields.Contains(DatItemField.HBEnd))
                    display.HBEnd = newItem.HBEnd;

                if (datItemFields.Contains(DatItemField.HBStart))
                    display.HBStart = newItem.HBStart;

                if (datItemFields.Contains(DatItemField.VTotal))
                    display.VTotal = newItem.VTotal;

                if (datItemFields.Contains(DatItemField.VBEnd))
                    display.VBEnd = newItem.VBEnd;

                if (datItemFields.Contains(DatItemField.VBStart))
                    display.VBStart = newItem.VBStart;
            }

            #endregion

            #region Driver

            else if (datItem is Driver driver)
            {
                Driver newItem = repDatItem as Driver;

                if (datItemFields.Contains(DatItemField.SupportStatus))
                    driver.Status = newItem.Status;

                if (datItemFields.Contains(DatItemField.EmulationStatus))
                    driver.Emulation = newItem.Emulation;

                if (datItemFields.Contains(DatItemField.CocktailStatus))
                    driver.Cocktail = newItem.Cocktail;

                if (datItemFields.Contains(DatItemField.SaveStateStatus))
                    driver.SaveState = newItem.SaveState;
            }

            #endregion

            #region Extension

            else if (datItem is Extension extension)
            {
                Extension newItem = repDatItem as Extension;

                if (datItemFields.Contains(DatItemField.Extension_Name))
                    extension.Name = newItem.Name;
            }

            #endregion

            #region Feature

            else if (datItem is Feature feature)
            {
                Feature newItem = repDatItem as Feature;

                if (datItemFields.Contains(DatItemField.FeatureType))
                    feature.Type = newItem.Type;

                if (datItemFields.Contains(DatItemField.FeatureStatus))
                    feature.Status = newItem.Status;

                if (datItemFields.Contains(DatItemField.FeatureOverall))
                    feature.Overall = newItem.Overall;
            }

            #endregion

            #region Info

            else if (datItem is Info info)
            {
                Info newItem = repDatItem as Info;

                if (datItemFields.Contains(DatItemField.Value))
                    info.Value = newItem.Value;
            }

            #endregion

            #region Input

            else if (datItem is Input input)
            {
                Input newItem = repDatItem as Input;

                if (datItemFields.Contains(DatItemField.Service))
                    input.Service = newItem.Service;

                if (datItemFields.Contains(DatItemField.Tilt))
                    input.Tilt = newItem.Tilt;

                if (datItemFields.Contains(DatItemField.Players))
                    input.Players = newItem.Players;

                if (datItemFields.Contains(DatItemField.Coins))
                    input.Coins = newItem.Coins;

                // Control_* doesn't make sense here
                // since not every control under the other item
                // can replace every control under this item
            }

            #endregion

            #region Instance

            else if (datItem is Instance instance)
            {
                Instance newItem = repDatItem as Instance;

                if (datItemFields.Contains(DatItemField.Instance_Name))
                    instance.Name = newItem.Name;

                if (datItemFields.Contains(DatItemField.Instance_BriefName))
                    instance.BriefName = newItem.BriefName;
            }

            #endregion

            #region Location

            else if (datItem is Location location)
            {
                Location newItem = repDatItem as Location;

                if (datItemFields.Contains(DatItemField.Location_Name))
                    location.Name = newItem.Name;

                if (datItemFields.Contains(DatItemField.Location_Number))
                    location.Number = newItem.Number;

                if (datItemFields.Contains(DatItemField.Location_Inverted))
                    location.Inverted = newItem.Inverted;
            }

            #endregion

            #region Media

            else if (datItem is Media media)
            {
                Media newItem = repDatItem as Media;

                if (datItemFields.Contains(DatItemField.MD5))
                {
                    if (string.IsNullOrEmpty(media.MD5) && !string.IsNullOrEmpty(newItem.MD5))
                        media.MD5 = newItem.MD5;
                }

                if (datItemFields.Contains(DatItemField.SHA1))
                {
                    if (string.IsNullOrEmpty(media.SHA1) && !string.IsNullOrEmpty(newItem.SHA1))
                        media.SHA1 = newItem.SHA1;
                }

                if (datItemFields.Contains(DatItemField.SHA256))
                {
                    if (string.IsNullOrEmpty(media.SHA256) && !string.IsNullOrEmpty(newItem.SHA256))
                        media.SHA256 = newItem.SHA256;
                }

                if (datItemFields.Contains(DatItemField.SpamSum))
                {
                    if (string.IsNullOrEmpty(media.SpamSum) && !string.IsNullOrEmpty(newItem.SpamSum))
                        media.SpamSum = newItem.SpamSum;
                }
            }

            #endregion

            #region Part

            else if (datItem is Part part)
            {
                Part newItem = repDatItem as Part;

                if (datItemFields.Contains(DatItemField.Part_Name))
                    part.Name = newItem.Name;

                if (datItemFields.Contains(DatItemField.Part_Interface))
                    part.Interface = newItem.Interface;

                // Part_Feature_* doesn't make sense here
                // since not every part feature under the other item
                // can replace every part feature under this item
            }

            #endregion

            #region PartFeature

            else if (datItem is PartFeature partFeature)
            {
                PartFeature newItem = repDatItem as PartFeature;

                if (datItemFields.Contains(DatItemField.Part_Feature_Name))
                    partFeature.Name = newItem.Name;

                if (datItemFields.Contains(DatItemField.Part_Feature_Value))
                    partFeature.Value = newItem.Value;
            }

            #endregion

            #region Port

            else if (datItem is Port)
            {
                // Analog_* doesn't make sense here
                // since not every analog under the other item
                // can replace every analog under this item
            }

            #endregion

            #region RamOption

            else if (datItem is RamOption ramOption)
            {
                RamOption newItem = repDatItem as RamOption;

                if (datItemFields.Contains(DatItemField.Default))
                    ramOption.Default = newItem.Default;

                if (datItemFields.Contains(DatItemField.Content))
                    ramOption.Content = newItem.Content;
            }

            #endregion

            #region Release

            else if (datItem is Release release)
            {
                Release newItem = repDatItem as Release;

                if (datItemFields.Contains(DatItemField.Region))
                    release.Region = newItem.Region;

                if (datItemFields.Contains(DatItemField.Language))
                    release.Language = newItem.Language;

                if (datItemFields.Contains(DatItemField.Date))
                    release.Date = newItem.Date;

                if (datItemFields.Contains(DatItemField.Default))
                    release.Default = newItem.Default;
            }

            #endregion

            #region Rom

            else if (datItem is Rom rom)
            {
                Rom newItem = repDatItem as Rom;

                #region Common

                if (datItemFields.Contains(DatItemField.Bios))
                    rom.Bios = newItem.Bios;

                if (datItemFields.Contains(DatItemField.Size))
                    rom.Size = newItem.Size;

                if (datItemFields.Contains(DatItemField.CRC))
                {
                    if (string.IsNullOrEmpty(rom.CRC) && !string.IsNullOrEmpty(newItem.CRC))
                        rom.CRC = newItem.CRC;
                }

                if (datItemFields.Contains(DatItemField.MD5))
                {
                    if (string.IsNullOrEmpty(rom.MD5) && !string.IsNullOrEmpty(newItem.MD5))
                        rom.MD5 = newItem.MD5;
                }

                if (datItemFields.Contains(DatItemField.SHA1))
                {
                    if (string.IsNullOrEmpty(rom.SHA1) && !string.IsNullOrEmpty(newItem.SHA1))
                        rom.SHA1 = newItem.SHA1;
                }

                if (datItemFields.Contains(DatItemField.SHA256))
                {
                    if (string.IsNullOrEmpty(rom.SHA256) && !string.IsNullOrEmpty(newItem.SHA256))
                        rom.SHA256 = newItem.SHA256;
                }

                if (datItemFields.Contains(DatItemField.SHA384))
                {
                    if (string.IsNullOrEmpty(rom.SHA384) && !string.IsNullOrEmpty(newItem.SHA384))
                        rom.SHA384 = newItem.SHA384;
                }

                if (datItemFields.Contains(DatItemField.SHA512))
                {
                    if (string.IsNullOrEmpty(rom.SHA512) && !string.IsNullOrEmpty(newItem.SHA512))
                        rom.SHA512 = newItem.SHA512;
                }

                if (datItemFields.Contains(DatItemField.SpamSum))
                {
                    if (string.IsNullOrEmpty(rom.SpamSum) && !string.IsNullOrEmpty(newItem.SpamSum))
                        rom.SpamSum = newItem.SpamSum;
                }

                if (datItemFields.Contains(DatItemField.Merge))
                    rom.MergeTag = newItem.MergeTag;

                if (datItemFields.Contains(DatItemField.Region))
                    rom.Region = newItem.Region;

                if (datItemFields.Contains(DatItemField.Offset))
                    rom.Offset = newItem.Offset;

                if (datItemFields.Contains(DatItemField.Date))
                    rom.Date = newItem.Date;

                if (datItemFields.Contains(DatItemField.Status))
                    rom.ItemStatus = newItem.ItemStatus;

                if (datItemFields.Contains(DatItemField.Optional))
                    rom.Optional = newItem.Optional;

                if (datItemFields.Contains(DatItemField.Inverted))
                    rom.Inverted = newItem.Inverted;

                #endregion

                #region AttractMode

                if (datItemFields.Contains(DatItemField.AltName))
                    rom.AltName = newItem.AltName;

                if (datItemFields.Contains(DatItemField.AltTitle))
                    rom.AltTitle = newItem.AltTitle;

                #endregion

                #region OpenMSX

                if (datItemFields.Contains(DatItemField.Original))
                    rom.Original = newItem.Original;

                if (datItemFields.Contains(DatItemField.OpenMSXSubType))
                    rom.OpenMSXSubType = newItem.OpenMSXSubType;

                if (datItemFields.Contains(DatItemField.OpenMSXType))
                    rom.OpenMSXType = newItem.OpenMSXType;

                if (datItemFields.Contains(DatItemField.Remark))
                    rom.Remark = newItem.Remark;

                if (datItemFields.Contains(DatItemField.Boot))
                    rom.Boot = newItem.Boot;

                #endregion

                #region SoftwareList

                if (datItemFields.Contains(DatItemField.LoadFlag))
                    rom.LoadFlag = newItem.LoadFlag;

                if (datItemFields.Contains(DatItemField.Value))
                    rom.Value = newItem.Value;

                if (rom.DataAreaSpecified && newItem.DataAreaSpecified)
                    ReplaceFields(rom.DataArea, newItem.DataArea, datItemFields);

                if (rom.PartSpecified && newItem.PartSpecified)
                    ReplaceFields(rom.Part, newItem.Part, datItemFields);

                #endregion
            }

            #endregion

            #region Setting

            else if (datItem is Setting setting)
            {
                Setting newItem = repDatItem as Setting;

                if (datItemFields.Contains(DatItemField.Setting_Name))
                    setting.Name = newItem.Name;

                if (datItemFields.Contains(DatItemField.Setting_Value))
                    setting.Value = newItem.Value;

                if (datItemFields.Contains(DatItemField.Setting_Default))
                    setting.Default = newItem.Default;

                // Condition_* doesn't make sense here
                // since not every condition under the other item
                // can replace every condition under this item
            }

            #endregion

            #region SharedFeature

            else if (datItem is SharedFeature sharedFeature)
            {
                SharedFeature newItem = repDatItem as SharedFeature;

                if (datItemFields.Contains(DatItemField.Value))
                    sharedFeature.Value = newItem.Value;
            }

            #endregion

            #region Slot

            else if (datItem is Slot)
            {
                // SlotOption_* doesn't make sense here
                // since not every slot option under the other item
                // can replace every slot option under this item
            }

            #endregion

            #region SlotOption

            else if (datItem is SlotOption slotOption)
            {
                SlotOption newItem = repDatItem as SlotOption;

                if (datItemFields.Contains(DatItemField.SlotOption_Name))
                    slotOption.Name = newItem.Name;

                if (datItemFields.Contains(DatItemField.SlotOption_DeviceName))
                    slotOption.DeviceName = newItem.DeviceName;

                if (datItemFields.Contains(DatItemField.SlotOption_Default))
                    slotOption.Default = newItem.Default;
            }

            #endregion

            #region SoftwareList

            else if (datItem is SoftwareList softwareList)
            {
                SoftwareList newItem = repDatItem as SoftwareList;

                if (datItemFields.Contains(DatItemField.SoftwareListStatus))
                    softwareList.Status = newItem.Status;

                if (datItemFields.Contains(DatItemField.Filter))
                    softwareList.Filter = newItem.Filter;
            }

            #endregion

            #region Sound

            else if (datItem is Sound sound)
            {
                Sound newItem = repDatItem as Sound;

                if (datItemFields.Contains(DatItemField.Channels))
                    sound.Channels = newItem.Channels;
            }

            #endregion
        }

        /// <summary>
        /// Replace fields with given values
        /// </summary>
        /// <param name="machine">Machine to replace fields in</param>
        /// <param name="repMachine">Machine to pull new information from</param>
        /// <param name="machineFields">List of fields representing what should be updated</param>
        /// <param name="onlySame">True if descriptions should only be replaced if the game name is the same, false otherwise</param>
        public static void ReplaceFields(Machine machine, Machine repMachine, List<MachineField> machineFields, bool onlySame)
        {
            #region Common

            if (machineFields.Contains(MachineField.Name))
                machine.Name = repMachine.Name;

            if (machineFields.Contains(MachineField.Comment))
                machine.Comment = repMachine.Comment;

            if (machineFields.Contains(MachineField.Description))
            {
                if (!onlySame || (onlySame && machine.Name == machine.Description))
                    machine.Description = repMachine.Description;
            }

            if (machineFields.Contains(MachineField.Year))
                machine.Year = repMachine.Year;

            if (machineFields.Contains(MachineField.Manufacturer))
                machine.Manufacturer = repMachine.Manufacturer;

            if (machineFields.Contains(MachineField.Publisher))
                machine.Publisher = repMachine.Publisher;

            if (machineFields.Contains(MachineField.Category))
                machine.Category = repMachine.Category;

            if (machineFields.Contains(MachineField.RomOf))
                machine.RomOf = repMachine.RomOf;

            if (machineFields.Contains(MachineField.CloneOf))
                machine.CloneOf = repMachine.CloneOf;

            if (machineFields.Contains(MachineField.SampleOf))
                machine.SampleOf = repMachine.SampleOf;

            if (machineFields.Contains(MachineField.Type))
                machine.MachineType = repMachine.MachineType;

            #endregion

            #region AttractMode

            if (machineFields.Contains(MachineField.Players))
                machine.Players = repMachine.Players;

            if (machineFields.Contains(MachineField.Rotation))
                machine.Rotation = repMachine.Rotation;

            if (machineFields.Contains(MachineField.Control))
                machine.Control = repMachine.Control;

            if (machineFields.Contains(MachineField.Status))
                machine.Status = repMachine.Status;

            if (machineFields.Contains(MachineField.DisplayCount))
                machine.DisplayCount = repMachine.DisplayCount;

            if (machineFields.Contains(MachineField.DisplayType))
                machine.DisplayType = repMachine.DisplayType;

            if (machineFields.Contains(MachineField.Buttons))
                machine.Buttons = repMachine.Buttons;

            #endregion

            #region ListXML

            if (machineFields.Contains(MachineField.History))
                machine.History = repMachine.History;

            if (machineFields.Contains(MachineField.SourceFile))
                machine.SourceFile = repMachine.SourceFile;

            if (machineFields.Contains(MachineField.Runnable))
                machine.Runnable = repMachine.Runnable;

            #endregion

            #region Logiqx

            if (machineFields.Contains(MachineField.Board))
                machine.Board = repMachine.Board;

            if (machineFields.Contains(MachineField.RebuildTo))
                machine.RebuildTo = repMachine.RebuildTo;

            #endregion

            #region Logiqx EmuArc

            if (machineFields.Contains(MachineField.TitleID))
                machine.TitleID = repMachine.TitleID;

            if (machineFields.Contains(MachineField.Developer))
                machine.Developer = repMachine.Developer;

            if (machineFields.Contains(MachineField.Genre))
                machine.Genre = repMachine.Genre;

            if (machineFields.Contains(MachineField.Subgenre))
                machine.Subgenre = repMachine.Subgenre;

            if (machineFields.Contains(MachineField.Ratings))
                machine.Ratings = repMachine.Ratings;

            if (machineFields.Contains(MachineField.Score))
                machine.Score = repMachine.Score;

            if (machineFields.Contains(MachineField.Enabled))
                machine.Enabled = repMachine.Enabled;

            if (machineFields.Contains(MachineField.CRC))
                machine.Crc = repMachine.Crc;

            if (machineFields.Contains(MachineField.RelatedTo))
                machine.RelatedTo = repMachine.RelatedTo;

            #endregion

            #region OpenMSX

            if (machineFields.Contains(MachineField.GenMSXID))
                machine.GenMSXID = repMachine.GenMSXID;

            if (machineFields.Contains(MachineField.System))
                machine.System = repMachine.System;

            if (machineFields.Contains(MachineField.Country))
                machine.Country = repMachine.Country;

            #endregion

            #region SoftwareList

            if (machineFields.Contains(MachineField.Supported))
                machine.Supported = repMachine.Supported;

            #endregion
        }

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="datItem">DatItem to set fields on</param>
        /// <param name="datItemMappings">DatItem mappings dictionary</param>
        /// <param name="machineMappings">Machine mappings dictionary</param>
        /// <param name="sub">True if this is a subitem, false otherwise</param>
        public static void SetFields(
            DatItem datItem,
            Dictionary<DatItemField, string> datItemMappings,
            Dictionary<MachineField, string> machineMappings,
            bool sub = false)
        {
            if (datItem == null)
                return;

            #region Common

            if (machineMappings != null && datItem.Machine != null)
                SetFields(datItem.Machine, machineMappings);

            if (datItemMappings == null)
                return;

            if (datItemMappings.Keys.Contains(DatItemField.Name))
                datItem.SetName(datItemMappings[DatItemField.Name]);

            #endregion

            #region Adjuster

            if (datItem is Adjuster adjuster)
            {
                if (datItemMappings.Keys.Contains(DatItemField.Default))
                    adjuster.Default = datItemMappings[DatItemField.Default].AsYesNo();

                // Field.DatItem_Conditions does not apply here
                if (adjuster.ConditionsSpecified)
                {
                    foreach (Condition subCondition in adjuster.Conditions)
                    {
                        SetFields(subCondition, datItemMappings, machineMappings, true);
                    }
                }
            }

            #endregion

            #region Analog

            else if (datItem is Analog analog)
            {
                if (datItemMappings.Keys.Contains(DatItemField.Analog_Mask))
                    analog.Mask = datItemMappings[DatItemField.Analog_Mask];
            }

            #endregion

            #region BiosSet

            else if (datItem is BiosSet biosSet)
            {
                if (datItemMappings.Keys.Contains(DatItemField.Description))
                    biosSet.Description = datItemMappings[DatItemField.Description];

                if (datItemMappings.Keys.Contains(DatItemField.Default))
                    biosSet.Default = datItemMappings[DatItemField.Default].AsYesNo();
            }

            #endregion

            #region Chip

            else if (datItem is Chip chip)
            {
                if (datItemMappings.Keys.Contains(DatItemField.Tag))
                    chip.Tag = datItemMappings[DatItemField.Tag];

                if (datItemMappings.Keys.Contains(DatItemField.ChipType))
                    chip.ChipType = datItemMappings[DatItemField.ChipType].AsChipType();

                if (datItemMappings.Keys.Contains(DatItemField.Clock))
                    chip.Clock = Utilities.CleanLong(datItemMappings[DatItemField.Clock]);
            }

            #endregion

            #region Condition

            else if (datItem is Condition condition)
            {
                if (sub)
                {
                    if (datItemMappings.Keys.Contains(DatItemField.Condition_Tag))
                        condition.Tag = datItemMappings[DatItemField.Condition_Tag];

                    if (datItemMappings.Keys.Contains(DatItemField.Condition_Mask))
                        condition.Mask = datItemMappings[DatItemField.Condition_Mask];

                    if (datItemMappings.Keys.Contains(DatItemField.Condition_Relation))
                        condition.Relation = datItemMappings[DatItemField.Condition_Relation].AsRelation();

                    if (datItemMappings.Keys.Contains(DatItemField.Condition_Value))
                        condition.Value = datItemMappings[DatItemField.Condition_Value];
                }
                else
                {
                    if (datItemMappings.Keys.Contains(DatItemField.Tag))
                        condition.Tag = datItemMappings[DatItemField.Tag];

                    if (datItemMappings.Keys.Contains(DatItemField.Mask))
                        condition.Mask = datItemMappings[DatItemField.Mask];

                    if (datItemMappings.Keys.Contains(DatItemField.Relation))
                        condition.Relation = datItemMappings[DatItemField.Relation].AsRelation();

                    if (datItemMappings.Keys.Contains(DatItemField.Value))
                        condition.Value = datItemMappings[DatItemField.Value];
                }
            }

            #endregion

            #region Configuration

            else if (datItem is Configuration configuration)
            {
                if (datItemMappings.Keys.Contains(DatItemField.Tag))
                    configuration.Tag = datItemMappings[DatItemField.Tag];

                if (datItemMappings.Keys.Contains(DatItemField.Mask))
                    configuration.Mask = datItemMappings[DatItemField.Mask];

                if (configuration.ConditionsSpecified)
                {
                    foreach (Condition subCondition in configuration.Conditions)
                    {
                        SetFields(subCondition, datItemMappings, machineMappings, true);
                    }
                }

                if (configuration.LocationsSpecified)
                {
                    foreach (Location subLocation in configuration.Locations)
                    {
                        SetFields(subLocation, datItemMappings, machineMappings);
                    }
                }

                if (configuration.SettingsSpecified)
                {
                    foreach (Setting subSetting in configuration.Settings)
                    {
                        SetFields(subSetting, datItemMappings, machineMappings);
                    }
                }
            }

            #endregion

            #region Control

            else if (datItem is Control control)
            {
                if (datItemMappings.Keys.Contains(DatItemField.Control_Type))
                    control.ControlType = datItemMappings[DatItemField.Control_Type].AsControlType();

                if (datItemMappings.Keys.Contains(DatItemField.Control_Player))
                    control.Player = Utilities.CleanLong(datItemMappings[DatItemField.Control_Player]);

                if (datItemMappings.Keys.Contains(DatItemField.Control_Buttons))
                    control.Buttons = Utilities.CleanLong(datItemMappings[DatItemField.Control_Buttons]);

                if (datItemMappings.Keys.Contains(DatItemField.Control_RequiredButtons))
                    control.RequiredButtons = Utilities.CleanLong(datItemMappings[DatItemField.Control_RequiredButtons]);

                if (datItemMappings.Keys.Contains(DatItemField.Control_Minimum))
                    control.Minimum = Utilities.CleanLong(datItemMappings[DatItemField.Control_Minimum]);

                if (datItemMappings.Keys.Contains(DatItemField.Control_Maximum))
                    control.Maximum = Utilities.CleanLong(datItemMappings[DatItemField.Control_Maximum]);

                if (datItemMappings.Keys.Contains(DatItemField.Control_Sensitivity))
                    control.Sensitivity = Utilities.CleanLong(datItemMappings[DatItemField.Control_Sensitivity]);

                if (datItemMappings.Keys.Contains(DatItemField.Control_KeyDelta))
                    control.KeyDelta = Utilities.CleanLong(datItemMappings[DatItemField.Control_KeyDelta]);

                if (datItemMappings.Keys.Contains(DatItemField.Control_Reverse))
                    control.Reverse = datItemMappings[DatItemField.Control_Reverse].AsYesNo();

                if (datItemMappings.Keys.Contains(DatItemField.Control_Ways))
                    control.Ways = datItemMappings[DatItemField.Control_Ways];

                if (datItemMappings.Keys.Contains(DatItemField.Control_Ways2))
                    control.Ways2 = datItemMappings[DatItemField.Control_Ways2];

                if (datItemMappings.Keys.Contains(DatItemField.Control_Ways3))
                    control.Ways3 = datItemMappings[DatItemField.Control_Ways3];
            }

            #endregion

            #region DataArea

            else if (datItem is DataArea dataArea)
            {
                if (datItemMappings.Keys.Contains(DatItemField.AreaSize))
                    dataArea.Size = Utilities.CleanLong(datItemMappings[DatItemField.AreaSize]);

                if (datItemMappings.Keys.Contains(DatItemField.AreaWidth))
                    dataArea.Width = Utilities.CleanLong(datItemMappings[DatItemField.AreaWidth]);

                if (datItemMappings.Keys.Contains(DatItemField.AreaEndianness))
                    dataArea.Endianness = datItemMappings[DatItemField.AreaEndianness].AsEndianness();
            }

            #endregion

            #region Device

            else if (datItem is Device device)
            {
                if (datItemMappings.Keys.Contains(DatItemField.DeviceType))
                    device.DeviceType = datItemMappings[DatItemField.DeviceType].AsDeviceType();

                if (datItemMappings.Keys.Contains(DatItemField.Tag))
                    device.Tag = datItemMappings[DatItemField.Tag];

                if (datItemMappings.Keys.Contains(DatItemField.FixedImage))
                    device.FixedImage = datItemMappings[DatItemField.FixedImage];

                if (datItemMappings.Keys.Contains(DatItemField.Mandatory))
                    device.Mandatory = Utilities.CleanLong(datItemMappings[DatItemField.Mandatory]);

                if (datItemMappings.Keys.Contains(DatItemField.Interface))
                    device.Interface = datItemMappings[DatItemField.Interface];

                if (device.InstancesSpecified)
                {
                    foreach (Instance subInstance in device.Instances)
                    {
                        SetFields(subInstance, datItemMappings, machineMappings);
                    }
                }

                if (device.ExtensionsSpecified)
                {
                    foreach (Extension subExtension in device.Extensions)
                    {
                        SetFields(subExtension, datItemMappings, machineMappings);
                    }
                }
            }

            #endregion
        
            #region DipSwitch

            else if (datItem is DipSwitch dipSwitch)
            {
                #region Common

                if (datItemMappings.Keys.Contains(DatItemField.Tag))
                    dipSwitch.Tag = datItemMappings[DatItemField.Tag];

                if (datItemMappings.Keys.Contains(DatItemField.Mask))
                    dipSwitch.Mask = datItemMappings[DatItemField.Mask];

                if (dipSwitch.ConditionsSpecified)
                {
                    foreach (Condition subCondition in dipSwitch.Conditions)
                    {
                        SetFields(subCondition, datItemMappings, machineMappings, true);
                    }
                }

                if (dipSwitch.LocationsSpecified)
                {
                    foreach (Location subLocation in dipSwitch.Locations)
                    {
                        SetFields(subLocation, datItemMappings, machineMappings);
                    }
                }

                if (dipSwitch.ValuesSpecified)
                {
                    foreach (Setting subValue in dipSwitch.Values)
                    {
                        SetFields(subValue, datItemMappings, machineMappings);
                    }
                }

                #endregion

                #region SoftwareList

                // Handle Part-specific fields
                if (dipSwitch.Part == null)
                    dipSwitch.Part = new Part();

                SetFields(dipSwitch.Part, datItemMappings, machineMappings);

                #endregion
            }

            #endregion

            #region Disk

            else if (datItem is Disk disk)
            {
                #region Common
                
                if (datItemMappings.Keys.Contains(DatItemField.MD5))
                    disk.MD5 = datItemMappings[DatItemField.MD5];

                if (datItemMappings.Keys.Contains(DatItemField.SHA1))
                    disk.SHA1 = datItemMappings[DatItemField.SHA1];

                if (datItemMappings.Keys.Contains(DatItemField.Merge))
                    disk.MergeTag = datItemMappings[DatItemField.Merge];

                if (datItemMappings.Keys.Contains(DatItemField.Region))
                    disk.Region = datItemMappings[DatItemField.Region];

                if (datItemMappings.Keys.Contains(DatItemField.Index))
                    disk.Index = datItemMappings[DatItemField.Index];

                if (datItemMappings.Keys.Contains(DatItemField.Writable))
                    disk.Writable = datItemMappings[DatItemField.Writable].AsYesNo();

                if (datItemMappings.Keys.Contains(DatItemField.Status))
                    disk.ItemStatus = datItemMappings[DatItemField.Status].AsItemStatus();

                if (datItemMappings.Keys.Contains(DatItemField.Optional))
                    disk.Optional = datItemMappings[DatItemField.Optional].AsYesNo();

                #endregion

                #region SoftwareList

                if (disk.DiskArea == null)
                    disk.DiskArea = new DiskArea();

                SetFields(disk.DiskArea, datItemMappings, machineMappings);

                if (disk.Part == null)
                    disk.Part = new Part();

                SetFields(disk.Part, datItemMappings, machineMappings);

                #endregion
            }

            #endregion

            #region Display

            else if (datItem is Display display)
            {
                if (datItemMappings.Keys.Contains(DatItemField.Tag))
                    display.Tag = datItemMappings[DatItemField.Tag];

                if (datItemMappings.Keys.Contains(DatItemField.DisplayType))
                    display.DisplayType = datItemMappings[DatItemField.DisplayType].AsDisplayType();

                if (datItemMappings.Keys.Contains(DatItemField.Rotate))
                    display.Rotate = Utilities.CleanLong(datItemMappings[DatItemField.Rotate]);

                if (datItemMappings.Keys.Contains(DatItemField.FlipX))
                    display.FlipX = datItemMappings[DatItemField.FlipX].AsYesNo();

                if (datItemMappings.Keys.Contains(DatItemField.Width))
                    display.Width = Utilities.CleanLong(datItemMappings[DatItemField.Width]);

                if (datItemMappings.Keys.Contains(DatItemField.Height))
                    display.Height = Utilities.CleanLong(datItemMappings[DatItemField.Height]);

                if (datItemMappings.Keys.Contains(DatItemField.Refresh))
                {
                    if (Double.TryParse(datItemMappings[DatItemField.Refresh], out double refresh))
                        display.Refresh = refresh;
                }

                if (datItemMappings.Keys.Contains(DatItemField.PixClock))
                    display.PixClock = Utilities.CleanLong(datItemMappings[DatItemField.PixClock]);

                if (datItemMappings.Keys.Contains(DatItemField.HTotal))
                    display.HTotal = Utilities.CleanLong(datItemMappings[DatItemField.HTotal]);

                if (datItemMappings.Keys.Contains(DatItemField.HBEnd))
                    display.HBEnd = Utilities.CleanLong(datItemMappings[DatItemField.HBEnd]);

                if (datItemMappings.Keys.Contains(DatItemField.HBStart))
                    display.HBStart = Utilities.CleanLong(datItemMappings[DatItemField.HBStart]);

                if (datItemMappings.Keys.Contains(DatItemField.VTotal))
                    display.VTotal = Utilities.CleanLong(datItemMappings[DatItemField.VTotal]);

                if (datItemMappings.Keys.Contains(DatItemField.VBEnd))
                    display.VBEnd = Utilities.CleanLong(datItemMappings[DatItemField.VBEnd]);

                if (datItemMappings.Keys.Contains(DatItemField.VBStart))
                    display.VBStart = Utilities.CleanLong(datItemMappings[DatItemField.VBStart]);
            }

            #endregion

            #region Driver

            else if (datItem is Driver driver)
            {
                if (datItemMappings.Keys.Contains(DatItemField.SupportStatus))
                    driver.Status = datItemMappings[DatItemField.SupportStatus].AsSupportStatus();

                if (datItemMappings.Keys.Contains(DatItemField.EmulationStatus))
                    driver.Emulation = datItemMappings[DatItemField.EmulationStatus].AsSupportStatus();

                if (datItemMappings.Keys.Contains(DatItemField.CocktailStatus))
                    driver.Cocktail = datItemMappings[DatItemField.CocktailStatus].AsSupportStatus();

                if (datItemMappings.Keys.Contains(DatItemField.SaveStateStatus))
                    driver.SaveState = datItemMappings[DatItemField.SaveStateStatus].AsSupported();
            }

            #endregion

            #region Feature

            else if (datItem is Feature feature)
            {
                if (datItemMappings.Keys.Contains(DatItemField.FeatureType))
                    feature.Type = datItemMappings[DatItemField.FeatureType].AsFeatureType();

                if (datItemMappings.Keys.Contains(DatItemField.FeatureStatus))
                    feature.Status = datItemMappings[DatItemField.FeatureStatus].AsFeatureStatus();

                if (datItemMappings.Keys.Contains(DatItemField.FeatureOverall))
                    feature.Overall = datItemMappings[DatItemField.FeatureOverall].AsFeatureStatus();
            }

            #endregion

            #region Info

            else if (datItem is Info info)
            {
                if (datItemMappings.Keys.Contains(DatItemField.Value))
                    info.Value = datItemMappings[DatItemField.Value];
            }

            #endregion

            #region Input

            else if (datItem is Input input)
            {
                if (datItemMappings.Keys.Contains(DatItemField.Service))
                    input.Service = datItemMappings[DatItemField.Service].AsYesNo();

                if (datItemMappings.Keys.Contains(DatItemField.Tilt))
                    input.Tilt = datItemMappings[DatItemField.Tilt].AsYesNo();

                if (datItemMappings.Keys.Contains(DatItemField.Players))
                    input.Players = Utilities.CleanLong(datItemMappings[DatItemField.Players]);

                if (datItemMappings.Keys.Contains(DatItemField.Coins))
                    input.Coins = Utilities.CleanLong(datItemMappings[DatItemField.Coins]);

                if (input.ControlsSpecified)
                {
                    foreach (Control subControl in input.Controls)
                    {
                        SetFields(subControl, datItemMappings, machineMappings);
                    }
                }
            }

            #endregion

            #region Instance

            else if (datItem is Instance instance)
            {
                if (datItemMappings.Keys.Contains(DatItemField.Instance_BriefName))
                    instance.BriefName = datItemMappings[DatItemField.Instance_BriefName];
            }

            #endregion

            #region Location

            else if (datItem is Location location)
            {
                if (datItemMappings.Keys.Contains(DatItemField.Location_Number))
                    location.Number = Utilities.CleanLong(datItemMappings[DatItemField.Location_Number]);

                if (datItemMappings.Keys.Contains(DatItemField.Location_Inverted))
                    location.Inverted = datItemMappings[DatItemField.Location_Inverted].AsYesNo();
            }

            #endregion

            #region Media

            else if (datItem is Media media)
            {
                if (datItemMappings.Keys.Contains(DatItemField.MD5))
                    media.MD5 = datItemMappings[DatItemField.MD5];

                if (datItemMappings.Keys.Contains(DatItemField.SHA1))
                    media.SHA1 = datItemMappings[DatItemField.SHA1];

                if (datItemMappings.Keys.Contains(DatItemField.SHA256))
                    media.SHA256 = datItemMappings[DatItemField.SHA256];

                if (datItemMappings.Keys.Contains(DatItemField.SpamSum))
                    media.SpamSum = datItemMappings[DatItemField.SpamSum];
            }

            #endregion

            #region Part

            else if (datItem is Part part)
            {
                if (datItemMappings.Keys.Contains(DatItemField.Part_Name))
                    part.Name = datItemMappings[DatItemField.Part_Name];

                if (datItemMappings.Keys.Contains(DatItemField.Part_Interface))
                    part.Interface = datItemMappings[DatItemField.Part_Interface];

                if (part.FeaturesSpecified)
                {
                    foreach (PartFeature subPartFeature in part.Features)
                    {
                        SetFields(subPartFeature, datItemMappings, machineMappings);
                    }
                }
            }

            #endregion

            #region PartFeature

            else if (datItem is PartFeature partFeature)
            {
                if (datItemMappings.Keys.Contains(DatItemField.Part_Feature_Name))
                    partFeature.Name = datItemMappings[DatItemField.Part_Feature_Name];

                if (datItemMappings.Keys.Contains(DatItemField.Part_Feature_Value))
                    partFeature.Value = datItemMappings[DatItemField.Part_Feature_Value];
            }

            #endregion

            #region Port

            else if (datItem is Port port)
            {
                if (datItemMappings.Keys.Contains(DatItemField.Tag))
                    port.Tag = datItemMappings[DatItemField.Tag];

                if (port.AnalogsSpecified)
                {
                    foreach (Analog subAnalog in port.Analogs)
                    {
                        SetFields(subAnalog, datItemMappings, machineMappings);
                    }
                }
            }

            #endregion

            #region RamOption

            else if (datItem is RamOption ramOption)
            {
                if (datItemMappings.Keys.Contains(DatItemField.Default))
                    ramOption.Default = datItemMappings[DatItemField.Default].AsYesNo();

                if (datItemMappings.Keys.Contains(DatItemField.Content))
                    ramOption.Content = datItemMappings[DatItemField.Content];
            }

            #endregion

            #region Release

            else if (datItem is Release release)
            {
                if (datItemMappings.Keys.Contains(DatItemField.Region))
                    release.Region = datItemMappings[DatItemField.Region];

                if (datItemMappings.Keys.Contains(DatItemField.Language))
                    release.Language = datItemMappings[DatItemField.Language];

                if (datItemMappings.Keys.Contains(DatItemField.Date))
                    release.Date = datItemMappings[DatItemField.Date];

                if (datItemMappings.Keys.Contains(DatItemField.Default))
                    release.Default = datItemMappings[DatItemField.Default].AsYesNo();
            }

            #endregion

            #region Rom

            else if (datItem is Rom rom)
            {
                #region Common

                if (datItemMappings.Keys.Contains(DatItemField.Bios))
                    rom.Bios = datItemMappings[DatItemField.Bios];

                if (datItemMappings.Keys.Contains(DatItemField.Size))
                    rom.Size = Utilities.CleanLong(datItemMappings[DatItemField.Size]);

                if (datItemMappings.Keys.Contains(DatItemField.CRC))
                    rom.CRC = datItemMappings[DatItemField.CRC];

                if (datItemMappings.Keys.Contains(DatItemField.MD5))
                    rom.MD5 = datItemMappings[DatItemField.MD5];

                if (datItemMappings.Keys.Contains(DatItemField.SHA1))
                    rom.SHA1 = datItemMappings[DatItemField.SHA1];

                if (datItemMappings.Keys.Contains(DatItemField.SHA256))
                    rom.SHA256 = datItemMappings[DatItemField.SHA256];

                if (datItemMappings.Keys.Contains(DatItemField.SHA384))
                    rom.SHA384 = datItemMappings[DatItemField.SHA384];

                if (datItemMappings.Keys.Contains(DatItemField.SHA512))
                    rom.SHA512 = datItemMappings[DatItemField.SHA512];

                if (datItemMappings.Keys.Contains(DatItemField.SpamSum))
                    rom.SpamSum = datItemMappings[DatItemField.SpamSum];

                if (datItemMappings.Keys.Contains(DatItemField.Merge))
                    rom.MergeTag = datItemMappings[DatItemField.Merge];

                if (datItemMappings.Keys.Contains(DatItemField.Region))
                    rom.Region = datItemMappings[DatItemField.Region];

                if (datItemMappings.Keys.Contains(DatItemField.Offset))
                    rom.Offset = datItemMappings[DatItemField.Offset];

                if (datItemMappings.Keys.Contains(DatItemField.Date))
                    rom.Date = datItemMappings[DatItemField.Date];

                if (datItemMappings.Keys.Contains(DatItemField.Status))
                    rom.ItemStatus = datItemMappings[DatItemField.Status].AsItemStatus();

                if (datItemMappings.Keys.Contains(DatItemField.Optional))
                    rom.Optional = datItemMappings[DatItemField.Optional].AsYesNo();

                if (datItemMappings.Keys.Contains(DatItemField.Inverted))
                    rom.Inverted = datItemMappings[DatItemField.Optional].AsYesNo();

                #endregion

                #region AttractMode

                if (datItemMappings.Keys.Contains(DatItemField.AltName))
                    rom.AltName = datItemMappings[DatItemField.AltName];

                if (datItemMappings.Keys.Contains(DatItemField.AltTitle))
                    rom.AltTitle = datItemMappings[DatItemField.AltTitle];

                #endregion

                #region OpenMSX

                if (datItemMappings.Keys.Contains(DatItemField.Original))
                    rom.Original = new Original() { Content = datItemMappings[DatItemField.Original] };

                if (datItemMappings.Keys.Contains(DatItemField.OpenMSXSubType))
                    rom.OpenMSXSubType = datItemMappings[DatItemField.OpenMSXSubType].AsOpenMSXSubType();

                if (datItemMappings.Keys.Contains(DatItemField.OpenMSXType))
                    rom.OpenMSXType = datItemMappings[DatItemField.OpenMSXType];

                if (datItemMappings.Keys.Contains(DatItemField.Remark))
                    rom.Remark = datItemMappings[DatItemField.Remark];

                if (datItemMappings.Keys.Contains(DatItemField.Boot))
                    rom.Boot = datItemMappings[DatItemField.Boot];

                #endregion

                #region SoftwareList

                if (datItemMappings.Keys.Contains(DatItemField.LoadFlag))
                    rom.LoadFlag = datItemMappings[DatItemField.LoadFlag].AsLoadFlag();

                if (datItemMappings.Keys.Contains(DatItemField.Value))
                    rom.Value = datItemMappings[DatItemField.Value];

                if (rom.DataArea == null)
                    rom.DataArea = new DataArea();

                SetFields(rom.DataArea, datItemMappings, machineMappings);

                if (rom.Part == null)
                    rom.Part = new Part();

                SetFields(rom.Part, datItemMappings, machineMappings);

                #endregion
            }

            #endregion

            #region Setting

            else if (datItem is Setting setting)
            {
                if (datItemMappings.Keys.Contains(DatItemField.Setting_Name))
                    setting.Name = datItemMappings[DatItemField.Setting_Name];

                if (datItemMappings.Keys.Contains(DatItemField.Setting_Value))
                    setting.Value = datItemMappings[DatItemField.Setting_Value];

                if (datItemMappings.Keys.Contains(DatItemField.Setting_Default))
                    setting.Default = datItemMappings[DatItemField.Setting_Default].AsYesNo();

                if (setting.ConditionsSpecified)
                {
                    foreach (Condition subCondition in setting.Conditions)
                    {
                        SetFields(subCondition, datItemMappings, machineMappings, true);
                    }
                }
            }

            #endregion

            #region SharedFeature

            else if (datItem is SharedFeature sharedFeature)
            {
                if (datItemMappings.Keys.Contains(DatItemField.Value))
                    sharedFeature.Value = datItemMappings[DatItemField.Value];
            }

            #endregion

            #region Slot

            else if (datItem is Slot slot)
            {
                if (slot.SlotOptionsSpecified)
                {
                    foreach (SlotOption subSlotOption in slot.SlotOptions)
                    {
                        SetFields(subSlotOption, datItemMappings, machineMappings);
                    }
                }
            }

            #endregion

            #region SlotOption

            else if (datItem is SlotOption slotOption)
            {
                if (datItemMappings.Keys.Contains(DatItemField.SlotOption_Name))
                    slotOption.Name = datItemMappings[DatItemField.SlotOption_Name];

                if (datItemMappings.Keys.Contains(DatItemField.SlotOption_DeviceName))
                    slotOption.DeviceName = datItemMappings[DatItemField.SlotOption_DeviceName];

                if (datItemMappings.Keys.Contains(DatItemField.SlotOption_Default))
                    slotOption.Default = datItemMappings[DatItemField.SlotOption_Default].AsYesNo();
            }

            #endregion

            #region SoftwareList

            else if (datItem is SoftwareList softwareList)
            {
                if (datItemMappings.Keys.Contains(DatItemField.SoftwareListStatus))
                    softwareList.Status = datItemMappings[DatItemField.SoftwareListStatus].AsSoftwareListStatus();

                if (datItemMappings.Keys.Contains(DatItemField.Filter))
                    softwareList.Filter = datItemMappings[DatItemField.Filter];
            }

            #endregion

            #region Sound

            else if (datItem is Sound sound)
            {
                if (datItemMappings.Keys.Contains(DatItemField.Channels))
                    sound.Channels = Utilities.CleanLong(datItemMappings[DatItemField.Channels]);
            }

            #endregion
        }

        /// <summary>
        /// Set fields with given values
        /// </summary>
        /// <param name="machine">Machine to set fields on</param>
        /// <param name="mappings">Mappings dictionary</param>
        public static void SetFields(Machine machine, Dictionary<MachineField, string> mappings)
        {
            if (machine == null || mappings == null)
                return;

            #region Common

            if (mappings.Keys.Contains(MachineField.Name))
                machine.Name = mappings[MachineField.Name];

            if (mappings.Keys.Contains(MachineField.Comment))
                machine.Comment = mappings[MachineField.Comment];

            if (mappings.Keys.Contains(MachineField.Description))
                machine.Description = mappings[MachineField.Description];

            if (mappings.Keys.Contains(MachineField.Year))
                machine.Year = mappings[MachineField.Year];

            if (mappings.Keys.Contains(MachineField.Manufacturer))
                machine.Manufacturer = mappings[MachineField.Manufacturer];

            if (mappings.Keys.Contains(MachineField.Publisher))
                machine.Publisher = mappings[MachineField.Publisher];

            if (mappings.Keys.Contains(MachineField.Category))
                machine.Category = mappings[MachineField.Category];

            if (mappings.Keys.Contains(MachineField.RomOf))
                machine.RomOf = mappings[MachineField.RomOf];

            if (mappings.Keys.Contains(MachineField.CloneOf))
                machine.CloneOf = mappings[MachineField.CloneOf];

            if (mappings.Keys.Contains(MachineField.SampleOf))
                machine.SampleOf = mappings[MachineField.SampleOf];

            if (mappings.Keys.Contains(MachineField.Type))
                machine.MachineType = mappings[MachineField.Type].AsMachineType();

            #endregion

            #region AttractMode

            if (mappings.Keys.Contains(MachineField.Players))
                machine.Players = mappings[MachineField.Players];

            if (mappings.Keys.Contains(MachineField.Rotation))
                machine.Rotation = mappings[MachineField.Rotation];

            if (mappings.Keys.Contains(MachineField.Control))
                machine.Control = mappings[MachineField.Control];

            if (mappings.Keys.Contains(MachineField.Status))
                machine.Status = mappings[MachineField.Status];

            if (mappings.Keys.Contains(MachineField.DisplayCount))
                machine.DisplayCount = mappings[MachineField.DisplayCount];

            if (mappings.Keys.Contains(MachineField.DisplayType))
                machine.DisplayType = mappings[MachineField.DisplayType];

            if (mappings.Keys.Contains(MachineField.Buttons))
                machine.Buttons = mappings[MachineField.Buttons];

            #endregion

            #region ListXML

            if (mappings.Keys.Contains(MachineField.History))
                machine.History = mappings[MachineField.History];

            if (mappings.Keys.Contains(MachineField.SourceFile))
                machine.SourceFile = mappings[MachineField.SourceFile];

            if (mappings.Keys.Contains(MachineField.Runnable))
                machine.Runnable = mappings[MachineField.Runnable].AsRunnable();

            #endregion

            #region Logiqx

            if (mappings.Keys.Contains(MachineField.Board))
                machine.Board = mappings[MachineField.Board];

            if (mappings.Keys.Contains(MachineField.RebuildTo))
                machine.RebuildTo = mappings[MachineField.RebuildTo];

            #endregion

            #region Logiqx EmuArc

            if (mappings.Keys.Contains(MachineField.TitleID))
                machine.TitleID = mappings[MachineField.TitleID];

            if (mappings.Keys.Contains(MachineField.Developer))
                machine.Developer = mappings[MachineField.Developer];

            if (mappings.Keys.Contains(MachineField.Genre))
                machine.Genre = mappings[MachineField.Genre];

            if (mappings.Keys.Contains(MachineField.Subgenre))
                machine.Subgenre = mappings[MachineField.Subgenre];

            if (mappings.Keys.Contains(MachineField.Ratings))
                machine.Ratings = mappings[MachineField.Ratings];

            if (mappings.Keys.Contains(MachineField.Score))
                machine.Score = mappings[MachineField.Score];

            if (mappings.Keys.Contains(MachineField.Enabled))
                machine.Enabled = mappings[MachineField.Enabled];

            if (mappings.Keys.Contains(MachineField.CRC))
                machine.Crc = mappings[MachineField.CRC].AsYesNo();

            if (mappings.Keys.Contains(MachineField.RelatedTo))
                machine.RelatedTo = mappings[MachineField.RelatedTo];

            #endregion

            #region OpenMSX

            if (mappings.Keys.Contains(MachineField.GenMSXID))
                machine.GenMSXID = mappings[MachineField.GenMSXID];

            if (mappings.Keys.Contains(MachineField.System))
                machine.System = mappings[MachineField.System];

            if (mappings.Keys.Contains(MachineField.Country))
                machine.Country = mappings[MachineField.Country];

            #endregion

            #region SoftwareList

            if (mappings.Keys.Contains(MachineField.Supported))
                machine.Supported = mappings[MachineField.Supported].AsSupported();

            #endregion
        }
    
        /// <summary>
        /// Set internal names to match One Rom Per Game (ORPG) logic
        /// </summary>
        /// <param name="datItem">DatItem to run logic on</param>
        public static void SetOneRomPerGame(DatItem datItem)
        {
            if (datItem.GetName() == null)
                return;

            string[] splitname = datItem.GetName().Split('.');
            datItem.Machine.Name += $"/{string.Join(".", splitname.Take(splitname.Length > 1 ? splitname.Length - 1 : 1))}";
            datItem.SetName(Path.GetFileName(datItem.GetName()));
        }   
    }
}