﻿using System.Collections.Generic;
using System.IO;

using SabreTools.DatFiles;
using SabreTools.Help;
using SabreTools.IO;

namespace RombaSharp.Features
{
    internal class Diffdat : BaseFeature
    {
        public const string Value = "Diffdat";

        public Diffdat()
        {
            Name = Value;
            Flags = new List<string>() { "diffdat" };
            Description = "Creates a DAT file with those entries that are in -new DAT.";
            _featureType = ParameterType.Flag;
            LongDescription = @"Creates a DAT file with those entries that are in -new DAT file and not
in -old DAT file. Ignores those entries in -old that are not in -new.";
            this.Features = new Dictionary<string, Feature>();

            AddFeature(OutStringInput);
            AddFeature(OldStringInput);
            AddFeature(NewStringInput);
            AddFeature(NameStringInput);
            AddFeature(DescriptionStringInput);
        }

        public override void ProcessFeatures(Dictionary<string, Feature> features)
        {
            base.ProcessFeatures(features);

            // Get feature flags
            string name = GetString(features, NameStringValue);
            string description = GetString(features, DescriptionStringValue);
            string newdat = GetString(features, NewStringValue);
            string olddat = GetString(features, OldStringValue);
            string outdat = GetString(features, OutStringValue);

            // Ensure the output directory
            DirectoryExtensions.Ensure(outdat, create: true);

            // Check that all required files exist
            if (!File.Exists(olddat))
            {
                logger.Error($"File '{olddat}' does not exist!");
                return;
            }

            if (!File.Exists(newdat))
            {
                logger.Error($"File '{newdat}' does not exist!");
                return;
            }

            // Get the DatTool for opeations
            DatTool dt = new DatTool();

            // Create the encapsulating datfile
            DatFile datfile = DatFile.Create();
            datfile.Header.Name = name;
            datfile.Header.Description = description;
            dt.ParseInto(datfile, olddat);

            // Diff against the new datfile
            DatFile intDat = dt.CreateAndParse(newdat);
            datfile.DiffAgainst(intDat, false);
            dt.Write(intDat, outdat);
        }
    }
}
