using System.Collections.Generic;
using System.Linq;
using SabreTools.Models.Internal;

namespace SabreTools.Serialization
{
    /// <summary>
    /// Serializer for OfflineList models to internal structure
    /// </summary>
    public partial class Internal
    {
        #region Serialize

        /// <summary>
        /// Convert from <cref="Models.OfflineList.Dat"/> to <cref="Header"/>
        /// </summary>
        public static Header ConvertHeaderFromOfflineList(Models.OfflineList.Dat item)
        {
            var header = new Header
            {
                [Header.NoNamespaceSchemaLocationKey] = item.NoNamespaceSchemaLocation,
            };

            if (item.Configuration != null)
            {
                header[Header.NameKey] = item.Configuration.DatName;
                header[Header.ImFolderKey] = item.Configuration.ImFolder;
                header[Header.DatVersionKey] = item.Configuration.DatVersion;
                header[Header.SystemKey] = item.Configuration.System;
                header[Header.ScreenshotsWidthKey] = item.Configuration.ScreenshotsWidth;
                header[Header.ScreenshotsHeightKey] = item.Configuration.ScreenshotsHeight;
                header[Header.InfosKey] = item.Configuration.Infos;
                header[Header.CanOpenKey] = item.Configuration.CanOpen;
                header[Header.NewDatKey] = item.Configuration.NewDat;
                header[Header.SearchKey] = item.Configuration.Search;
                header[Header.RomTitleKey] = item.Configuration.RomTitle;
            }

            if (item.GUI != null)
            {
                header[Header.ImagesKey] = item.GUI.Images;
            }

            return header;
        }

        /// <summary>
        /// Convert from <cref="Models.OfflineList.Game"/> to <cref="Machine"/>
        /// </summary>
        public static Machine ConvertMachineFromOfflineList(Models.OfflineList.Game item)
        {
            var machine = new Machine
            {
                [Machine.ImageNumberKey] = item.ImageNumber,
                [Machine.ReleaseNumberKey] = item.ReleaseNumber,
                [Machine.NameKey] = item.Title,
                [Machine.SaveTypeKey] = item.SaveType,
                [Machine.PublisherKey] = item.Publisher,
                [Machine.LocationKey] = item.Location,
                [Machine.SourceRomKey] = item.SourceRom,
                [Machine.LanguageKey] = item.Language,
                [Machine.Im1CRCKey] = item.Im1CRC,
                [Machine.Im2CRCKey] = item.Im2CRC,
                [Machine.CommentKey] = item.Comment,
                [Machine.DuplicateIDKey] = item.DuplicateID,
            };

            if (item.Files?.RomCRC != null && item.Files.RomCRC.Any())
            {
                var roms = new List<Rom>();
                foreach (var file in item.Files.RomCRC)
                {
                    var rom = ConvertFromOfflineList(file);
                    rom[Rom.SizeKey] = item.RomSize;
                    roms.Add(rom);
                }
                machine[Machine.RomKey] = roms.ToArray();
            }

            return machine;
        }

        /// <summary>
        /// Convert from <cref="Models.OfflineList.Files"/> to an array of <cref="Rom"/>
        /// </summary>
        public static Rom[] ConvertFromOfflineList(Models.OfflineList.Files item)
        {
            var roms = new List<Rom>();
            foreach (var romCRC in item.RomCRC)
            {
                roms.Add(ConvertFromOfflineList(romCRC));
            }
            return roms.ToArray();
        }

        /// <summary>
        /// Convert from <cref="Models.OfflineList.FileRomCRC"/> to <cref="Rom"/>
        /// </summary>
        public static Rom ConvertFromOfflineList(Models.OfflineList.FileRomCRC item)
        {
            var rom = new Rom
            {
                [Rom.ExtensionKey] = item.Extension,
                [Rom.CRCKey] = item.Content,
            };
            return rom;
        }

        #endregion

        #region Deserialize

