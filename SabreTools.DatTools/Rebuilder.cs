using System.Collections.Generic;
using System.IO;
using System.Linq;
#if NET40_OR_GREATER || NETCOREAPP
using System.Threading.Tasks;
#endif
using SabreTools.Core;
using SabreTools.Core.Tools;
using SabreTools.DatFiles;
using SabreTools.DatItems;
using SabreTools.DatItems.Formats;
using SabreTools.FileTypes;
using SabreTools.FileTypes.Archives;
using SabreTools.Hashing;
using SabreTools.IO;
using SabreTools.Logging;
using SabreTools.Skippers;

namespace SabreTools.DatTools
{
    /// <summary>
    /// Helper methods for rebuilding from DatFiles
    /// </summary>
    public class Rebuilder
    {
        #region Logging

        /// <summary>
        /// Logging object
        /// </summary>
        private static readonly Logger logger = new();

        #endregion

        /// <summary>
        /// Process the DAT and find all matches in input files and folders assuming they're a depot
        /// </summary>
        /// <param name="datFile">Current DatFile object to rebuild from</param>
        /// <param name="inputs">List of input files/folders to check</param>
        /// <param name="outDir">Output directory to use to build to</param>
        /// <param name="date">True if the date from the DAT should be used if available, false otherwise</param>
        /// <param name="delete">True if input files should be deleted, false otherwise</param>
        /// <param name="inverse">True if the DAT should be used as a filter instead of a template, false otherwise</param>
        /// <param name="outputFormat">Output format that files should be written to</param>
        /// <returns>True if rebuilding was a success, false otherwise</returns>
        public static bool RebuildDepot(
            DatFile datFile,
            List<string> inputs,
            string outDir,
            bool date = false,
            bool delete = false,
            bool inverse = false,
            OutputFormat outputFormat = OutputFormat.Folder)
        {
            #region Perform setup

            // If the DAT is not populated and inverse is not set, inform the user and quit
            if (datFile.Items.TotalCount == 0 && !inverse)
            {
                logger.User("No entries were found to rebuild, exiting...");
                return false;
            }

            // Check that the output directory exists
            outDir = outDir.Ensure(create: true);

            // Now we want to get forcepack flag if it's not overridden
            if (outputFormat == OutputFormat.Folder && datFile.Header.GetFieldValue<PackingFlag>(Models.Metadata.Header.ForcePackingKey) != PackingFlag.None)
                outputFormat = GetOutputFormat(datFile.Header.GetFieldValue<PackingFlag>(Models.Metadata.Header.ForcePackingKey));

            #endregion

            bool success = true;

            #region Rebuild from depots in order

            string format = FromOutputFormat(outputFormat) ?? string.Empty;
            InternalStopwatch watch = new($"Rebuilding all files to {format}");

            // Now loop through and get only directories from the input paths
            List<string> directories = [];
#if NET452_OR_GREATER || NETCOREAPP
            Parallel.ForEach(inputs, Globals.ParallelOptions, input =>
#elif NET40_OR_GREATER
            Parallel.ForEach(inputs, input =>
#else
            foreach (var input in inputs)
#endif
            {
                // Add to the list if the input is a directory
                if (Directory.Exists(input))
                {
                    logger.Verbose($"Adding depot: {input}");
                    lock (directories)
                    {
                        directories.Add(input);
                    }
                }
#if NET40_OR_GREATER || NETCOREAPP
            });
#else
            }
#endif

            // If we don't have any directories, we want to exit
            if (directories.Count == 0)
                return success;

            // Now that we have a list of depots, we want to bucket the input DAT by SHA-1
            datFile.Items.BucketBy(ItemKey.SHA1, DedupeType.None);

            // Then we want to loop through each of the hashes and see if we can rebuild
            var keys = datFile.Items.SortedKeys.ToList();
            foreach (string hash in keys)
            {
                // Pre-empt any issues that could arise from string length
                if (hash.Length != Constants.SHA1Length)
                    continue;

                logger.User($"Checking hash '{hash}'");

                // Get the extension path for the hash
                string? subpath = Utilities.GetDepotPath(hash, datFile.Header.InputDepot?.Depth ?? 0);
                if (subpath == null)
                    continue;

                // Find the first depot that includes the hash
                string? foundpath = null;
                foreach (string directory in directories)
                {
                    if (System.IO.File.Exists(Path.Combine(directory, subpath)))
                    {
                        foundpath = Path.Combine(directory, subpath);
                        break;
                    }
                }

                // If we didn't find a path, then we continue
                if (foundpath == null)
                    continue;

                // If we have a path, we want to try to get the rom information
                GZipArchive archive = new(foundpath);
                BaseFile? fileinfo = archive.GetTorrentGZFileInfo();

                // If the file information is null, then we continue
                if (fileinfo == null)
                    continue;

                // Ensure we are sorted correctly (some other calls can change this)
                datFile.Items.BucketBy(ItemKey.SHA1, DedupeType.None);

                // If there are no items in the hash, we continue
                var items = datFile.Items[hash];
                if (items == null || items.Count == 0)
                    continue;

                // Otherwise, we rebuild that file to all locations that we need to
                bool usedInternally;
                if (items[0].GetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey) == ItemType.Disk)
                    usedInternally = RebuildIndividualFile(datFile, new Disk(fileinfo), foundpath, outDir, date, inverse, outputFormat, isZip: false);
                else if (items[0].GetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey) == ItemType.File)
                    usedInternally = RebuildIndividualFile(datFile, new DatItems.Formats.File(fileinfo), foundpath, outDir, date, inverse, outputFormat, isZip: false);
                else if (items[0].GetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey) == ItemType.Media)
                    usedInternally = RebuildIndividualFile(datFile, new Media(fileinfo), foundpath, outDir, date, inverse, outputFormat, isZip: false);
                else
                    usedInternally = RebuildIndividualFile(datFile, new Rom(fileinfo), foundpath, outDir, date, inverse, outputFormat, isZip: false);

