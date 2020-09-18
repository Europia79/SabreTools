﻿using System;
using System.Collections.Generic;
using System.IO;

using SabreTools.Library.DatItems;

namespace SabreTools.Library.FileTypes
{
    /// <summary>
    /// Represents a TorrentLRZip archive for reading and writing
    /// </summary>
    /// TODO: Implement from source at https://github.com/ckolivas/lrzip
    public class LRZipArchive : BaseArchive
    {
        #region Constructors

        /// <summary>
        /// Create a new LRZipArchive with no base file
        /// </summary>
        public LRZipArchive()
            : base()
        {
            this.Type = FileType.LRZipArchive;
        }

        /// <summary>
        /// Create a new LRZipArchive from the given file
        /// </summary>
        /// <param name="filename">Name of the file to use as an archive</param>
        /// <param name="getHashes">True if hashes for this file should be calculated, false otherwise (default)</param>
        public LRZipArchive(string filename, bool getHashes = false)
            : base(filename, getHashes)
        {
            this.Type = FileType.LRZipArchive;
        }

        #endregion

        #region Extraction

        /// <summary>
        /// Attempt to extract a file as an archive
        /// </summary>
        /// <param name="outDir">Output directory for archive extraction</param>
        /// <returns>True if the extraction was a success, false otherwise</returns>
        public override bool CopyAll(string outDir)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Attempt to extract a file from an archive
        /// </summary>
        /// <param name="entryName">Name of the entry to be extracted</param>
        /// <param name="outDir">Output directory for archive extraction</param>
        /// <returns>Name of the extracted file, null on error</returns>
        public override string CopyToFile(string entryName, string outDir)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Attempt to extract a stream from an archive
        /// </summary>
        /// <param name="entryName">Name of the entry to be extracted</param>
        /// <param name="realEntry">Output representing the entry name that was found</param>
        /// <returns>MemoryStream representing the entry, null on error</returns>
        public override (MemoryStream, string) CopyToStream(string entryName)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Information

        /// <summary>
        /// Generate a list of DatItem objects from the header values in an archive
        /// </summary>
        /// <returns>List of DatItem objects representing the found data</returns>
        public override List<BaseFile> GetChildren()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Generate a list of empty folders in an archive
        /// </summary>
        /// <param name="input">Input file to get data from</param>
        /// <returns>List of empty folders in the archive</returns>
        public override List<string> GetEmptyFolders()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Check whether the input file is a standardized format
        /// </summary>
        public override bool IsTorrent()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Writing

        /// <summary>
        /// Write an input file to a torrent LRZip file
        /// </summary>
        /// <param name="inputFile">Input filename to be moved</param>
        /// <param name="outDir">Output directory to build to</param>
        /// <param name="rom">DatItem representing the new information</param>
        /// <returns>True if the write was a success, false otherwise</returns>
        /// <remarks>This works for now, but it can be sped up by using Ionic.Zip or another zlib wrapper that allows for header values built-in. See edc's code.</remarks>
        public override bool Write(string inputFile, string outDir, Rom rom)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Write an input stream to a torrent LRZip file
        /// </summary>
        /// <param name="inputStream">Input stream to be moved</param>
        /// <param name="outDir">Output directory to build to</param>
        /// <param name="rom">DatItem representing the new information</param>
        /// <returns>True if the write was a success, false otherwise</returns>
        /// <remarks>This works for now, but it can be sped up by using Ionic.Zip or another zlib wrapper that allows for header values built-in. See edc's code.</remarks>
        public override bool Write(Stream inputStream, string outDir, Rom rom)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Write a set of input files to a torrent LRZip archive (assuming the same output archive name)
        /// </summary>
        /// <param name="inputFiles">Input files to be moved</param>
        /// <param name="outDir">Output directory to build to</param>
        /// <param name="rom">DatItem representing the new information</param>
        /// <returns>True if the archive was written properly, false otherwise</returns>
        public override bool Write(List<string> inputFiles, string outDir, List<Rom> roms)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
