﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Xml;

using SabreTools.Library.Data;
using SabreTools.Library.DatItems;
using SabreTools.Library.Tools;

#if MONO
using System.IO;
#else
using Alphaleonis.Win32.Filesystem;

using FileStream = System.IO.FileStream;
using StreamWriter = System.IO.StreamWriter;
#endif
using NaturalSort;

namespace SabreTools.Library.DatFiles
{
	/// <summary>
	/// Represents parsing and writing of a SofwareList, M1, or MAME XML DAT
	/// </summary>
	/// TODO: Verify that all read/write for this DatFile type is correct
	internal class SoftwareList : DatFile
	{
		/// <summary>
		/// Constructor designed for casting a base DatFile
		/// </summary>
		/// <param name="datFile">Parent DatFile to copy from</param>
		public SoftwareList(DatFile datFile)
			: base(datFile, cloneHeader: false)
		{
		}

		/// <summary>
		/// Parse an SofwareList XML DAT and return all found games and roms within
		/// </summary>
		/// <param name="filename">Name of the file to be parsed</param>
		/// <param name="sysid">System ID for the DAT</param>
		/// <param name="srcid">Source ID for the DAT</param>
		/// <param name="keep">True if full pathnames are to be kept, false otherwise (default)</param>
		/// <param name="clean">True if game names are sanitized, false otherwise (default)</param>
		/// <param name="remUnicode">True if we should remove non-ASCII characters from output, false otherwise (default)</param>
		/// <remarks>
		/// </remarks>
		public override void ParseFile(
			// Standard Dat parsing
			string filename,
			int sysid,
			int srcid,

			// Miscellaneous
			bool keep,
			bool clean,
			bool remUnicode)
		{
			// All XML-derived DATs share a lot in common so it just calls one implementation
			// TODO: Use the following implementation instead of passing to Logiqx
			new Logiqx(this, false).ParseFile(filename, sysid, srcid, keep, clean, remUnicode);
			return;

			// Prepare all internal variables
			Encoding enc = Utilities.GetEncoding(filename);
			XmlReader xtr = Utilities.GetXmlTextReader(filename);

			// If we got a null reader, just return
			if (xtr == null)
			{
				return;
			}

			// Otherwise, read the file to the end
			try
			{
				xtr.MoveToContent();
				while (!xtr.EOF)
				{
					// We only want elements
					if (xtr.NodeType != XmlNodeType.Element)
					{
						xtr.Read();
						continue;
					}

					switch (xtr.Name)
					{
						case "softwarelist":
							Name = (String.IsNullOrWhiteSpace(Name) ? xtr.GetAttribute("name") ?? "" : Name);
							Description = (String.IsNullOrWhiteSpace(Description) ? xtr.GetAttribute("description") ?? "" : Description);
							if (ForceMerging == ForceMerging.None)
							{
								ForceMerging = Utilities.GetForceMerging(xtr.GetAttribute("forcemerging"));
							}
							if (ForceNodump == ForceNodump.None)
							{
								ForceNodump = Utilities.GetForceNodump(xtr.GetAttribute("forcenodump"));
							}
							if (ForcePacking == ForcePacking.None)
							{
								ForcePacking = Utilities.GetForcePacking(xtr.GetAttribute("forcepacking"));
							}
							xtr.Read();
							break;
						// We want to process the entire subtree of the machine
						case "software":
							ReadSoftware(xtr.ReadSubtree(), filename, sysid, srcid, keep, clean, remUnicode);

							// Skip the software now that we've processed it
							xtr.Skip();
							break;
						default:
							xtr.Read();
							break;
					}
				}
			}
			catch (Exception ex)
			{
				Globals.Logger.Warning("Exception found while parsing '{0}': {1}", filename, ex);

				// For XML errors, just skip the affected node
				xtr?.Read();
			}

			xtr.Dispose();
		}

