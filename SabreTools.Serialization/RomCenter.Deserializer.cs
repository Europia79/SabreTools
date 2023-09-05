using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SabreTools.IO.Readers;
using SabreTools.Models.RomCenter;

namespace SabreTools.Serialization
{
    /// <summary>
    /// Deserializer for RomCenter INI files
    /// </summary>
    public partial class RomCenter
    {
        /// <summary>
        /// Deserializes a RomCenter INI file to the defined type
        /// </summary>
        /// <param name="path">Path to the file to deserialize</param>
        /// <returns>Deserialized data on success, null on failure</returns>
        public static MetadataFile? Deserialize(string path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Deserialize(stream);
        }

        /// <summary>
        /// Deserializes a RomCenter INI file in a stream to the defined type
        /// </summary>
        /// <param name="stream">Stream to deserialize</param>
        /// <returns>Deserialized data on success, null on failure</returns>
        public static MetadataFile? Deserialize(Stream? stream)
        {
            // If the stream is null
            if (stream == null)
                return default;

            // Setup the reader and output
            var reader = new IniReader(stream, Encoding.UTF8)
            {
                ValidateRows = false,
            };
            var dat = new MetadataFile();

            // Loop through and parse out the values
            var roms = new List<Rom>();
            var additional = new List<string>();
            var creditsAdditional = new List<string>();
            var datAdditional = new List<string>();
            var emulatorAdditional = new List<string>();
            var gamesAdditional = new List<string>();
            while (!reader.EndOfStream)
            {
                // If we have no next line
                if (!reader.ReadNextLine())
                    break;

                // Ignore certain row types
                switch (reader.RowType)
                {
                    case IniRowType.None:
                    case IniRowType.Comment:
                        continue;
                    case IniRowType.SectionHeader:
                        switch (reader.Section?.ToLowerInvariant())
                        {
                            case "credits":
                                dat.Credits ??= new Credits();
                                break;
                            case "dat":
                                dat.Dat ??= new Dat();
                                break;
                            case "emulator":
                                dat.Emulator ??= new Emulator();
                                break;
                            case "games":
                                dat.Games ??= new Games();
                                break;
                            default:
                                if (reader.CurrentLine != null)
                                    additional.Add(reader.CurrentLine);
                                break;
                        }
                        continue;
                }

                // If we're in credits
                if (reader.Section?.ToLowerInvariant() == "credits")
                {
                    // Create the section if we haven't already
                    dat.Credits ??= new Credits();

                    switch (reader.KeyValuePair?.Key?.ToLowerInvariant())
                    {
                        case "author":
                            dat.Credits.Author = reader.KeyValuePair?.Value;
                            break;
                        case "version":
                            dat.Credits.Version = reader.KeyValuePair?.Value;
                            break;
                        case "email":
                            dat.Credits.Email = reader.KeyValuePair?.Value;
                            break;
                        case "homepage":
                            dat.Credits.Homepage = reader.KeyValuePair?.Value;
                            break;
                        case "url":
                            dat.Credits.Url = reader.KeyValuePair?.Value;
                            break;
                        case "date":
                            dat.Credits.Date = reader.KeyValuePair?.Value;
                            break;
                        case "comment":
                            dat.Credits.Comment = reader.KeyValuePair?.Value;
                            break;
                        default:
                            if (reader.CurrentLine != null)
                                creditsAdditional.Add(reader.CurrentLine);
                            break;
                    }
                }

                // If we're in dat
                else if (reader.Section?.ToLowerInvariant() == "dat")
                {
                    // Create the section if we haven't already
                    dat.Dat ??= new Dat();

                    switch (reader.KeyValuePair?.Key?.ToLowerInvariant())
                    {
                        case "version":
                            dat.Dat.Version = reader.KeyValuePair?.Value;
                            break;
                        case "plugin":
                            dat.Dat.Plugin = reader.KeyValuePair?.Value;
                            break;
                        case "split":
                            dat.Dat.Split = reader.KeyValuePair?.Value;
                            break;
                        case "merge":
                            dat.Dat.Merge = reader.KeyValuePair?.Value;
                            break;
                        default:
                            if (reader.CurrentLine != null)
                                datAdditional.Add(reader.CurrentLine);
                            break;
                    }
                }

                // If we're in emulator
                else if (reader.Section?.ToLowerInvariant() == "emulator")
                {
                    // Create the section if we haven't already
                    dat.Emulator ??= new Emulator();

                    switch (reader.KeyValuePair?.Key?.ToLowerInvariant())
                    {
                        case "refname":
                            dat.Emulator.RefName = reader.KeyValuePair?.Value;
                            break;
                        case "version":
                            dat.Emulator.Version = reader.KeyValuePair?.Value;
                            break;
                        default:
                            if (reader.CurrentLine != null)
                                emulatorAdditional.Add(reader.CurrentLine);
                            break;
                    }
                }

                // If we're in games
                else if (reader.Section?.ToLowerInvariant() == "games")
                {
                    // Create the section if we haven't already
                    dat.Games ??= new Games();

                    // If the line doesn't contain the delimiter
                    if (!(reader.CurrentLine?.Contains('¬') ?? false))
                    {
                        if (reader.CurrentLine != null)
                            gamesAdditional.Add(reader.CurrentLine);

                        continue;
                    }

                    // Otherwise, separate out the line
                    string[] splitLine = reader.CurrentLine.Split('¬');
                    var rom = new Rom
                    {
                        // EMPTY = splitLine[0]
                        ParentName = splitLine[1],
                        ParentDescription = splitLine[2],
                        GameName = splitLine[3],
                        GameDescription = splitLine[4],
                        RomName = splitLine[5],
                        RomCRC = splitLine[6],
                        RomSize = splitLine[7],
                        RomOf = splitLine[8],
                        MergeName = splitLine[9],
                        // EMPTY = splitLine[10]
                    };

                    if (splitLine.Length > 11)
                        rom.ADDITIONAL_ELEMENTS = splitLine.Skip(11).ToArray();

                    roms.Add(rom);
                }

                else
                {
                    if (reader.CurrentLine != null)
                        additional.Add(reader.CurrentLine);
                }
            }

            // Add extra pieces and return
            dat.ADDITIONAL_ELEMENTS = additional.Where(s => s != null).ToArray();
            if (dat.Credits != null)
                dat.Credits.ADDITIONAL_ELEMENTS = creditsAdditional.Where(s => s != null).ToArray();
            if (dat.Dat != null)
                dat.Dat.ADDITIONAL_ELEMENTS = datAdditional.Where(s => s != null).ToArray();
            if (dat.Emulator != null)
                dat.Emulator.ADDITIONAL_ELEMENTS = emulatorAdditional.Where(s => s != null).ToArray();
            if (dat.Games != null)
            {
                dat.Games.Rom = roms.ToArray();
                dat.Games.ADDITIONAL_ELEMENTS = gamesAdditional.Where(s => s != null).Select(s => s).ToArray();
            }
            return dat;
        }

