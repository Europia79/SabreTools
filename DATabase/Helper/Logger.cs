﻿using System;
using System.IO;

namespace SabreTools.Helper
{
	/// <summary>
	/// Log either to file or to the console
	/// </summary>
	public class Logger
	{
		// Private instance variables
		private bool _tofile;
		private string _filename;
		private StreamWriter _log;

		// Public wrappers
		public bool ToFile
		{
			get { return _tofile; }
			set
			{
				if (!value)
				{
					Close();
				}
				_tofile = value;
				if (_tofile)
				{
					Start();
				}
			}
		}

		/// <summary>
		/// Initialize a Logger object with the given information
		/// </summary>
		/// <param name="tofile">True if file should be written to instead of console</param>
		/// <param name="filename">Optional filename representing log location</param>
		public Logger(bool tofile, string filename = "")
		{
			_tofile = tofile;
			_filename = filename;
		}

		/// <summary>
		/// Start logging by opening output file (if necessary)
		/// </summary>
		/// <returns>True if the logging was started correctly, false otherwise</returns>
		public bool Start()
		{
			if (!_tofile)
			{
				return true;
			}

			try
			{
				_log = new StreamWriter(File.Open(_filename, FileMode.OpenOrCreate | FileMode.Append));
				_log.WriteLine("Logging started " + DateTime.Now);
			}
			catch
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// End logging by closing output file (if necessary)
		/// </summary>
		/// <returns>True if the logging was ended correctly, false otherwise</returns>
		public bool Close()
		{
			if (!_tofile)
			{
				return true;
			}

			try
            {
				_log.WriteLine("Logging ended " + DateTime.Now);
				_log.Close();
			}
			catch
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Write the given string to the log output
		/// </summary>
		/// <param name="output">String to be written log</param>
		/// <returns>True if the output could be written, false otherwise</returns>
		public bool Log(string output)
		{
			// If we're writing to console, just write the string
			if (!_tofile)
			{
				Console.WriteLine(output);
				return true;
			}
			// If we're writing to file, use the existing stream
			try
			{
				_log.WriteLine(output);
			}
			catch
			{
				Console.WriteLine("Could not write to log file!");
				return false;
			}

			return true;
		}
	}
}