		/// <summary>
		/// Read software information
		/// </summary>
		/// <param name="filename">Name of the file to be parsed</param>
		/// <param name="sysid">System ID for the DAT</param>
		/// <param name="srcid">Source ID for the DAT</param>
		/// <param name="keep">True if full pathnames are to be kept, false otherwise (default)</param>
		/// <param name="clean">True if game names are sanitized, false otherwise (default)</param>
		/// <param name="remUnicode">True if we should remove non-ASCII characters from output, false otherwise (default)</param>
		private void ReadSoftware(
			XmlReader reader,

			// Standard Dat parsing
			string filename,
			int sysid,
			int srcid,

			// Miscellaneous
			bool keep,
			bool clean,
			bool remUnicode)
		{
			// If we have an empty software, skip it
			if (reader == null)
			{
				return;
			}

			// Otherwise, add what is possible
			reader.MoveToContent();

			string key = "";
			string temptype = reader.Name;
			bool containsItems = false;

			// Create a new machine
			MachineType machineType = MachineType.NULL;
			if (Utilities.GetYesNo(reader.GetAttribute("isbios")) == true)
			{
				machineType |= MachineType.Bios;
			}
			if (Utilities.GetYesNo(reader.GetAttribute("isdevice")) == true)
			{
				machineType |= MachineType.Device;
			}
			if (Utilities.GetYesNo(reader.GetAttribute("ismechanical")) == true)
			{
				machineType |= MachineType.Mechanical;
			}

			Machine machine = new Machine
			{
				Name = reader.GetAttribute("name"),
				Description = reader.GetAttribute("name"),
				Supported = Utilities.GetYesNo(reader.GetAttribute("supported")), // (yes|partial|no) "yes"

				CloneOf = reader.GetAttribute("cloneof") ?? "",

				MachineType = (machineType == MachineType.NULL ? MachineType.None : machineType),
			};

			while (!reader.EOF)
			{
				// We only want elements
				if (reader.NodeType != XmlNodeType.Element)
				{
					reader.Read();
					continue;
				}

				// Get the elements from the software
				switch (reader.Name)
				{
					case "description":
						machine.Description = reader.ReadElementContentAsString();
						break;
					case "year":
						machine.Year = reader.ReadElementContentAsString();
						break;
					case "publisher":
						machine.Publisher = reader.ReadElementContentAsString();
						break;
					case "info":
						machine.Infos.Add(new Tuple<string, string>(reader.GetAttribute("name"), reader.GetAttribute("value")));

						reader.Read();
						break;
					case "sharedfeat":
						// string name = reader.GetAttribute("name");
						// string value = reader.GetAttribute("value");

						reader.Read();
						break;
					case "part": // Contains all rom and disk information
						containsItems = ReadPart(reader.ReadSubtree(), machine, filename, sysid, srcid, keep, clean, remUnicode);

						// Skip the part now that we've processed it
						reader.Skip();
						break;
					default:
						reader.Read();
						break;
				}
			}

			// If no items were found for this machine, add a Blank placeholder
			if (!containsItems)
			{
				Blank blank = new Blank()
				{
					SystemID = sysid,
					System = filename,
					SourceID = srcid,
				};

				blank.CopyMachineInformation(machine);

				// Now process and add the rom
				key = ParseAddHelper(blank, clean, remUnicode);
			}
		}