        #region Internal

        /// <summary>
        /// Convert from <cref="Models.Metadata.MetadataFile"/> to <cref="Models.RomCenter.MetadataFile"/>
        /// </summary>
        public static MetadataFile? ConvertFromInternalModel(Models.Metadata.MetadataFile? item)
        {
            if (item == null)
                return null;

            var header = item.Read<Models.Metadata.Header>(Models.Metadata.MetadataFile.HeaderKey);
            var metadataFile = header != null ? ConvertHeaderFromInternalModel(header) : new MetadataFile();

            var machines = item.Read<Models.Metadata.Machine[]>(Models.Metadata.MetadataFile.MachineKey);
            if (machines != null && machines.Any())
            {
                metadataFile.Games = new Games
                {
                    Rom = machines
                        .Where(m => m != null)
                        .SelectMany(ConvertMachineFromInternalModel)
                        .ToArray()
                };
            }

            return metadataFile;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Header"/> to <cref="Models.RomCenter.MetadataFile"/>
        /// </summary>
        private static MetadataFile ConvertHeaderFromInternalModel(Models.Metadata.Header item)
        {
            var metadataFile = new MetadataFile();

            if (item.ContainsKey(Models.Metadata.Header.AuthorKey)
                || item.ContainsKey(Models.Metadata.Header.VersionKey)
                || item.ContainsKey(Models.Metadata.Header.EmailKey)
                || item.ContainsKey(Models.Metadata.Header.HomepageKey)
                || item.ContainsKey(Models.Metadata.Header.UrlKey)
                || item.ContainsKey(Models.Metadata.Header.DateKey)
                || item.ContainsKey(Models.Metadata.Header.CommentKey))
            {
                metadataFile.Credits = new Credits
                {
                    Author = item.ReadString(Models.Metadata.Header.AuthorKey),
                    Version = item.ReadString(Models.Metadata.Header.VersionKey),
                    Email = item.ReadString(Models.Metadata.Header.EmailKey),
                    Homepage = item.ReadString(Models.Metadata.Header.HomepageKey),
                    Url = item.ReadString(Models.Metadata.Header.UrlKey),
                    Date = item.ReadString(Models.Metadata.Header.DateKey),
                    Comment = item.ReadString(Models.Metadata.Header.CommentKey),
                };
            }

            if (item.ContainsKey(Models.Metadata.Header.DatVersionKey)
                || item.ContainsKey(Models.Metadata.Header.PluginKey)
                || item.ContainsKey(Models.Metadata.Header.ForceMergingKey))
            {
                metadataFile.Dat = new Dat
                {
                    Version = item.ReadString(Models.Metadata.Header.DatVersionKey),
                    Plugin = item.ReadString(Models.Metadata.Header.PluginKey),
                    Split = item.ReadString(Models.Metadata.Header.ForceMergingKey) == "split" ? "yes" : "no",
                    Merge = item.ReadString(Models.Metadata.Header.ForceMergingKey) == "merge" ? "yes" : "no",
                };
            }

            if (item.ContainsKey(Models.Metadata.Header.RefNameKey)
                || item.ContainsKey(Models.Metadata.Header.EmulatorVersionKey))
            {
                metadataFile.Emulator = new Emulator
                {
                    RefName = item.ReadString(Models.Metadata.Header.RefNameKey),
                    Version = item.ReadString(Models.Metadata.Header.EmulatorVersionKey),
                };
            }

            return metadataFile;
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Machine"/> to an array of <cref="Models.RomCenter.Rom"/>
        /// </summary>
        private static Rom[] ConvertMachineFromInternalModel(Models.Metadata.Machine item)
        {
            var roms = item.Read<Models.Metadata.Rom[]>(Models.Metadata.Machine.RomKey);
            if (roms == null)
                return Array.Empty<Rom>();

            return roms
                .Where(r => r != null)
                .Select(rom => ConvertFromInternalModel(rom, item))
                .ToArray();
        }

        /// <summary>
        /// Convert from <cref="Models.Metadata.Rom"/> to <cref="Models.RomCenter.Rom"/>
        /// </summary>
        private static Rom ConvertFromInternalModel(Models.Metadata.Rom item, Models.Metadata.Machine parent)
        {
            var row = new Rom
            {
                RomName = item.ReadString(Models.Metadata.Rom.NameKey),
                RomCRC = item.ReadString(Models.Metadata.Rom.CRCKey),
                RomSize = item.ReadString(Models.Metadata.Rom.SizeKey),
                MergeName = item.ReadString(Models.Metadata.Rom.MergeKey),

                ParentName = parent.ReadString(Models.Metadata.Machine.RomOfKey),
                //ParentDescription = parent.ReadString(Models.Metadata.Machine.ParentDescriptionKey), // This is unmappable
                GameName = parent.ReadString(Models.Metadata.Machine.NameKey),
                GameDescription = parent.ReadString(Models.Metadata.Machine.DescriptionKey),
            };
            return row;
        }

        #endregion
    }
}