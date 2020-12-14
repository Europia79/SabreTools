using System;

namespace SabreTools.DatItems
{
    /// <summary>
    /// Determines which type of duplicate a file is
    /// </summary>
    [Flags]
    public enum DupeType
    {
        // Type of match
        Hash = 1 << 0,
        All = 1 << 1,

        // Location of match
        Internal = 1 << 2,
        External = 1 << 3,
    }

    /// <summary>
    /// A subset of fields that can be used as keys
    /// </summary>
    public enum ItemKey
    {
        NULL = 0,

        Machine,

        CRC,
        MD5,
        SHA1,
        SHA256,
        SHA384,
        SHA512,
        SpamSum,
    }
}