		/// <summary>
		/// Read part information
		/// </summary>
		/// <param name="filename">Name of the file to be parsed</param>
		/// <param name="sysid">System ID for the DAT</param>
		/// <param name="srcid">Source ID for the DAT</param>
		/// <param name="keep">True if full pathnames are to be kept, false otherwise (default)</param>
		/// <param name="clean">True if game names are sanitized, false otherwise (default)</param>
		/// <param name="remUnicode">True if we should remove non-ASCII characters from output, false otherwise (default)</param>
		private bool ReadPart(
			XmlReader reader,
			Machine machine,

			// Standard Dat parsing
			string filename,
			int sysid,
			int srcid,

			// Miscellaneous
			bool keep,
			bool clean,
			bool remUnicode)
		{
			string key = "", areaname = "", partname = "", partinterface = "";
			string temptype = reader.Name;
			long? areasize = null;
			List<Tuple<string, string>> features = new List<Tuple<string, string>>();
			bool containsItems = false;

			while (!reader.EOF)
			{
				// We only want elements
				if (reader.NodeType != XmlNodeType.Element)
				{
					if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "part")
					{
						partname = "";
						partinterface = "";
						features = new List<Tuple<string, string>>();
					}
					if (reader.NodeType == XmlNodeType.EndElement && (reader.Name == "dataarea" || reader.Name == "diskarea"))
					{
						areaname = "";
						areasize = null;
					}

					reader.Read();
					continue;
				}

				// Get the elements from the software
				switch (reader.Name)
				{
					case "part":
						partname = reader.GetAttribute("name");
						partinterface = reader.GetAttribute("interface");

						reader.Read();
						break;
					case "feature":
						features.Add(new Tuple<string, string>(reader.GetAttribute("name"), reader.GetAttribute("feature")));

						reader.Read();
						break;
					case "dataarea":
						areaname = reader.GetAttribute("name");
						if (reader.GetAttribute("size") != null)
						{
							if (Int64.TryParse(reader.GetAttribute("size"), out long tempas))
							{
								areasize = tempas;
							}
						}
						// string width = reader.GetAttribute("width"); // (8|16|32|64) "8"
						// string endianness = reader.GetAttribute("endianness"); // endianness (big|little) "little"
						
						reader.Read();
						break;
					case "rom": // TODO: Put this in a proper subreader under dataarea
						containsItems = true;

						// If the rom is continue or ignore, add the size to the previous rom
						if (reader.GetAttribute("loadflag") == "continue" || reader.GetAttribute("loadflag") == "ignore")
						{
							int index = this[key].Count - 1;
							DatItem lastrom = this[key][index];
							if (lastrom.Type == ItemType.Rom)
							{
								((Rom)lastrom).Size += Utilities.GetSize(reader.GetAttribute("size"));
							}
							this[key].RemoveAt(index);
							this[key].Add(lastrom);
							reader.Read();
							continue;
						}

						DatItem rom = new Rom
						{
							Name = reader.GetAttribute("name"),
							Size = Utilities.GetSize(reader.GetAttribute("size")),
							CRC = reader.GetAttribute("crc")?.ToLowerInvariant(),
							MD5 = reader.GetAttribute("md5")?.ToLowerInvariant(),
							SHA1 = reader.GetAttribute("sha1")?.ToLowerInvariant(),
							SHA256 = reader.GetAttribute("sha256")?.ToLowerInvariant(),
							SHA384 = reader.GetAttribute("sha384")?.ToLowerInvariant(),
							SHA512 = reader.GetAttribute("sha512")?.ToLowerInvariant(),
							Offset = reader.GetAttribute("offset"),
							// Value = reader.GetAttribute("value");
							ItemStatus = Utilities.GetItemStatus(reader.GetAttribute("status")),
							// LoadFlag = reader.GetAttribute("loadflag"), // (load16_byte|load16_word|load16_word_swap|load32_byte|load32_word|load32_word_swap|load32_dword|load64_word|load64_word_swap|reload|fill|continue|reload_plain|ignore)

							AreaName = areaname,
							AreaSize = areasize,
							Features = features,
							PartName = partname,
							PartInterface = partinterface,

							SystemID = sysid,
							System = filename,
							SourceID = srcid,
						};

						rom.CopyMachineInformation(machine);

						// Now process and add the rom
						key = ParseAddHelper(rom, clean, remUnicode);

						reader.Read();
						break;
					case "diskarea":
						areaname = reader.GetAttribute("name");

						reader.Read();
						break;
					case "disk": // TODO: Put this in a proper subreader under diskarea
						containsItems = true;

						DatItem disk = new Disk
						{
							Name = reader.GetAttribute("name"),
							MD5 = reader.GetAttribute("md5")?.ToLowerInvariant(),
							SHA1 = reader.GetAttribute("sha1")?.ToLowerInvariant(),
							SHA256 = reader.GetAttribute("sha256")?.ToLowerInvariant(),
							SHA384 = reader.GetAttribute("sha384")?.ToLowerInvariant(),
							SHA512 = reader.GetAttribute("sha512")?.ToLowerInvariant(),
							ItemStatus = Utilities.GetItemStatus(reader.GetAttribute("status")),
							Writable = Utilities.GetYesNo(reader.GetAttribute("writable")),

							AreaName = areaname,
							AreaSize = areasize,
							Features = features,
							PartName = partname,
							PartInterface = partinterface,

							SystemID = sysid,
							System = filename,
							SourceID = srcid,
						};

						disk.CopyMachineInformation(machine);

						// Now process and add the rom
						key = ParseAddHelper(disk, clean, remUnicode);

						reader.Read();
						break;
					case "dipswitch":
						// string name = reader.GetAttribute("name");
						// string tag = reader.GetAttribute("tag");
						// string mask = reader.GetAttribute("mask");

						// For every <dipvalue> element...
						// string name = reader.GetAttribute("name");
						// string value = reader.GetAttribute("value");
						// bool? default = Utilities.GetYesNo(reader.GetAttribute("default")); // (yes|no) "no"

						reader.Read();
						break;
					default:
						reader.Read();
						break;
				}
			}

			return containsItems;
		}