                // If we are supposed to delete the depot file, do so
                if (delete && usedInternally)
                    System.IO.File.Delete(foundpath);
            }

            watch.Stop();

            #endregion

            return success;
        }

        /// <summary>
        /// Process the DAT and find all matches in input files and folders
        /// </summary>
        /// <param name="datFile">Current DatFile object to rebuild from</param>
        /// <param name="inputs">List of input files/folders to check</param>
        /// <param name="outDir">Output directory to use to build to</param>
        /// <param name="quickScan">True to enable external scanning of archives, false otherwise</param>
        /// <param name="date">True if the date from the DAT should be used if available, false otherwise</param>
        /// <param name="delete">True if input files should be deleted, false otherwise</param>
        /// <param name="inverse">True if the DAT should be used as a filter instead of a template, false otherwise</param>
        /// <param name="outputFormat">Output format that files should be written to</param>
        /// <param name="asFiles">TreatAsFiles representing special format scanning</param>
        /// <returns>True if rebuilding was a success, false otherwise</returns>
        public static bool RebuildGeneric(
            DatFile datFile,
            List<string> inputs,
            string outDir,
            bool quickScan = false,
            bool date = false,
            bool delete = false,
            bool inverse = false,
            OutputFormat outputFormat = OutputFormat.Folder,
            TreatAsFile asFiles = 0x00)
        {
            #region Perform setup

            // If the DAT is not populated and inverse is not set, inform the user and quit
            if (datFile.Items.TotalCount == 0 && !inverse)
            {
                logger.User("No entries were found to rebuild, exiting...");
                return false;
            }

            // Check that the output directory exists
            if (!Directory.Exists(outDir))
            {
                Directory.CreateDirectory(outDir);
                outDir = Path.GetFullPath(outDir);
            }

            // Now we want to get forcepack flag if it's not overridden
            if (outputFormat == OutputFormat.Folder && datFile.Header.GetFieldValue<PackingFlag>(Models.Metadata.Header.ForcePackingKey) != PackingFlag.None)
                outputFormat = GetOutputFormat(datFile.Header.GetFieldValue<PackingFlag>(Models.Metadata.Header.ForcePackingKey));


            #endregion

            bool success = true;

            #region Rebuild from sources in order

            string format = FromOutputFormat(outputFormat) ?? string.Empty;
            InternalStopwatch watch = new($"Rebuilding all files to {format}");

            // Now loop through all of the files in all of the inputs
            foreach (string input in inputs)
            {
                // If the input is a file
                if (System.IO.File.Exists(input))
                {
                    logger.User($"Checking file: {input}");
                    bool rebuilt = RebuildGenericHelper(datFile, input, outDir, quickScan, date, inverse, outputFormat, asFiles);

                    // If we are supposed to delete the file, do so
                    if (delete && rebuilt)
                        System.IO.File.Delete(input);
                }

                // If the input is a directory
                else if (Directory.Exists(input))
                {
                    logger.Verbose($"Checking directory: {input}");
#if NET20 || NET35
                    foreach (string file in Directory.GetFiles(input, "*"))
#else
                    foreach (string file in Directory.EnumerateFiles(input, "*", SearchOption.AllDirectories))
#endif
                    {
                        logger.User($"Checking file: {file}");
                        bool rebuilt = RebuildGenericHelper(datFile, file, outDir, quickScan, date, inverse, outputFormat, asFiles);

                        // If we are supposed to delete the file, do so
                        if (delete && rebuilt)
                            System.IO.File.Delete(file);
                    }
                }
            }

            watch.Stop();

            #endregion

            return success;
        }

        /// <summary>
        /// Attempt to add a file to the output if it matches
        /// </summary>
        /// <param name="datFile">Current DatFile object to rebuild from</param>
        /// <param name="file">Name of the file to process</param>
        /// <param name="outDir">Output directory to use to build to</param>
        /// <param name="quickScan">True to enable external scanning of archives, false otherwise</param>
        /// <param name="date">True if the date from the DAT should be used if available, false otherwise</param>
        /// <param name="inverse">True if the DAT should be used as a filter instead of a template, false otherwise</param>
        /// <param name="outputFormat">Output format that files should be written to</param>
        /// <param name="asFiles">TreatAsFiles representing special format scanning</param>
        /// <returns>True if the file was used to rebuild, false otherwise</returns>
        private static bool RebuildGenericHelper(
            DatFile datFile,
            string file,
            string outDir,
            bool quickScan,
            bool date,
            bool inverse,
            OutputFormat outputFormat,
            TreatAsFile asFiles)
        {
            // If we somehow have a null filename, return
            if (file == null)
                return false;

            // Set the deletion variables
            bool usedExternally = false, usedInternally = false;

            // Create an empty list of BaseFile for archive entries
            List<BaseFile>? entries = null;

            // Get the TGZ and TXZ status for later
            GZipArchive tgz = new(file);
            XZArchive txz = new(file);
            bool isSingleTorrent = tgz.IsTorrent() || txz.IsTorrent();

            // Get the base archive first
            BaseArchive? archive = BaseArchive.Create(file);

            // Now get all extracted items from the archive
            if (archive != null)
            {
                archive.AvailableHashTypes = quickScan ? [HashType.CRC32] : [HashType.CRC32, HashType.MD5, HashType.SHA1];
                entries = archive.GetChildren();
            }

            // If the entries list is null, we encountered an error or have a file and should scan externally
            if (entries == null && System.IO.File.Exists(file))
            {
                BaseFile? internalFileInfo = BaseFile.GetInfo(file, asFiles: asFiles);

                // Create the correct DatItem
                DatItem? internalDatItem;
                if (internalFileInfo == null)
                    internalDatItem = null;
#if NETFRAMEWORK
                else if (internalFileInfo.Type == FileType.AaruFormat && (asFiles & TreatAsFile.AaruFormat) == 0)
#else
                else if (internalFileInfo.Type == FileType.AaruFormat && !asFiles.HasFlag(TreatAsFile.AaruFormat))
#endif
                    internalDatItem = new Media(internalFileInfo);
#if NETFRAMEWORK
                else if (internalFileInfo.Type == FileType.CHD && (asFiles & TreatAsFile.CHD) == 0)
#else
                else if (internalFileInfo.Type == FileType.CHD && !asFiles.HasFlag(TreatAsFile.CHD))
#endif
                    internalDatItem = new Disk(internalFileInfo);
                else
                    internalDatItem = new Rom(internalFileInfo);

                if (internalDatItem != null)
                    usedExternally = RebuildIndividualFile(datFile, internalDatItem, file, outDir, date, inverse, outputFormat);
            }
            // Otherwise, loop through the entries and try to match
            else if (entries != null)
            {
                foreach (BaseFile entry in entries)
                {
                    DatItem? internalDatItem = DatItem.Create(entry);
                    if (internalDatItem == null)
                        continue;

                    usedInternally |= RebuildIndividualFile(datFile, internalDatItem, file, outDir, date, inverse, outputFormat, !isSingleTorrent /* isZip */);
                }
            }

            return usedExternally || usedInternally;
        }

        /// <summary>
        /// Find duplicates and rebuild individual files to output
        /// </summary>
        /// <param name="datFile">Current DatFile object to rebuild from</param>
        /// <param name="datItem">Information for the current file to rebuild from</param>
        /// <param name="file">Name of the file to process</param>
        /// <param name="outDir">Output directory to use to build to</param>
        /// <param name="date">True if the date from the DAT should be used if available, false otherwise</param>
        /// <param name="inverse">True if the DAT should be used as a filter instead of a template, false otherwise</param>
        /// <param name="outputFormat">Output format that files should be written to</param>
        /// <param name="isZip">True if the input file is an archive, false if the file is TGZ/TXZ, null otherwise</param>
        /// <returns>True if the file was able to be rebuilt, false otherwise</returns>
        private static bool RebuildIndividualFile(
            DatFile datFile,
            DatItem datItem,
            string file,
            string outDir,
            bool date,
            bool inverse,
            OutputFormat outputFormat,
            bool? isZip = null)
        {
            // Set the initial output value
            bool rebuilt = false;

            // If the DatItem is a Disk or Media, force rebuilding to a folder except if TGZ or TXZ
            if ((datItem is Disk || datItem is Media)
                && !(outputFormat == OutputFormat.TorrentGzip || outputFormat == OutputFormat.TorrentGzipRomba)
                && !(outputFormat == OutputFormat.TorrentXZ || outputFormat == OutputFormat.TorrentXZRomba))
            {
                outputFormat = OutputFormat.Folder;
            }

            // If we have a Disk, File, or Media, change it into a Rom for later use
            if (datItem is Disk disk)
                datItem = disk.ConvertToRom();
            else if (datItem is DatItems.Formats.File fileItem)
                datItem = fileItem.ConvertToRom();
            else if (datItem is Media media)
                datItem = media.ConvertToRom();

            // Prepopluate a key string
            string crc = (datItem as Rom)!.GetFieldValue<string?>(Models.Metadata.Rom.CRCKey) ?? string.Empty;

            // Try to get the stream for the file
            if (!GetFileStream(datItem, file, isZip, out Stream? fileStream))
                return false;

            // If either we have duplicates or we're filtering
            if (ShouldRebuild(datFile, datItem, fileStream, inverse, out ConcurrentList<DatItem> dupes))
            {
                // If we have a very specific TGZ->TGZ case, just copy it accordingly
                if (RebuildTorrentGzip(datFile, datItem, file, outDir, outputFormat, isZip))
                    return true;

                // If we have a very specific TXZ->TXZ case, just copy it accordingly
                if (RebuildTorrentXz(datFile, datItem, file, outDir, outputFormat, isZip))
                    return true;

                logger.User($"{(inverse ? "No matches" : "Matches")} found for '{Path.GetFileName(datItem.GetName() ?? datItem.GetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey).ToString())}', rebuilding accordingly...");
                rebuilt = true;

                // Special case for partial packing mode
                bool shouldCheck = false;
                if (outputFormat == OutputFormat.Folder && datFile.Header.GetFieldValue<PackingFlag>(Models.Metadata.Header.ForcePackingKey) == PackingFlag.Partial)
                {
                    shouldCheck = true;
                    datFile.Items.BucketBy(ItemKey.Machine, DedupeType.None, lower: false);
                }

                // Now loop through the list and rebuild accordingly
                foreach (DatItem item in dupes)
                {
                    // If we don't have a proper machine
                    if (item.GetFieldValue<Machine>(DatItem.MachineKey)!?.GetFieldValue<string?>(Models.Metadata.Machine.NameKey) == null || !datFile.Items.ContainsKey(item.GetFieldValue<Machine>(DatItem.MachineKey)!.GetFieldValue<string?>(Models.Metadata.Machine.NameKey)!))
                        continue;

                    // If we should check for the items in the machine
                    var items = datFile.Items[item.GetFieldValue<Machine>(DatItem.MachineKey)!.GetFieldValue<string?>(Models.Metadata.Machine.NameKey)!];
                    if (shouldCheck && items!.Count > 1)
                        outputFormat = OutputFormat.Folder;
                    else if (shouldCheck && items!.Count == 1)
                        outputFormat = OutputFormat.ParentFolder;

                    // Get the output archive, if possible
                    Folder? outputArchive = GetPreconfiguredFolder(datFile, date, outputFormat);

                    // Now rebuild to the output file
                    outputArchive!.Write(fileStream, outDir, (item as Rom)!.ConvertToBaseFile());
                }

                // Close the input stream
                fileStream?.Dispose();
            }

            // Now we want to take care of headers, if applicable
            if (datFile.Header.GetFieldValue<string?>(Models.Metadata.Header.HeaderKey) != null)
            {
                // Check to see if we have a matching header first
                SkipperMatch.Init();
                Rule rule = SkipperMatch.GetMatchingRule(fileStream, Path.GetFileNameWithoutExtension(datFile.Header.GetFieldValue<string?>(Models.Metadata.Header.HeaderKey)!));

                // If there's a match, create the new file to write
                if (rule.Tests != null && rule.Tests.Length != 0)
                {
                    // If the file could be transformed correctly
                    MemoryStream transformStream = new();
                    if (rule.TransformStream(fileStream, transformStream, keepReadOpen: true, keepWriteOpen: true))
                    {
                        // Get the file informations that we will be using
                        Rom headerless = new(BaseFile.GetInfo(transformStream, keepReadOpen: true));

                        // If we have duplicates and we're not filtering
                        if (ShouldRebuild(datFile, headerless, transformStream, false, out dupes))
                        {
                            logger.User($"Headerless matches found for '{Path.GetFileName(datItem.GetName() ?? datItem.GetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey).ToString())}', rebuilding accordingly...");
                            rebuilt = true;

                            // Now loop through the list and rebuild accordingly
                            foreach (DatItem item in dupes)
                            {
                                // Create a headered item to use as well
                                datItem.CopyMachineInformation(item);
                                datItem.SetName($"{datItem.GetName()}_{crc}");

                                // Get the output archive, if possible
                                Folder? outputArchive = GetPreconfiguredFolder(datFile, date, outputFormat);

                                // Now rebuild to the output file
                                bool eitherSuccess = false;
                                eitherSuccess |= outputArchive!.Write(transformStream, outDir, (item as Rom)!.ConvertToBaseFile());
                                eitherSuccess |= outputArchive.Write(fileStream, outDir, (datItem as Rom)!.ConvertToBaseFile());

                                // Now add the success of either rebuild
                                rebuilt &= eitherSuccess;
                            }
                        }
                    }

                    // Dispose of the stream
                    transformStream?.Dispose();
                }

                // Dispose of the stream
                fileStream?.Dispose();
            }

            return rebuilt;
        }

        /// <summary>
        /// Get the rebuild state for a given item
        /// </summary>
        /// <param name="datFile">Current DatFile object to rebuild from</param>
        /// <param name="datItem">Information for the current file to rebuild from</param>
        /// <param name="stream">Stream representing the input file</param>
        /// <param name="inverse">True if the DAT should be used as a filter instead of a template, false otherwise</param>
        /// <param name="dupes">Output list of duplicate items to rebuild to</param>
        /// <returns>True if the item should be rebuilt, false otherwise</returns>
        private static bool ShouldRebuild(DatFile datFile, DatItem datItem, Stream? stream, bool inverse, out ConcurrentList<DatItem> dupes)
        {
            // Find if the file has duplicates in the DAT
            dupes = datFile.Items.GetDuplicates(datItem);
            bool hasDuplicates = dupes.Count > 0;

            // If we have duplicates but we're filtering
            if (hasDuplicates && inverse)
            {
                return false;
            }

            // If we have duplicates without filtering
            else if (hasDuplicates && !inverse)
            {
                return true;
            }

            // If we have no duplicates and we're filtering
            else if (!hasDuplicates && inverse)
            {
                string? machinename = null;

                // Get the item from the current file
                Rom item = new(BaseFile.GetInfo(stream, keepReadOpen: true));
                item.GetFieldValue<Machine>(DatItem.MachineKey)!.SetFieldValue<string?>(Models.Metadata.Machine.DescriptionKey, Path.GetFileNameWithoutExtension(item.GetName()));
                item.GetFieldValue<Machine>(DatItem.MachineKey)!.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, Path.GetFileNameWithoutExtension(item.GetName()));

                // If we are coming from an archive, set the correct machine name
                if (machinename != null)
                {
                    item.GetFieldValue<Machine>(DatItem.MachineKey)!.SetFieldValue<string?>(Models.Metadata.Machine.DescriptionKey, machinename);
                    item.GetFieldValue<Machine>(DatItem.MachineKey)!.SetFieldValue<string?>(Models.Metadata.Machine.NameKey, machinename);
                }

                dupes.Add(item);
                return true;
            }

            // If we have no duplicates and we're not filtering
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Rebuild from TorrentGzip to TorrentGzip
        /// </summary>
        /// <param name="datFile">Current DatFile object to rebuild from</param>
        /// <param name="datItem">Information for the current file to rebuild from</param>
        /// <param name="file">Name of the file to process</param>
        /// <param name="outDir">Output directory to use to build to</param>
        /// <param name="outputFormat">Output format that files should be written to</param>
        /// <param name="isZip">True if the input file is an archive, false if the file is TGZ, null otherwise</param>
        /// <returns>True if rebuilt properly, false otherwise</returns>
        private static bool RebuildTorrentGzip(DatFile datFile, DatItem datItem, string file, string outDir, OutputFormat outputFormat, bool? isZip)
        {
            // If we have a very specific TGZ->TGZ case, just copy it accordingly
            GZipArchive tgz = new(file);
            BaseFile? tgzRom = tgz.GetTorrentGZFileInfo();
            if (isZip == false && tgzRom != null && (outputFormat == OutputFormat.TorrentGzip || outputFormat == OutputFormat.TorrentGzipRomba))
            {
                logger.User($"Matches found for '{Path.GetFileName(datItem.GetName() ?? string.Empty)}', rebuilding accordingly...");

                // Get the proper output path
                string sha1 = (datItem as Rom)!.GetFieldValue<string?>(Models.Metadata.Rom.SHA1Key) ?? string.Empty;
                if (outputFormat == OutputFormat.TorrentGzipRomba)
                    outDir = Path.Combine(outDir, Utilities.GetDepotPath(sha1, datFile.Header.OutputDepot?.Depth ?? 0) ?? string.Empty);
                else
                    outDir = Path.Combine(outDir, sha1 + ".gz");

                // Make sure the output folder is created
                string? dir = Path.GetDirectoryName(outDir);
                if (dir != null)
                    Directory.CreateDirectory(dir);

                // Now copy the file over
                try
                {
                    System.IO.File.Copy(file, outDir);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// Rebuild from TorrentXz to TorrentXz
        /// </summary>
        /// <param name="datFile">Current DatFile object to rebuild from</param>
        /// <param name="datItem">Information for the current file to rebuild from</param>
        /// <param name="file">Name of the file to process</param>
        /// <param name="outDir">Output directory to use to build to</param>
        /// <param name="outputFormat">Output format that files should be written to</param>
        /// <param name="isZip">True if the input file is an archive, false if the file is TXZ, null otherwise</param>
        /// <returns>True if rebuilt properly, false otherwise</returns>
        private static bool RebuildTorrentXz(DatFile datFile, DatItem datItem, string file, string outDir, OutputFormat outputFormat, bool? isZip)
        {
            // If we have a very specific TGZ->TGZ case, just copy it accordingly
            XZArchive txz = new(file);
            BaseFile? txzRom = txz.GetTorrentXZFileInfo();
            if (isZip == false && txzRom != null && (outputFormat == OutputFormat.TorrentXZ || outputFormat == OutputFormat.TorrentXZRomba))
            {
                logger.User($"Matches found for '{Path.GetFileName(datItem.GetName() ?? string.Empty)}', rebuilding accordingly...");

                // Get the proper output path
                string sha1 = (datItem as Rom)!.GetFieldValue<string?>(Models.Metadata.Rom.SHA1Key) ?? string.Empty;
                if (outputFormat == OutputFormat.TorrentXZRomba)
                    outDir = Path.Combine(outDir, Utilities.GetDepotPath(sha1, datFile.Header.OutputDepot?.Depth ?? 0) ?? string.Empty).Replace(".gz", ".xz");
                else
                    outDir = Path.Combine(outDir, sha1 + ".xz");

                // Make sure the output folder is created
                string? dir = Path.GetDirectoryName(outDir);
                if (dir != null)
                    Directory.CreateDirectory(dir);

                // Now copy the file over
                try
                {
                    System.IO.File.Copy(file, outDir);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// Get the Stream related to a file
        /// </summary>
        /// <param name="datItem">Information for the current file to rebuild from</param>
        /// <param name="file">Name of the file to process</param>
        /// <param name="isZip">Non-null if the input file is an archive</param>
        /// <param name="stream">Output stream representing the opened file</param>
        /// <returns>True if the stream opening succeeded, false otherwise</returns>
        private static bool GetFileStream(DatItem datItem, string file, bool? isZip, out Stream? stream)
        {
            // Get a generic stream for the file
            stream = null;

            // If we have a zipfile, extract the stream to memory
            if (isZip != null)
            {
                BaseArchive? archive = BaseArchive.Create(file);
                if (archive != null)
                    (stream, _) = archive.CopyToStream(datItem.GetName() ?? datItem.GetFieldValue<ItemType>(Models.Metadata.DatItem.TypeKey).ToString());
            }
            // Otherwise, just open the filestream
            else
            {
                stream = System.IO.File.OpenRead(file);
            }

            // If the stream is null, then continue
            if (stream == null)
                return false;

            // Seek to the beginning of the stream
            if (stream.CanSeek)
                stream.Seek(0, SeekOrigin.Begin);

            return true;
        }

        /// <summary>
        /// Get the default OutputFormat associated with each PackingFlag
        /// </summary>
        private static OutputFormat GetOutputFormat(PackingFlag packing)
        {
            return packing switch
            {
                PackingFlag.Zip => OutputFormat.TorrentZip,
                PackingFlag.Unzip => OutputFormat.Folder,
                PackingFlag.Partial => OutputFormat.Folder,
                PackingFlag.Flat => OutputFormat.ParentFolder,
                PackingFlag.None => OutputFormat.Folder,
                _ => OutputFormat.Folder,
            };
        }

        /// <summary>
        /// Get preconfigured Folder for rebuilding
        /// </summary>
        /// <param name="datFile">Current DatFile object to rebuild from</param>
        /// <param name="date">True if the date from the DAT should be used if available, false otherwise</param>
        /// <param name="outputFormat">Output format that files should be written to</param>
        /// <returns>Folder configured with proper flags</returns>
        private static Folder? GetPreconfiguredFolder(DatFile datFile, bool date, OutputFormat outputFormat)
        {
            Folder? outputArchive = Folder.Create(outputFormat);
            if (outputArchive is BaseArchive baseArchive && date)
                baseArchive.UseDates = date;

            // Set the depth fields where appropriate
            if (outputArchive is GZipArchive gzipArchive)
                gzipArchive.Depth = datFile.Header.OutputDepot?.Depth ?? 0;
            else if (outputArchive is XZArchive xzArchive)
                xzArchive.Depth = datFile.Header.OutputDepot?.Depth ?? 0;

            return outputArchive;
        }

        /// <summary>
        /// Get string value from input OutputFormat
        /// </summary>
        /// <param name="itemType">OutputFormat to get value from</param>
        /// <returns>String value corresponding to the OutputFormat</returns>
        private static string? FromOutputFormat(OutputFormat itemType)
        {
            return itemType switch
            {
                OutputFormat.Folder => "directory",
                OutputFormat.ParentFolder => "directory",
                OutputFormat.TapeArchive => "TAR",
                OutputFormat.Torrent7Zip => "Torrent7Z",
                OutputFormat.TorrentGzip => "TorrentGZ",
                OutputFormat.TorrentGzipRomba => "TorrentGZ",
                OutputFormat.TorrentLRZip => "TorrentLRZ",
                OutputFormat.TorrentRar => "TorrentRAR",
                OutputFormat.TorrentXZ => "TorrentXZ",
                OutputFormat.TorrentXZRomba => "TorrentXZ",
                OutputFormat.TorrentZip => "TorrentZip",
                _ => null,
            };
        }
    }
}