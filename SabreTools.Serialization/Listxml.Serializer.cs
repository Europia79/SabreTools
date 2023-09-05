using System.Linq;
using SabreTools.Models.Listxml;

namespace SabreTools.Serialization
{
    /// <summary>
    /// XML serializer for MAME listxml files
    /// </summary>
    public partial class Listxml : XmlSerializer<Mame>
    {
        #region Internal

        /// <summary>
        /// Convert from <cref="Models.Listxml.M1"/> to <cref="Models.Metadata.MetadataFile"/>
        /// </summary>
        public static Models.Metadata.MetadataFile? ConvertToInternalModel(M1? item)
        {
            if (item == null)
                return null;

            var metadataFile = new Models.Metadata.MetadataFile
            {
                [Models.Metadata.MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(item),
            };

            if (item?.Game != null && item.Game.Any())
            {
                metadataFile[Models.Metadata.MetadataFile.MachineKey] = item.Game
                    .Where(g => g != null)
                    .Select(ConvertMachineToInternalModel)
                    .ToArray();
            }

            return metadataFile;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Mame"/> to <cref="Models.Metadata.MetadataFile"/>
        /// </summary>
        public static Models.Metadata.MetadataFile? ConvertToInternalModel(Mame? item)
        {
            if (item == null)
                return null;

            var metadataFile = new Models.Metadata.MetadataFile
            {
                [Models.Metadata.MetadataFile.HeaderKey] = ConvertHeaderToInternalModel(item),
            };

            if (item?.Game != null && item.Game.Any())
            {
                metadataFile[Models.Metadata.MetadataFile.MachineKey] = item.Game
                    .Where(g => g != null)
                    .Select(ConvertMachineToInternalModel)
                    .ToArray();
            }

            return metadataFile;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.M1"/> to <cref="Models.Metadata.Header"/>
        /// </summary>
        private static Models.Metadata.Header ConvertHeaderToInternalModel(M1 item)
        {
            var header = new Models.Metadata.Header
            {
                [Models.Metadata.Header.VersionKey] = item.Version,
            };
            return header;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Mame"/> to <cref="Models.Metadata.Header"/>
        /// </summary>
        private static Models.Metadata.Header ConvertHeaderToInternalModel(Mame item)
        {
            var header = new Models.Metadata.Header
            {
                [Models.Metadata.Header.BuildKey] = item.Build,
                [Models.Metadata.Header.DebugKey] = item.Debug,
                [Models.Metadata.Header.MameConfigKey] = item.MameConfig,
            };
            return header;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.GameBase"/> to <cref="Models.Metadata.Machine"/>
        /// </summary>
        private static Models.Metadata.Machine ConvertMachineToInternalModel(GameBase item)
        {
            var machine = new Models.Metadata.Machine
            {
                [Models.Metadata.Machine.NameKey] = item.Name,
                [Models.Metadata.Machine.SourceFileKey] = item.SourceFile,
                [Models.Metadata.Machine.IsBiosKey] = item.IsBios,
                [Models.Metadata.Machine.IsDeviceKey] = item.IsDevice,
                [Models.Metadata.Machine.IsMechanicalKey] = item.IsMechanical,
                [Models.Metadata.Machine.RunnableKey] = item.Runnable,
                [Models.Metadata.Machine.CloneOfKey] = item.CloneOf,
                [Models.Metadata.Machine.RomOfKey] = item.RomOf,
                [Models.Metadata.Machine.SampleOfKey] = item.SampleOf,
                [Models.Metadata.Machine.DescriptionKey] = item.Description,
                [Models.Metadata.Machine.YearKey] = item.Year,
                [Models.Metadata.Machine.ManufacturerKey] = item.Manufacturer,
                [Models.Metadata.Machine.HistoryKey] = item.History,
            };

            if (item.BiosSet != null && item.BiosSet.Any())
            {
                machine[Models.Metadata.Machine.BiosSetKey] = item.BiosSet
                    .Where(b => b != null)
                    .Select(ConvertToInternalModel)
                    .ToArray();
            }

            if (item.Rom != null && item.Rom.Any())
            {
                machine[Models.Metadata.Machine.RomKey] = item.Rom
                    .Where(r => r != null)
                    .Select(ConvertToInternalModel)
                    .ToArray();
            }

            if (item.Disk != null && item.Disk.Any())
            {
                machine[Models.Metadata.Machine.DiskKey] = item.Disk
                    .Where(d => d != null)
                    .Select(ConvertToInternalModel)
                    .ToArray();
            }

            if (item.DeviceRef != null && item.DeviceRef.Any())
            {
                machine[Models.Metadata.Machine.DeviceRefKey] = item.DeviceRef
                    .Where(d => d != null)
                    .Select(ConvertToInternalModel)
                    .ToArray();
            }

            if (item.Sample != null && item.Sample.Any())
            {
                machine[Models.Metadata.Machine.SampleKey] = item.Sample
                    .Where(s => s != null)
                    .Select(ConvertToInternalModel)
                    .ToArray();
            }

            if (item.Chip != null && item.Chip.Any())
            {
                machine[Models.Metadata.Machine.ChipKey] = item.Chip
                    .Where(c => c != null)
                    .Select(ConvertToInternalModel)
                    .ToArray();
            }

            if (item.Display != null && item.Display.Any())
            {
                machine[Models.Metadata.Machine.DisplayKey] = item.Display
                    .Where(d => d != null)
                    .Select(ConvertToInternalModel)
                    .ToArray();
            }

            if (item.Video != null && item.Video.Any())
            {
                machine[Models.Metadata.Machine.VideoKey] = item.Video
                    .Where(v => v != null)
                    .Select(ConvertToInternalModel)
                    .ToArray();
            }

            if (item.Sound != null)
                machine[Models.Metadata.Machine.SoundKey] = ConvertToInternalModel(item.Sound);

            if (item.Input != null)
                machine[Models.Metadata.Machine.InputKey] = ConvertToInternalModel(item.Input);

            if (item.DipSwitch != null && item.DipSwitch.Any())
            {
                machine[Models.Metadata.Machine.DipSwitchKey] = item.DipSwitch
                    .Where(d => d != null)
                    .Select(ConvertToInternalModel)
                    .ToArray();
            }

            if (item.Configuration != null && item.Configuration.Any())
            {
                machine[Models.Metadata.Machine.ConfigurationKey] = item.Configuration
                    .Where(c => c != null)
                    .Select(ConvertToInternalModel)
                    .ToArray();
            }

            if (item.Port != null && item.Port.Any())
            {
                machine[Models.Metadata.Machine.PortKey] = item.Port
                    .Where(p => p != null)
                    .Select(ConvertToInternalModel)
                    .ToArray();
            }

            if (item.Adjuster != null && item.Adjuster.Any())
            {
                machine[Models.Metadata.Machine.AdjusterKey] = item.Adjuster
                    .Where(a => a != null)
                    .Select(ConvertToInternalModel)
                    .ToArray();
            }

            if (item.Driver != null)
                machine[Models.Metadata.Machine.DriverKey] = ConvertToInternalModel(item.Driver);

            if (item.Feature != null && item.Feature.Any())
            {
                machine[Models.Metadata.Machine.FeatureKey] = item.Feature
                    .Where(f => f != null)
                    .Select(ConvertToInternalModel)
                    .ToArray();
            }

            if (item.Device != null && item.Device.Any())
            {
                machine[Models.Metadata.Machine.DeviceKey] = item.Device
                    .Where(d => d != null)
                    .Select(ConvertToInternalModel)
                    .ToArray();
            }

            if (item.Slot != null && item.Slot.Any())
            {
                machine[Models.Metadata.Machine.SlotKey] = item.Slot
                    .Where(s => s != null)
                    .Select(ConvertToInternalModel)
                    .ToArray();
            }

            if (item.SoftwareList != null && item.SoftwareList.Any())
            {
                machine[Models.Metadata.Machine.SoftwareListKey] = item.SoftwareList
                    .Where(s => s != null)
                    .Select(ConvertToInternalModel)
                    .ToArray();
            }

            if (item.RamOption != null && item.RamOption.Any())
            {
                machine[Models.Metadata.Machine.RamOptionKey] = item.RamOption
                    .Where(r => r != null)
                    .Select(ConvertToInternalModel)
                    .ToArray();
            }

            return machine;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Adjuster"/> to <cref="Models.Metadata.Adjuster"/>
        /// </summary>
        private static Models.Metadata.Adjuster ConvertToInternalModel(Adjuster item)
        {
            var adjuster = new Models.Metadata.Adjuster
            {
                [Models.Metadata.Adjuster.NameKey] = item.Name,
                [Models.Metadata.Adjuster.DefaultKey] = item.Default,
            };

            if (item.Condition != null)
                adjuster[Models.Metadata.Adjuster.ConditionKey] = ConvertToInternalModel(item.Condition);

            return adjuster;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Analog"/> to <cref="Models.Metadata.Analog"/>
        /// </summary>
        private static Models.Metadata.Analog ConvertToInternalModel(Analog item)
        {
            var analog = new Models.Metadata.Analog
            {
                [Models.Metadata.Analog.MaskKey] = item.Mask,
            };
            return analog;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.BiosSet"/> to <cref="Models.Metadata.BiosSet"/>
        /// </summary>
        private static Models.Metadata.BiosSet ConvertToInternalModel(BiosSet item)
        {
            var biosset = new Models.Metadata.BiosSet
            {
                [Models.Metadata.BiosSet.NameKey] = item.Name,
                [Models.Metadata.BiosSet.DescriptionKey] = item.Description,
                [Models.Metadata.BiosSet.DefaultKey] = item.Default,
            };
            return biosset;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Chip"/> to <cref="Models.Metadata.Chip"/>
        /// </summary>
        private static Models.Metadata.Chip ConvertToInternalModel(Chip item)
        {
            var chip = new Models.Metadata.Chip
            {
                [Models.Metadata.Chip.NameKey] = item.Name,
                [Models.Metadata.Chip.TagKey] = item.Tag,
                [Models.Metadata.Chip.TypeKey] = item.Type,
                [Models.Metadata.Chip.SoundOnlyKey] = item.SoundOnly,
                [Models.Metadata.Chip.ClockKey] = item.Clock,
            };
            return chip;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Condition"/> to <cref="Models.Metadata.Condition"/>
        /// </summary>
        private static Models.Metadata.Condition ConvertToInternalModel(Condition item)
        {
            var condition = new Models.Metadata.Condition
            {
                [Models.Metadata.Condition.TagKey] = item.Tag,
                [Models.Metadata.Condition.MaskKey] = item.Mask,
                [Models.Metadata.Condition.RelationKey] = item.Relation,
                [Models.Metadata.Condition.ValueKey] = item.Value,
            };
            return condition;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Configuration"/> to <cref="Models.Metadata.Configuration"/>
        /// </summary>
        private static Models.Metadata.Configuration ConvertToInternalModel(Configuration item)
        {
            var configuration = new Models.Metadata.Configuration
            {
                [Models.Metadata.Configuration.NameKey] = item.Name,
                [Models.Metadata.Configuration.TagKey] = item.Tag,
                [Models.Metadata.Configuration.MaskKey] = item.Mask,
            };

            if (item.Condition != null)
                configuration[Models.Metadata.Configuration.ConditionKey] = ConvertToInternalModel(item.Condition);

            if (item.ConfLocation != null && item.ConfLocation.Any())
            {
                configuration[Models.Metadata.Configuration.ConfLocationKey] = item.ConfLocation
                    .Where(c => c != null)
                    .Select(ConvertToInternalModel)
                    .ToArray();
            }

            if (item.ConfSetting != null && item.ConfSetting.Any())
            {
                configuration[Models.Metadata.Configuration.ConfSettingKey] = item.ConfSetting
                    .Where(c => c != null)
                    .Select(ConvertToInternalModel)
                    .ToArray();
            }

            return configuration;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.ConfLocation"/> to <cref="Models.Metadata.ConfLocation"/>
        /// </summary>
        private static Models.Metadata.ConfLocation ConvertToInternalModel(ConfLocation item)
        {
            var confLocation = new Models.Metadata.ConfLocation
            {
                [Models.Metadata.ConfLocation.NameKey] = item.Name,
                [Models.Metadata.ConfLocation.NumberKey] = item.Number,
                [Models.Metadata.ConfLocation.InvertedKey] = item.Inverted,
            };
            return confLocation;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.ConfSetting"/> to <cref="Models.Metadata.ConfSetting"/>
        /// </summary>
        private static Models.Metadata.ConfSetting ConvertToInternalModel(ConfSetting item)
        {
            var confSetting = new Models.Metadata.ConfSetting
            {
                [Models.Metadata.ConfSetting.NameKey] = item.Name,
                [Models.Metadata.ConfSetting.ValueKey] = item.Value,
                [Models.Metadata.ConfSetting.DefaultKey] = item.Default,
            };

            if (item.Condition != null)
                confSetting[Models.Metadata.ConfSetting.ConditionKey] = ConvertToInternalModel(item.Condition);

            return confSetting;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Control"/> to <cref="Models.Metadata.Control"/>
        /// </summary>
        private static Models.Metadata.Control ConvertToInternalModel(Control item)
        {
            var control = new Models.Metadata.Control
            {
                [Models.Metadata.Control.TypeKey] = item.Type,
                [Models.Metadata.Control.PlayerKey] = item.Player,
                [Models.Metadata.Control.ButtonsKey] = item.Buttons,
                [Models.Metadata.Control.ReqButtonsKey] = item.ReqButtons,
                [Models.Metadata.Control.MinimumKey] = item.Minimum,
                [Models.Metadata.Control.MaximumKey] = item.Maximum,
                [Models.Metadata.Control.SensitivityKey] = item.Sensitivity,
                [Models.Metadata.Control.KeyDeltaKey] = item.KeyDelta,
                [Models.Metadata.Control.ReverseKey] = item.Reverse,
                [Models.Metadata.Control.WaysKey] = item.Ways,
                [Models.Metadata.Control.Ways2Key] = item.Ways2,
                [Models.Metadata.Control.Ways3Key] = item.Ways3,
            };
            return control;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Device"/> to <cref="Models.Metadata.Device"/>
        /// </summary>
        private static Models.Metadata.Device ConvertToInternalModel(Device item)
        {
            var device = new Models.Metadata.Device
            {
                [Models.Metadata.Device.TypeKey] = item.Type,
                [Models.Metadata.Device.TagKey] = item.Tag,
                [Models.Metadata.Device.FixedImageKey] = item.FixedImage,
                [Models.Metadata.Device.MandatoryKey] = item.Mandatory,
                [Models.Metadata.Device.InterfaceKey] = item.Interface,
            };

            if (item.Instance != null)
                device[Models.Metadata.Device.InstanceKey] = ConvertToInternalModel(item.Instance);

            if (item.Extension != null && item.Extension.Any())
            {
                device[Models.Metadata.Device.ExtensionKey] = item.Extension
                    .Where(e => e != null)
                    .Select(ConvertToInternalModel)
                    .ToArray();
            }

            return device;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.DeviceRef"/> to <cref="Models.Metadata.DeviceRef"/>
        /// </summary>
        private static Models.Metadata.DeviceRef ConvertToInternalModel(DeviceRef item)
        {
            var deviceRef = new Models.Metadata.DeviceRef
            {
                [Models.Metadata.DeviceRef.NameKey] = item.Name,
            };
            return deviceRef;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.DipLocation"/> to <cref="Models.Metadata.DipLocation"/>
        /// </summary>
        private static Models.Metadata.DipLocation ConvertToInternalModel(DipLocation item)
        {
            var dipLocation = new Models.Metadata.DipLocation
            {
                [Models.Metadata.DipLocation.NameKey] = item.Name,
                [Models.Metadata.DipLocation.NumberKey] = item.Number,
                [Models.Metadata.DipLocation.InvertedKey] = item.Inverted,
            };
            return dipLocation;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.DipSwitch"/> to <cref="Models.Metadata.DipSwitch"/>
        /// </summary>
        private static Models.Metadata.DipSwitch ConvertToInternalModel(DipSwitch item)
        {
            var dipSwitch = new Models.Metadata.DipSwitch
            {
                [Models.Metadata.DipSwitch.NameKey] = item.Name,
                [Models.Metadata.DipSwitch.TagKey] = item.Tag,
                [Models.Metadata.DipSwitch.MaskKey] = item.Mask,
            };

            if (item.Condition != null)
                dipSwitch[Models.Metadata.DipSwitch.ConditionKey] = ConvertToInternalModel(item.Condition);

            if (item.DipLocation != null && item.DipLocation.Any())
            {
                dipSwitch[Models.Metadata.DipSwitch.DipLocationKey] = item.DipLocation
                    .Where(d => d != null)
                    .Select(ConvertToInternalModel)
                    .ToArray();
            }

            if (item.DipValue != null && item.DipValue.Any())
            {
                dipSwitch[Models.Metadata.DipSwitch.DipValueKey] = item.DipValue
                    .Where(d => d != null)
                    .Select(ConvertToInternalModel)
                    .ToArray();
            }

            return dipSwitch;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.DipValue"/> to <cref="Models.Metadata.DipValue"/>
        /// </summary>
        private static Models.Metadata.DipValue ConvertToInternalModel(DipValue item)
        {
            var dipValue = new Models.Metadata.DipValue
            {
                [Models.Metadata.DipValue.NameKey] = item.Name,
                [Models.Metadata.DipValue.ValueKey] = item.Value,
                [Models.Metadata.DipValue.DefaultKey] = item.Default,
            };

            if (item.Condition != null)
                dipValue[Models.Metadata.DipValue.ConditionKey] = ConvertToInternalModel(item.Condition);

            return dipValue;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Disk"/> to <cref="Models.Metadata.Disk"/>
        /// </summary>
        private static Models.Metadata.Disk ConvertToInternalModel(Disk item)
        {
            var disk = new Models.Metadata.Disk
            {
                [Models.Metadata.Disk.NameKey] = item.Name,
                [Models.Metadata.Disk.MD5Key] = item.MD5,
                [Models.Metadata.Disk.SHA1Key] = item.SHA1,
                [Models.Metadata.Disk.MergeKey] = item.Merge,
                [Models.Metadata.Disk.RegionKey] = item.Region,
                [Models.Metadata.Disk.IndexKey] = item.Index,
                [Models.Metadata.Disk.WritableKey] = item.Writable,
                [Models.Metadata.Disk.StatusKey] = item.Status,
                [Models.Metadata.Disk.OptionalKey] = item.Optional,
            };
            return disk;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Display"/> to <cref="Models.Metadata.Display"/>
        /// </summary>
        private static Models.Metadata.Display ConvertToInternalModel(Display item)
        {
            var display = new Models.Metadata.Display
            {
                [Models.Metadata.Display.TagKey] = item.Tag,
                [Models.Metadata.Display.TypeKey] = item.Type,
                [Models.Metadata.Display.RotateKey] = item.Rotate,
                [Models.Metadata.Display.FlipXKey] = item.FlipX,
                [Models.Metadata.Display.WidthKey] = item.Width,
                [Models.Metadata.Display.HeightKey] = item.Height,
                [Models.Metadata.Display.RefreshKey] = item.Refresh,
                [Models.Metadata.Display.PixClockKey] = item.PixClock,
                [Models.Metadata.Display.HTotalKey] = item.HTotal,
                [Models.Metadata.Display.HBEndKey] = item.HBEnd,
                [Models.Metadata.Display.HBStartKey] = item.HBStart,
                [Models.Metadata.Display.VTotalKey] = item.VTotal,
                [Models.Metadata.Display.VBEndKey] = item.VBEnd,
                [Models.Metadata.Display.VBStartKey] = item.VBStart,
            };
            return display;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Driver"/> to <cref="Models.Metadata.Driver"/>
        /// </summary>
        private static Models.Metadata.Driver ConvertToInternalModel(Driver item)
        {
            var driver = new Models.Metadata.Driver
            {
                [Models.Metadata.Driver.StatusKey] = item.Status,
                [Models.Metadata.Driver.ColorKey] = item.Color,
                [Models.Metadata.Driver.SoundKey] = item.Sound,
                [Models.Metadata.Driver.PaletteSizeKey] = item.PaletteSize,
                [Models.Metadata.Driver.EmulationKey] = item.Emulation,
                [Models.Metadata.Driver.CocktailKey] = item.Cocktail,
                [Models.Metadata.Driver.SaveStateKey] = item.SaveState,
                [Models.Metadata.Driver.RequiresArtworkKey] = item.RequiresArtwork,
                [Models.Metadata.Driver.UnofficialKey] = item.Unofficial,
                [Models.Metadata.Driver.NoSoundHardwareKey] = item.NoSoundHardware,
                [Models.Metadata.Driver.IncompleteKey] = item.Incomplete,
            };
            return driver;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Extension"/> to <cref="Models.Metadata.Extension"/>
        /// </summary>
        private static Models.Metadata.Extension ConvertToInternalModel(Extension item)
        {
            var extension = new Models.Metadata.Extension
            {
                [Models.Metadata.Extension.NameKey] = item.Name,
            };
            return extension;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Feature"/> to <cref="Models.Metadata.Feature"/>
        /// </summary>
        private static Models.Metadata.Feature ConvertToInternalModel(Feature item)
        {
            var feature = new Models.Metadata.Feature
            {
                [Models.Metadata.Feature.TypeKey] = item.Type,
                [Models.Metadata.Feature.StatusKey] = item.Status,
                [Models.Metadata.Feature.OverallKey] = item.Overall,
            };
            return feature;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Input"/> to <cref="Models.Metadata.Input"/>
        /// </summary>
        private static Models.Metadata.Input ConvertToInternalModel(Input item)
        {
            var input = new Models.Metadata.Input
            {
                [Models.Metadata.Input.ServiceKey] = item.Service,
                [Models.Metadata.Input.TiltKey] = item.Tilt,
                [Models.Metadata.Input.PlayersKey] = item.Players,
                [Models.Metadata.Input.ControlKey] = item.ControlAttr,
                [Models.Metadata.Input.ButtonsKey] = item.Buttons,
                [Models.Metadata.Input.CoinsKey] = item.Coins,
            };

            if (item.Control != null && item.Control.Any())
            {
                input[Models.Metadata.Input.ControlKey] = item.Control
                    .Where(c => c != null)
                    .Select(ConvertToInternalModel)
                    .ToArray();
            }

            return input;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Instance"/> to <cref="Models.Metadata.Instance"/>
        /// </summary>
        private static Models.Metadata.Instance ConvertToInternalModel(Instance item)
        {
            var instance = new Models.Metadata.Instance
            {
                [Models.Metadata.Instance.NameKey] = item.Name,
                [Models.Metadata.Instance.BriefNameKey] = item.BriefName,
            };
            return instance;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Port"/> to <cref="Models.Metadata.Port"/>
        /// </summary>
        private static Models.Metadata.Port ConvertToInternalModel(Port item)
        {
            var port = new Models.Metadata.Port
            {
                [Models.Metadata.Port.TagKey] = item.Tag,
            };

            if (item.Analog != null && item.Analog.Any())
            {
                port[Models.Metadata.Port.AnalogKey] = item.Analog
                    .Where(a => a != null)
                    .Select(ConvertToInternalModel)
                    .ToArray();
            }

            return port;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.RamOption"/> to <cref="Models.Metadata.RamOption"/>
        /// </summary>
        private static Models.Metadata.RamOption ConvertToInternalModel(RamOption item)
        {
            var ramOption = new Models.Metadata.RamOption
            {
                [Models.Metadata.RamOption.NameKey] = item.Name,
                [Models.Metadata.RamOption.DefaultKey] = item.Default,
                [Models.Metadata.RamOption.ContentKey] = item.Content,
            };
            return ramOption;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Rom"/> to <cref="Models.Metadata.Rom"/>
        /// </summary>
        private static Models.Metadata.Rom ConvertToInternalModel(Rom item)
        {
            var rom = new Models.Metadata.Rom
            {
                [Models.Metadata.Rom.NameKey] = item.Name,
                [Models.Metadata.Rom.BiosKey] = item.Bios,
                [Models.Metadata.Rom.SizeKey] = item.Size,
                [Models.Metadata.Rom.CRCKey] = item.CRC,
                [Models.Metadata.Rom.SHA1Key] = item.SHA1,
                [Models.Metadata.Rom.MergeKey] = item.Merge,
                [Models.Metadata.Rom.RegionKey] = item.Region,
                [Models.Metadata.Rom.OffsetKey] = item.Offset,
                [Models.Metadata.Rom.StatusKey] = item.Status,
                [Models.Metadata.Rom.OptionalKey] = item.Optional,
                [Models.Metadata.Rom.DisposeKey] = item.Dispose,
                [Models.Metadata.Rom.SoundOnlyKey] = item.SoundOnly,
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Sample"/> to <cref="Models.Metadata.Sample"/>
        /// </summary>
        private static Models.Metadata.Sample ConvertToInternalModel(Sample item)
        {
            var sample = new Models.Metadata.Sample
            {
                [Models.Metadata.Sample.NameKey] = item.Name,
            };
            return sample;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Slot"/> to <cref="Models.Metadata.Slot"/>
        /// </summary>
        private static Models.Metadata.Slot ConvertToInternalModel(Slot item)
        {
            var slot = new Models.Metadata.Slot
            {
                [Models.Metadata.Slot.NameKey] = item.Name,
            };

            if (item.SlotOption != null && item.SlotOption.Any())
            {
                slot[Models.Metadata.Slot.SlotOptionKey] = item.SlotOption
                    .Where(s => s != null)
                    .Select(ConvertToInternalModel)
                    .ToArray();
            }

            return slot;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.SlotOption"/> to <cref="Models.Metadata.SlotOption"/>
        /// </summary>
        private static Models.Metadata.SlotOption ConvertToInternalModel(SlotOption item)
        {
            var slotOption = new Models.Metadata.SlotOption
            {
                [Models.Metadata.SlotOption.NameKey] = item.Name,
                [Models.Metadata.SlotOption.DevNameKey] = item.DevName,
                [Models.Metadata.SlotOption.DefaultKey] = item.Default,
            };
            return slotOption;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.SoftwareList"/> to <cref="Models.Metadata.SoftwareList"/>
        /// </summary>
        private static Models.Metadata.SoftwareList ConvertToInternalModel(SoftwareList item)
        {
            var softwareList = new Models.Metadata.SoftwareList
            {
                [Models.Metadata.SoftwareList.TagKey] = item.Tag,
                [Models.Metadata.SoftwareList.NameKey] = item.Name,
                [Models.Metadata.SoftwareList.StatusKey] = item.Status,
                [Models.Metadata.SoftwareList.FilterKey] = item.Filter,
            };
            return softwareList;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Sound"/> to <cref="Models.Metadata.Sound"/>
        /// </summary>
        private static Models.Metadata.Sound ConvertToInternalModel(Sound item)
        {
            var sound = new Models.Metadata.Sound
            {
                [Models.Metadata.Sound.ChannelsKey] = item.Channels,
            };
            return sound;
        }

        /// <summary>
        /// Convert from <cref="Models.Listxml.Video"/> to <cref="Models.Metadata.Video"/>
        /// </summary>
        private static Models.Metadata.Video ConvertToInternalModel(Video item)
        {
            var video = new Models.Metadata.Video
            {
                [Models.Metadata.Video.ScreenKey] = item.Screen,
                [Models.Metadata.Video.OrientationKey] = item.Orientation,
                [Models.Metadata.Video.WidthKey] = item.Width,
                [Models.Metadata.Video.HeightKey] = item.Height,
                [Models.Metadata.Video.AspectXKey] = item.AspectX,
                [Models.Metadata.Video.AspectYKey] = item.AspectY,
                [Models.Metadata.Video.RefreshKey] = item.Refresh,
            };
            return video;
        }

        #endregion

    }
}