		/// <summary>
		/// Create and open an output file for writing direct from a dictionary
		/// </summary>
		/// <param name="outfile">Name of the file to write to</param>
		/// <param name="ignoreblanks">True if blank roms should be skipped on output, false otherwise (default)</param>
		/// <returns>True if the DAT was written correctly, false otherwise</returns>
		public override bool WriteToFile(string outfile, bool ignoreblanks = false)
		{
			try
			{
				Globals.Logger.User("Opening file for writing: {0}", outfile);
				FileStream fs = Utilities.TryCreate(outfile);

				// If we get back null for some reason, just log and return
				if (fs == null)
				{
					Globals.Logger.Warning("File '{0}' could not be created for writing! Please check to see if the file is writable", outfile);
					return false;
				}

				StreamWriter sw = new StreamWriter(fs, new UTF8Encoding(false));

				// Write out the header
				WriteHeader(sw);

				// Write out each of the machines and roms
				string lastgame = null;

				// Get a properly sorted set of keys
				List<string> keys = Keys;
				keys.Sort(new NaturalComparer());

				foreach (string key in keys)
				{
					List<DatItem> roms = this[key];

					// Resolve the names in the block
					roms = DatItem.ResolveNames(roms);

					for (int index = 0; index < roms.Count; index++)
					{
						DatItem rom = roms[index];

						// There are apparently times when a null rom can skip by, skip them
						if (rom.Name == null || rom.MachineName == null)
						{
							Globals.Logger.Warning("Null rom found!");
							continue;
						}

						// If we have a different game and we're not at the start of the list, output the end of last item
						if (lastgame != null && lastgame.ToLowerInvariant() != rom.MachineName.ToLowerInvariant())
						{
							WriteEndGame(sw);
						}

						// If we have a new game, output the beginning of the new item
						if (lastgame == null || lastgame.ToLowerInvariant() != rom.MachineName.ToLowerInvariant())
						{
							WriteStartGame(sw, rom);
						}

						// If we have a "null" game (created by DATFromDir or something similar), log it to file
						if (rom.Type == ItemType.Rom
							&& ((Rom)rom).Size == -1
							&& ((Rom)rom).CRC == "null")
						{
							Globals.Logger.Verbose("Empty folder found: {0}", rom.MachineName);

							lastgame = rom.MachineName;
							continue;
						}

						// Now, output the rom data
						WriteDatItem(sw, rom, ignoreblanks);

						// Set the new data to compare against
						lastgame = rom.MachineName;
					}
				}

				// Write the file footer out
				WriteFooter(sw);

				Globals.Logger.Verbose("File written!" + Environment.NewLine);
				sw.Dispose();
				fs.Dispose();
			}
			catch (Exception ex)
			{
				Globals.Logger.Error(ex.ToString());
				return false;
			}

			return true;
		}

		/// <summary>
		/// Write out DAT header using the supplied StreamWriter
		/// </summary>
		/// <param name="sw">StreamWriter to output to</param>
		/// <returns>True if the data was written, false on error</returns>
		private bool WriteHeader(StreamWriter sw)
		{
			try
			{
				string header = "<?xml version=\"1.0\"?>\n" +
							"<!DOCTYPE softwarelist SYSTEM \"softwarelist.dtd\">\n\n" +
							"<softwarelist name=\"" + HttpUtility.HtmlEncode(Name) + "\"" +
								" description=\"" + HttpUtility.HtmlEncode(Description) + "\"" +
								(ForcePacking == ForcePacking.Unzip ? " forcepacking=\"unzip\"" : "") +
								(ForcePacking == ForcePacking.Zip ? " forcepacking=\"zip\"" : "") +
								(ForceMerging == ForceMerging.Full ? " forcemerging=\"full\"" : "") +
								(ForceMerging == ForceMerging.Split ? " forcemerging=\"split\"" : "") +
								(ForceMerging == ForceMerging.Merged ? " forcemerging=\"merged\"" : "") +
								(ForceMerging == ForceMerging.NonMerged ? " forcemerging=\"nonmerged\"" : "") +
								(ForceNodump == ForceNodump.Ignore ? " forcenodump=\"ignore\"" : "") +
								(ForceNodump == ForceNodump.Obsolete ? " forcenodump=\"obsolete\"" : "") +
								(ForceNodump == ForceNodump.Required ? " forcenodump=\"required\"" : "") +
								">\n\n";

				// Write the header out
				sw.Write(header);
				sw.Flush();
			}
			catch (Exception ex)
			{
				Globals.Logger.Error(ex.ToString());
				return false;
			}

			return true;
		}

