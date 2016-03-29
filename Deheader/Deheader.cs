﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SabreTools
{
	/// <summary>
	/// Entry class for the Deheader application
	/// </summary>
	class Deheader
	{
		private static Dictionary<string, int> types;
		private static bool save;
		private static string help = @"Deheader.exe [-s] filename|dirname";

		/// <summary>
		/// Start deheader operation with supplied parameters
		/// </summary>
		/// <param name="args">String array representing command line parameters</param>
		static void Main(string[] args)
		{
			// Type mapped to header size (in decimal bytes)
			types = new Dictionary<string, int>();
			types.Add("a7800", 128);
			types.Add("fds", 16);
			types.Add("lynx", 64);
			types.Add("nes", 16);
			types.Add("snes", 512);

			if (args.Length == 0 || args.Length > 2)
			{
				Console.WriteLine(help);
				return;
			}

			// Get the filename (or foldername)
			string file = "";
			if (args.Length == 1)
			{
				file = args[0];
				save = false;
			}
			else
			{
				file = args[1];
				save = true;
			}			
			

			// If it's a single file, just check it
			if (File.Exists(file))
			{
				DetectRemoveHeader(file);
			}
			// If it's a directory, recursively check all
			else if (Directory.Exists(file))
			{
				foreach (string sub in Directory.GetFiles(file))
				{
					if (sub != ".." && sub != ".")
					{
						DetectRemoveHeader(sub);
					}
				}
			}
			// Else, show that help text
			else
			{
				Console.WriteLine(help);
			}
		}

		/// <summary>
		/// Detect and remove header from the given file
		/// </summary>
		/// <param name="file">Name of the file to be parsed</param>
		private static void DetectRemoveHeader(string file)
		{
			// Open the file in read mode
			BinaryReader br = new BinaryReader(File.OpenRead(file));

			// Extract the first 1024 bytes of the file
			byte[] hbin = br.ReadBytes(1024);
			string header = BitConverter.ToString(hbin).Replace("-", string.Empty);

			// Determine the type of the file from the header, if possible
			string type = "";
			if (Regex.IsMatch(header, "^.{2}415441524937383030") || Regex.IsMatch(header, "^.{200}41435455414C20434152542044415441205354415254532048455245"))
			{
				type = "a7800";
			}
			else if (Regex.IsMatch(header, "^4644531A0[1-4]0000000000000000000000"))
			{
				type = "fds";
			}
			else if (Regex.IsMatch(header, "^4C594E58") || Regex.IsMatch(header, "^425339"))
			{
				type = "lynx";
			}
			else if (Regex.IsMatch(header, "^4E45531A"))
			{
				type = "nes";
			}
			else if (Regex.IsMatch(header, "^.{16}0000000000000000") || Regex.IsMatch(header, "^.{16}AABB040000000000") || Regex.IsMatch(header, "^.{16}535550455255464F")) // fig, smc, ufo
			{
				type = "snes";
			}

			Console.WriteLine("File has header: " + (type != ""));

			if (type != "")
			{
				Console.WriteLine("Deteched header type: " + type);
				int hs = types[type];

				// Write out the header if we're saving it
				if (save)
				{
					Console.WriteLine("Writing header to file: " + file + ".header");
					BinaryWriter bwh = new BinaryWriter(File.OpenWrite(file + ".header"));
					for (int i = 0; i < hs; i++)
					{
						bwh.Write(hbin[i]);
					}
					bwh.Close();
				}

				// Get the bytes that aren't from the header from the extracted bit so they can be written before the rest of the file
				hbin = hbin.Skip(hs).ToArray();

				// Write out the new file
				Console.WriteLine("Creating unheadered file: " + file + ".new");
				BinaryWriter bw = new BinaryWriter(File.OpenWrite(file + ".new"));
				FileInfo fi = new FileInfo(file);
				bw.Write(hbin);
				bw.Write(br.ReadBytes((int)fi.Length - hs));
				bw.Close();
				Console.WriteLine("Unheadered file created!");
			}
			br.Close();
		}
	}
}
