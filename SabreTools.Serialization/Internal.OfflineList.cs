using System.Collections.Generic;
using System.Linq;

namespace SabreTools.Serialization
{
    /// <summary>
    /// Serializer for OfflineList models to internal structure
    /// </summary>
    public partial class Internal
    {
        #region Serialize

        /// <summary>
        /// Convert from <cref="Models.OfflineList.Game"/> to <cref="Models.Internal.Machine"/>
        /// </summary>
        public static Models.Internal.Machine ConvertMachineFromOfflineList(Models.OfflineList.Game item)
        {
            var machine = new Models.Internal.Machine
            {
                [Models.Internal.Machine.ImageNumberKey] = item.ImageNumber,
                [Models.Internal.Machine.ReleaseNumberKey] = item.ReleaseNumber,
                [Models.Internal.Machine.NameKey] = item.Title,
                [Models.Internal.Machine.SaveTypeKey] = item.SaveType,
                [Models.Internal.Machine.PublisherKey] = item.Publisher,
                [Models.Internal.Machine.LocationKey] = item.Location,
                [Models.Internal.Machine.SourceRomKey] = item.SourceRom,
                [Models.Internal.Machine.LanguageKey] = item.Language,
                [Models.Internal.Machine.Im1CRCKey] = item.Im1CRC,
                [Models.Internal.Machine.Im2CRCKey] = item.Im2CRC,
                [Models.Internal.Machine.CommentKey] = item.Comment,
                [Models.Internal.Machine.DuplicateIDKey] = item.DuplicateID,
            };

            if (item.Files?.RomCRC != null && item.Files.RomCRC.Any())
            {
                var roms = new List<Models.Internal.Rom>();
                foreach (var file in item.Files.RomCRC)
                {
                    var rom = ConvertFromOfflineList(file);
                    rom[Models.Internal.Rom.SizeKey] = item.RomSize;
                    roms.Add(rom);
                }
                machine[Models.Internal.Machine.RomKey] = roms.ToArray();
            }

            return machine;
        }

        /// <summary>
        /// Convert from <cref="Models.OfflineList.Files"/> to an array of <cref="Models.Internal.Rom"/>
        /// </summary>
        public static Models.Internal.Rom[] ConvertFromOfflineList(Models.OfflineList.Files item)
        {
            var roms = new List<Models.Internal.Rom>();
            foreach (var romCRC in item.RomCRC)
            {
                roms.Add(ConvertFromOfflineList(romCRC));
            }
            return roms.ToArray();
        }

        /// <summary>
        /// Convert from <cref="Models.OfflineList.FileRomCRC"/> to <cref="Models.Internal.Rom"/>
        /// </summary>
        public static Models.Internal.Rom ConvertFromOfflineList(Models.OfflineList.FileRomCRC item)
        {
            var rom = new Models.Internal.Rom
            {
                [Models.Internal.Rom.ExtensionKey] = item.Extension,
                [Models.Internal.Rom.CRCKey] = item.Content,
            };
            return rom;
        }

        #endregion

        #region Deserialize

        /// <summary>
        /// Convert from <cref="Models.Internal.Machine"/> to <cref="Models.OfflineList.Game"/>
        /// </summary>
        public static Models.OfflineList.Game ConvertMachineToOfflineList(Models.Internal.Machine item)
        {
            var game = new Models.OfflineList.Game
            {
                ImageNumber = item.ReadString(Models.Internal.Machine.ImageNumberKey),
                ReleaseNumber = item.ReadString(Models.Internal.Machine.ReleaseNumberKey),
                Title = item.ReadString(Models.Internal.Machine.NameKey),
                SaveType = item.ReadString(Models.Internal.Machine.SaveTypeKey),
                Publisher = item.ReadString(Models.Internal.Machine.PublisherKey),
                Location = item.ReadString(Models.Internal.Machine.LocationKey),
                SourceRom = item.ReadString(Models.Internal.Machine.SourceRomKey),
                Language = item.ReadString(Models.Internal.Machine.LanguageKey),
                Im1CRC = item.ReadString(Models.Internal.Machine.Im1CRCKey),
                Im2CRC = item.ReadString(Models.Internal.Machine.Im2CRCKey),
                Comment = item.ReadString(Models.Internal.Machine.CommentKey),
                DuplicateID = item.ReadString(Models.Internal.Machine.DuplicateIDKey),
            };

            if (item.ContainsKey(Models.Internal.Machine.RomKey) && item[Models.Internal.Machine.RomKey] is Models.Internal.Rom[] roms)
            {
                var romCRCItems = new List<Models.OfflineList.FileRomCRC>();
                foreach (var rom in roms)
                {
                    game.RomSize ??= rom.ReadString(Models.Internal.Rom.SizeKey);
                    romCRCItems.Add(ConvertToOfflineList(rom));
                }
                game.Files = new Models.OfflineList.Files { RomCRC = romCRCItems.ToArray() };
            }

            return game;
        }

        /// <summary>
        /// Convert from an array of <cref="Models.Internal.Rom"/> to <cref="Models.OfflineList.FileRomCRC"/>
        /// </summary>
        public static Models.OfflineList.Files ConvertToOfflineList(Models.Internal.Rom[] items)
        {
            var romCRCs = new List<Models.OfflineList.FileRomCRC>();
            foreach (var item in items)
            {
                romCRCs.Add(ConvertToOfflineList(item));
            }
            return new Models.OfflineList.Files() { RomCRC = romCRCs.ToArray() };
        }

        /// <summary>
        /// Convert from <cref="Models.Internal.Rom"/> to <cref="Models.OfflineList.FileRomCRC"/>
        /// </summary>
        public static Models.OfflineList.FileRomCRC ConvertToOfflineList(Models.Internal.Rom item)
        {
            var fileRomCRC = new Models.OfflineList.FileRomCRC
            {
                Extension = item.ReadString(Models.Internal.Rom.ExtensionKey),
                Content = item.ReadString(Models.Internal.Rom.CRCKey),
            };
            return fileRomCRC;
        }

        #endregion
    }
}