		/// <summary>
		/// Write out Game start using the supplied StreamWriter
		/// </summary>
		/// <param name="sw">StreamWriter to output to</param>
		/// <param name="rom">DatItem object to be output</param>
		/// <returns>True if the data was written, false on error</returns>
		private bool WriteStartGame(StreamWriter sw, DatItem rom)
		{
			try
			{
				// No game should start with a path separator
				if (rom.MachineName.StartsWith(Path.DirectorySeparatorChar.ToString()))
				{
					rom.MachineName = rom.MachineName.Substring(1);
				}

				string state = "\t<software name=\"" + HttpUtility.HtmlEncode(rom.MachineName) + "\""
							+ (ExcludeOf ? "" :
									(String.IsNullOrWhiteSpace(rom.CloneOf) || (rom.MachineName.ToLowerInvariant() == rom.CloneOf.ToLowerInvariant())
										? ""
										: " cloneof=\"" + HttpUtility.HtmlEncode(rom.CloneOf) + "\"")
								)
							+ " supported=\"" + (rom.Supported == true ? "yes" : rom.Supported == false ? "no" : "partial") + "\">\n"
							+ "\t\t<description>" + HttpUtility.HtmlEncode(rom.MachineDescription) + "</description>\n"
							+ (rom.Year != null ? "\t\t<year>" + HttpUtility.HtmlEncode(rom.Year) + "</year>\n" : "")
							+ (rom.Publisher != null ? "\t\t<publisher>" + HttpUtility.HtmlEncode(rom.Publisher) + "</publisher>\n" : "");

				foreach (Tuple<string, string> kvp in rom.Infos)
				{
					state += "\t\t<info name=\"" + HttpUtility.HtmlEncode(kvp.Item1) + "\" value=\"" + HttpUtility.HtmlEncode(kvp.Item2) + "\" />\n";
				}

				sw.Write(state);
				sw.Flush();
			}
			catch (Exception ex)
			{
				Globals.Logger.Error(ex.ToString());
				return false;
			}

			return true;
		}

		/// <summary>
		/// Write out Game start using the supplied StreamWriter
		/// </summary>
		/// <param name="sw">StreamWriter to output to</param>
		/// <returns>True if the data was written, false on error</returns>
		private bool WriteEndGame(StreamWriter sw)
		{
			try
			{
				string state = "\t</software>\n\n";

				sw.Write(state);
				sw.Flush();
			}
			catch (Exception ex)
			{
				Globals.Logger.Error(ex.ToString());
				return false;
			}

			return true;
		}