        /// <summary>
        /// Convert from <cref="Header"/> to <cref="Models.OfflineList.Dat"/>
        /// </summary>
        public static Models.OfflineList.Dat? ConvertHeaderToOfflineList(Header? item)
        {
            if (item == null)
                return null;

            var dat = new Models.OfflineList.Dat
            {
                NoNamespaceSchemaLocation = item.ReadString(Header.NoNamespaceSchemaLocationKey),
            };

            if (item.ContainsKey(Header.NameKey)
                || item.ContainsKey(Header.ImFolderKey)
                || item.ContainsKey(Header.DatVersionKey)
                || item.ContainsKey(Header.SystemKey)
                || item.ContainsKey(Header.ScreenshotsWidthKey)
                || item.ContainsKey(Header.ScreenshotsHeightKey)
                || item.ContainsKey(Header.InfosKey)
                || item.ContainsKey(Header.CanOpenKey)
                || item.ContainsKey(Header.NewDatKey)
                || item.ContainsKey(Header.SearchKey)
                || item.ContainsKey(Header.RomTitleKey))
            {
                dat.Configuration = new Models.OfflineList.Configuration
                {
                    DatName = item.ReadString(Header.NameKey),
                    ImFolder = item.ReadString(Header.ImFolderKey),
                    DatVersion = item.ReadString(Header.DatVersionKey),
                    System = item.ReadString(Header.SystemKey),
                    ScreenshotsWidth = item.ReadString(Header.ScreenshotsWidthKey),
                    ScreenshotsHeight = item.ReadString(Header.ScreenshotsHeightKey),
                    Infos = item.Read<Models.OfflineList.Infos>(Header.InfosKey),
                    CanOpen = item.Read<Models.OfflineList.CanOpen>(Header.CanOpenKey),
                    NewDat = item.Read<Models.OfflineList.NewDat>(Header.NewDatKey),
                    Search = item.Read<Models.OfflineList.Search>(Header.SearchKey),
                    RomTitle = item.ReadString(Header.RomTitleKey),
                };
            }

            if (item.ContainsKey(Header.ImagesKey))
            {
                dat.GUI = new Models.OfflineList.GUI
                {
                    Images = item.Read<Models.OfflineList.Images>(Header.ImagesKey),
                };
            }

            return dat;
        }

        /// <summary>
        /// Convert from <cref="Machine"/> to <cref="Models.OfflineList.Game"/>
        /// </summary>
        public static Models.OfflineList.Game? ConvertMachineToOfflineList(Machine? item)
        {
            if (item == null)
                return null;

            var game = new Models.OfflineList.Game
            {
                ImageNumber = item.ReadString(Machine.ImageNumberKey),
                ReleaseNumber = item.ReadString(Machine.ReleaseNumberKey),
                Title = item.ReadString(Machine.NameKey),
                SaveType = item.ReadString(Machine.SaveTypeKey),
                Publisher = item.ReadString(Machine.PublisherKey),
                Location = item.ReadString(Machine.LocationKey),
                SourceRom = item.ReadString(Machine.SourceRomKey),
                Language = item.ReadString(Machine.LanguageKey),
                Im1CRC = item.ReadString(Machine.Im1CRCKey),
                Im2CRC = item.ReadString(Machine.Im2CRCKey),
                Comment = item.ReadString(Machine.CommentKey),
                DuplicateID = item.ReadString(Machine.DuplicateIDKey),
            };

            var roms = item.Read<Rom[]>(Machine.RomKey);
            game.RomSize = roms?
                .Select(rom => rom.ReadString(Rom.SizeKey))?
                .FirstOrDefault(s => s != null);
            var romCRCs = roms?.Select(ConvertToOfflineList).ToArray();
            game.Files = new Models.OfflineList.Files { RomCRC = romCRCs };

            return game;
        }

        /// <summary>
        /// Convert from <cref="Rom"/> to <cref="Models.OfflineList.FileRomCRC"/>
        /// </summary>
        private static Models.OfflineList.FileRomCRC? ConvertToOfflineList(Rom? item)
        {
            if (item == null)
                return null;

            var fileRomCRC = new Models.OfflineList.FileRomCRC
            {
                Extension = item.ReadString(Rom.ExtensionKey),
                Content = item.ReadString(Rom.CRCKey),
            };
            return fileRomCRC;
        }

        #endregion
    }
}