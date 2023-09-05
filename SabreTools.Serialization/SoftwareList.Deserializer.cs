using System.Linq;
using SabreTools.Models.SoftwareList;

namespace SabreTools.Serialization
{
    /// <summary>
    /// XML deserializer for MAME softwarelist files
    /// </summary>
    public partial class SoftawreList : XmlSerializer<SoftwareList>
    {
        #region Internal

        /// <summary>
        /// Convert from <cref="Models.Metadata.MetadataFile"/> to <cref="Models.SoftawreList.SoftwareList"/>
        /// </summary>
        public static SoftwareList? ConvertFromInternalModel(Models.Metadata.MetadataFile? item)
        {
            if (item == null)
                return null;

            var header = item.Read<Models.Metadata.Header>(Models.Metadata.MetadataFile.HeaderKey);
            var metadataFile = header != null ? ConvertHeaderFromInternalModel(header) : new SoftwareList();

            var machines = item.Read<Models.Metadata.Machine[]>(Models.Metadata.MetadataFile.MachineKey);
            if (machines != null && machines.Any())
            {
                metadataFile.Software = machines
                    .Where(m => m != null)
                    .Select(ConvertMachineFromInternalModel)
                    .ToArray();
            }

            return metadataFile;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Header"/> to <cref="Models.SoftwareList.SoftwareList"/>
        /// </summary>
        private static SoftwareList ConvertHeaderFromInternalModel(Models.Metadata.Header item)
        {
            var softwareList = new SoftwareList
            {
                Name = item.ReadString(Models.Metadata.Header.NameKey),
                Description = item.ReadString(Models.Metadata.Header.DescriptionKey),
                Notes = item.ReadString(Models.Metadata.Header.NotesKey),
            };
            return softwareList;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Machine"/> to <cref="Models.SoftwareList.Software"/>
        /// </summary>
        private static Software ConvertMachineFromInternalModel(Models.Metadata.Machine item)
        {
            var software = new Software
            {
                Name = item.ReadString(Models.Metadata.Machine.NameKey),
                CloneOf = item.ReadString(Models.Metadata.Machine.CloneOfKey),
                Supported = item.ReadString(Models.Metadata.Machine.SupportedKey),
                Description = item.ReadString(Models.Metadata.Machine.DescriptionKey),
                Year = item.ReadString(Models.Metadata.Machine.YearKey),
                Publisher = item.ReadString(Models.Metadata.Machine.PublisherKey),
                Notes = item.ReadString(Models.Metadata.Machine.NotesKey),
            };

            var infos = item.Read<Models.Metadata.Info[]>(Models.Metadata.Machine.InfoKey);
            if (infos != null && infos.Any())
            {
                software.Info = infos
                    .Where(i => i != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var sharedFeats = item.Read<Models.Metadata.SharedFeat[]>(Models.Metadata.Machine.SharedFeatKey);
            if (sharedFeats != null && sharedFeats.Any())
            {
                software.SharedFeat = sharedFeats
                    .Where(s => s != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var parts = item.Read<Models.Metadata.Part[]>(Models.Metadata.Machine.PartKey);
            if (parts != null && parts.Any())
            {
                software.Part = parts
                    .Where(p => p != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            return software;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.DataArea"/> to <cref="Models.SoftwareList.DataArea"/>
        /// </summary>
        private static DataArea ConvertFromInternalModel(Models.Metadata.DataArea item)
        {
            var dataArea = new DataArea
            {
                Name = item.ReadString(Models.Metadata.DataArea.NameKey),
                Size = item.ReadString(Models.Metadata.DataArea.SizeKey),
                Width = item.ReadString(Models.Metadata.DataArea.WidthKey),
                Endianness = item.ReadString(Models.Metadata.DataArea.EndiannessKey),
            };

            var roms = item.Read<Models.Metadata.Rom[]>(Models.Metadata.DataArea.RomKey);
            if (roms != null && roms.Any())
            {
                dataArea.Rom = roms
                    .Where(r => r != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            return dataArea;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.DipSwitch"/> to <cref="Models.SoftwareList.DipSwitch"/>
        /// </summary>
        private static DipSwitch ConvertFromInternalModel(Models.Metadata.DipSwitch item)
        {
            var dipSwitch = new DipSwitch
            {
                Name = item.ReadString(Models.Metadata.DipSwitch.NameKey),
                Tag = item.ReadString(Models.Metadata.DipSwitch.TagKey),
                Mask = item.ReadString(Models.Metadata.DipSwitch.MaskKey),
            };

            var dipValues = item.Read<Models.Metadata.DipValue[]>(Models.Metadata.DipSwitch.DipValueKey);
            if (dipValues != null && dipValues.Any())
            {
                dipSwitch.DipValue = dipValues
                    .Where(d => d != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            return dipSwitch;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.DipValue"/> to <cref="Models.SoftwareList.DipValue"/>
        /// </summary>
        private static DipValue ConvertFromInternalModel(Models.Metadata.DipValue item)
        {
            var dipValue = new DipValue
            {
                Name = item.ReadString(Models.Metadata.DipValue.NameKey),
                Value = item.ReadString(Models.Metadata.DipValue.ValueKey),
                Default = item.ReadString(Models.Metadata.DipValue.DefaultKey),
            };
            return dipValue;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Disk"/> to <cref="Models.SoftwareList.Disk"/>
        /// </summary>
        private static Disk ConvertFromInternalModel(Models.Metadata.Disk item)
        {
            var disk = new Disk
            {
                Name = item.ReadString(Models.Metadata.Disk.NameKey),
                MD5 = item.ReadString(Models.Metadata.Disk.MD5Key),
                SHA1 = item.ReadString(Models.Metadata.Disk.SHA1Key),
                Status = item.ReadString(Models.Metadata.Disk.StatusKey),
                Writeable = item.ReadString(Models.Metadata.Disk.WritableKey),
            };
            return disk;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.DiskArea"/> to <cref="Models.SoftwareList.DiskArea"/>
        /// </summary>
        private static DiskArea ConvertFromInternalModel(Models.Metadata.DiskArea item)
        {
            var diskArea = new DiskArea
            {
                Name = item.ReadString(Models.Metadata.DiskArea.NameKey),
            };

            var disks = item.Read<Models.Metadata.Disk[]>(Models.Metadata.DiskArea.DiskKey);
            if (disks != null && disks.Any())
            {
                diskArea.Disk = disks
                    .Where(d => d != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            return diskArea;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Feature"/> to <cref="Models.SoftwareList.Feature"/>
        /// </summary>
        private static Feature ConvertFromInternalModel(Models.Metadata.Feature item)
        {
            var feature = new Feature
            {
                Name = item.ReadString(Models.Metadata.Feature.NameKey),
                Value = item.ReadString(Models.Metadata.Feature.ValueKey),
            };
            return feature;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Info"/> to <cref="Models.SoftwareList.Info"/>
        /// </summary>
        private static Info ConvertFromInternalModel(Models.Metadata.Info item)
        {
            var info = new Info
            {
                Name = item.ReadString(Models.Metadata.Info.NameKey),
                Value = item.ReadString(Models.Metadata.Info.ValueKey),
            };
            return info;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Part"/> to <cref="Models.SoftwareList.Part"/>
        /// </summary>
        private static Part ConvertFromInternalModel(Models.Metadata.Part item)
        {
            var part = new Part
            {
                Name = item.ReadString(Models.Metadata.Part.NameKey),
                Interface = item.ReadString(Models.Metadata.Part.InterfaceKey),
            };

            var features = item.Read<Models.Metadata.Feature[]>(Models.Metadata.Part.FeatureKey);
            if (features != null && features.Any())
            {
                part.Feature = features
                    .Where(f => f != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var dataAreas = item.Read<Models.Metadata.DataArea[]>(Models.Metadata.Part.DataAreaKey);
            if (dataAreas != null && dataAreas.Any())
            {
                part.DataArea = dataAreas
                    .Where(d => d != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var diskAreas = item.Read<Models.Metadata.DiskArea[]>(Models.Metadata.Part.DiskAreaKey);
            if (diskAreas != null && diskAreas.Any())
            {
                part.DiskArea = diskAreas
                    .Where(d => d != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            var dipSwitches = item.Read<Models.Metadata.DipSwitch[]>(Models.Metadata.Part.DipSwitchKey);
            if (dipSwitches != null && dipSwitches.Any())
            {
                part.DipSwitch = dipSwitches
                    .Where(d => d != null)
                    .Select(ConvertFromInternalModel)
                    .ToArray();
            }

            return part;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Rom"/> to <cref="Models.SoftwareList.Rom"/>
        /// </summary>
        private static Rom ConvertFromInternalModel(Models.Metadata.Rom item)
        {
            var rom = new Rom
            {
                Name = item.ReadString(Models.Metadata.Rom.NameKey),
                Size = item.ReadString(Models.Metadata.Rom.SizeKey),
                Length = item.ReadString(Models.Metadata.Rom.LengthKey),
                CRC = item.ReadString(Models.Metadata.Rom.CRCKey),
                SHA1 = item.ReadString(Models.Metadata.Rom.SHA1Key),
                Offset = item.ReadString(Models.Metadata.Rom.OffsetKey),
                Value = item.ReadString(Models.Metadata.Rom.ValueKey),
                Status = item.ReadString(Models.Metadata.Rom.StatusKey),
                LoadFlag = item.ReadString(Models.Metadata.Rom.LoadFlagKey),
            };
            return rom;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.SharedFeat"/> to <cref="Models.SoftwareList.SharedFeat"/>
        /// </summary>
        private static SharedFeat ConvertFromInternalModel(Models.Metadata.SharedFeat item)
        {
            var sharedFeat = new SharedFeat
            {
                Name = item.ReadString(Models.Metadata.SharedFeat.NameKey),
                Value = item.ReadString(Models.Metadata.SharedFeat.ValueKey),
            };
            return sharedFeat;
        }

        #endregion
    }
}