		/// <summary>
		/// Write out DatItem using the supplied StreamWriter
		/// </summary>
		/// <param name="sw">StreamWriter to output to</param>
		/// <param name="rom">DatItem object to be output</param>
		/// <param name="ignoreblanks">True if blank roms should be skipped on output, false otherwise (default)</param>
		/// <returns>True if the data was written, false on error</returns>
		private bool WriteDatItem(StreamWriter sw, DatItem rom, bool ignoreblanks = false)
		{
			// If we are in ignore blanks mode AND we have a blank (0-size) rom, skip
			if (ignoreblanks
				&& (rom.Type == ItemType.Rom
				&& (((Rom)rom).Size == 0 || ((Rom)rom).Size == -1)))
			{
				return true;
			}

			try
			{
				string state = "";
				state += "\t\t<part name=\"" + rom.PartName + "\" interface=\"" + rom.PartInterface + "\">\n";

				foreach (Tuple<string, string> kvp in rom.Features)
				{
					state += "\t\t\t<feature name=\"" + HttpUtility.HtmlEncode(kvp.Item1) + "\" value=\"" + HttpUtility.HtmlEncode(kvp.Item2) + "\"/>\n";
				}

				switch (rom.Type)
				{
					case ItemType.Disk:
						state += "\t\t\t<diskarea name=\"" + (String.IsNullOrWhiteSpace(rom.AreaName) ? "cdrom" : rom.AreaName) + "\""
								+ (rom.AreaSize != null ? " size=\"" + rom.AreaSize + "\"" : "") + ">\n"
							+ "\t\t\t\t<disk name=\"" + HttpUtility.HtmlEncode(rom.Name) + "\""
							+ (!String.IsNullOrWhiteSpace(((Disk)rom).MD5) ? " md5=\"" + ((Disk)rom).MD5.ToLowerInvariant() + "\"" : "")
							+ (!String.IsNullOrWhiteSpace(((Disk)rom).SHA1) ? " sha1=\"" + ((Disk)rom).SHA1.ToLowerInvariant() + "\"" : "")
							+ (!String.IsNullOrWhiteSpace(((Disk)rom).SHA256) ? " sha256=\"" + ((Disk)rom).SHA256.ToLowerInvariant() + "\"" : "")
							+ (!String.IsNullOrWhiteSpace(((Disk)rom).SHA384) ? " sha384=\"" + ((Disk)rom).SHA384.ToLowerInvariant() + "\"" : "")
							+ (!String.IsNullOrWhiteSpace(((Disk)rom).SHA512) ? " sha512=\"" + ((Disk)rom).SHA512.ToLowerInvariant() + "\"" : "")
							+ (((Disk)rom).ItemStatus != ItemStatus.None ? " status=\"" + ((Disk)rom).ItemStatus.ToString().ToLowerInvariant() + "\"" : "")
							+ (((Disk)rom).Writable != null ? " writable=\"" + (((Disk)rom).Writable == true ? "yes" : "no") + "\"" : "")
							+ "/>\n"
							+ "\t\t\t</diskarea>\n";
						break;
					case ItemType.Rom:
						state += "\t\t\t<dataarea name=\"" + (String.IsNullOrWhiteSpace(rom.AreaName) ? "rom" : rom.AreaName) + "\""
								+ (rom.AreaSize != null ? " size=\"" + rom.AreaSize + "\"" : "") + ">\n"
							+ "\t\t\t\t<rom name=\"" + HttpUtility.HtmlEncode(rom.Name) + "\""
							+ (((Rom)rom).Size != -1 ? " size=\"" + ((Rom)rom).Size + "\"" : "")
							+ (!String.IsNullOrWhiteSpace(((Rom)rom).CRC) ? " crc=\"" + ((Rom)rom).CRC.ToLowerInvariant() + "\"" : "")
							+ (!String.IsNullOrWhiteSpace(((Rom)rom).MD5) ? " md5=\"" + ((Rom)rom).MD5.ToLowerInvariant() + "\"" : "")
							+ (!String.IsNullOrWhiteSpace(((Rom)rom).SHA1) ? " sha1=\"" + ((Rom)rom).SHA1.ToLowerInvariant() + "\"" : "")
							+ (!String.IsNullOrWhiteSpace(((Rom)rom).SHA256) ? " sha256=\"" + ((Rom)rom).SHA256.ToLowerInvariant() + "\"" : "")
							+ (!String.IsNullOrWhiteSpace(((Rom)rom).SHA384) ? " sha384=\"" + ((Rom)rom).SHA384.ToLowerInvariant() + "\"" : "")
							+ (!String.IsNullOrWhiteSpace(((Rom)rom).SHA512) ? " sha512=\"" + ((Rom)rom).SHA512.ToLowerInvariant() + "\"" : "")
							+ (!String.IsNullOrWhiteSpace(((Rom)rom).Offset) ? " offset=\"" + ((Rom)rom).Offset + "\"" : "")
							// + (!String.IsNullOrWhiteSpace(((Rom)rom).Value) ? " value=\"" + ((Rom)rom).Value + "\"" : "")
							+ (((Rom)rom).ItemStatus != ItemStatus.None ? " status=\"" + ((Rom)rom).ItemStatus.ToString().ToLowerInvariant() + "\"" : "")
							// + (!String.IsNullOrWhiteSpace(((Rom)rom).Loadflag) ? " loadflag=\"" + ((Rom)rom).Loadflag + "\"" : "")
							+ "/>\n"
							+ "\t\t\t</dataarea>\n";
						break;
				}

				state += "\t\t</part>\n";

				sw.Write(state);
				sw.Flush();
			}
			catch (Exception ex)
			{
				Globals.Logger.Error(ex.ToString());
				return false;
			}

			return true;
		}

		/// <summary>
		/// Write out DAT footer using the supplied StreamWriter
		/// </summary>
		/// <param name="sw">StreamWriter to output to</param>
		/// <returns>True if the data was written, false on error</returns>
		private bool WriteFooter(StreamWriter sw)
		{
			try
			{
				string footer = "\t</software>\n\n</softwarelist>\n";

				// Write the footer out
				sw.Write(footer);
				sw.Flush();
			}
			catch (Exception ex)
			{
				Globals.Logger.Error(ex.ToString());
				return false;
			}

			return true;
		}
	}
}
