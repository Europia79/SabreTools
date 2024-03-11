using System;
using System.Collections.Generic;
using System.Linq;
using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;

namespace SabreTools.DatFiles.Formats
{
    /// <summary>
    /// Represents parsing and writing of a DosCenter DAT
    /// </summary>
    internal partial class DosCenter : DatFile
    {
        /// <inheritdoc/>
        protected override ItemType[] GetSupportedTypes()
        {
            return
            [
                ItemType.Rom
            ];
        }

        /// <inheritdoc/>
        protected override List<string>? GetMissingRequiredFields(DatItem datItem)
        {
            var missingFields = new List<string>();

            // Check item name
            if (string.IsNullOrEmpty(datItem.GetName()))
                missingFields.Add(Models.Metadata.Rom.NameKey);

            switch (datItem)
            {
                case Rom rom:
                    if (rom.GetFieldValue<string?>(Models.Metadata.Rom.SizeKey) == null || NumberHelper.ConvertToInt64(rom.GetFieldValue<string?>(Models.Metadata.Rom.SizeKey)) < 0)
                        missingFields.Add(Models.Metadata.Rom.SizeKey);
                    // if (string.IsNullOrEmpty(rom.Date))
                    //     missingFields.Add(Models.Metadata.Rom.DateKey);
                    if (string.IsNullOrEmpty(rom.GetFieldValue<string?>(Models.Metadata.Rom.CRCKey)))
                        missingFields.Add(Models.Metadata.Rom.CRCKey);
                    break;
            }

            return missingFields;
        }

        /// <inheritdoc/>
        public override bool WriteToFile(string outfile, bool ignoreblanks = false, bool throwOnError = false)
        {
            try
            {
                logger.User($"Writing to '{outfile}'...");

                var metadataFile = CreateMetadataFile(ignoreblanks);
                if (!(new Serialization.Files.DosCenter().Serialize(metadataFile, outfile)))
                {
                    logger.Warning($"File '{outfile}' could not be written! See the log for more details.");
                    return false;
                }
            }
            catch (Exception ex) when (!throwOnError)
            {
                logger.Error(ex);
                return false;
            }

            logger.User($"'{outfile}' written!{Environment.NewLine}");
            return true;
        }

        #region Converters

        /// <summary>
        /// Create a MetadataFile from the current internal information
        /// <summary>
        /// <param name="ignoreblanks">True if blank roms should be skipped on output, false otherwise</param>
        private Models.DosCenter.MetadataFile CreateMetadataFile(bool ignoreblanks)
        {
            var metadataFile = new Models.DosCenter.MetadataFile
            {
                DosCenter = CreateDosCenter(),
                Game = CreateGames(ignoreblanks)
            };
            return metadataFile;
        }

        /// <summary>
        /// Create a DosCenter from the current internal information
        /// <summary>
        private Models.DosCenter.DosCenter? CreateDosCenter()
        {
            // If we don't have a header, we can't do anything
            if (this.Header == null)
                return null;

            var clrMamePro = new Models.DosCenter.DosCenter
            {
                Name = Header.GetFieldValue<string?>(Models.Metadata.Header.NameKey),
                Description = Header.GetFieldValue<string?>(Models.Metadata.Header.DescriptionKey),
                Version = Header.GetFieldValue<string?>(Models.Metadata.Header.VersionKey),
                Date = Header.GetFieldValue<string?>(Models.Metadata.Header.DateKey),
                Author = Header.GetFieldValue<string?>(Models.Metadata.Header.AuthorKey),
                Homepage = Header.GetFieldValue<string?>(Models.Metadata.Header.HomepageKey),
                Comment = Header.GetFieldValue<string?>(Models.Metadata.Header.CommentKey),
            };

            return clrMamePro;
        }

        /// <summary>
        /// Create an array of GameBase from the current internal information
        /// <summary>
        /// <param name="ignoreblanks">True if blank roms should be skipped on output, false otherwise</param>
        private Models.DosCenter.Game[]? CreateGames(bool ignoreblanks)
        {
            // If we don't have items, we can't do anything
            if (this.Items == null || !this.Items.Any())
                return null;

            // Create a list of hold the games
            var games = new List<Models.DosCenter.Game>();

            // Loop through the sorted items and create games for them
            foreach (string key in Items.SortedKeys)
            {
                var items = Items.FilteredItems(key);
                if (items == null || !items.Any())
                    continue;

                // Get the first item for game information
                var machine = items[0].GetFieldValue<Machine>(DatItem.MachineKey);

                // We re-add the missing parts of the game name
                var game = new Models.DosCenter.Game
                {
                    Name = $"\"{machine?.GetFieldValue<string?>(Models.Metadata.Machine.NameKey) ?? string.Empty}.zip\""
                };

                // Create holders for all item types
                var files = new List<Models.DosCenter.File>();

                // Loop through and convert the items to respective lists
                for (int index = 0; index < items.Count; index++)
                {
                    // Get the item
                    var item = items[index];

                    // Check for a "null" item
                    item = ProcessNullifiedItem(item);

                    // Skip if we're ignoring the item
                    if (ShouldIgnore(item, ignoreblanks))
                        continue;

                    switch (item)
                    {
                        case Rom rom:
                            files.Add(CreateFile(rom));
                            break;
                    }
                }

                // Assign the values to the game
                game.File = [.. files];

                // Add the game to the list
                games.Add(game);
            }

            return [.. games];
        }

        /// <summary>
        /// Create a File from the current Rom DatItem
        /// <summary>
        private static Models.DosCenter.File CreateFile(Rom item)
        {
            var rom = new Models.DosCenter.File
            {
                Name = item.GetName(),
                Size = item.GetFieldValue<string?>(Models.Metadata.Rom.SizeKey),
                CRC = item.GetFieldValue<string?>(Models.Metadata.Rom.CRCKey),
                Date = item.GetFieldValue<string?>(Models.Metadata.Rom.DateKey),
            };
            return rom;
        }

        #endregion
    }
}
