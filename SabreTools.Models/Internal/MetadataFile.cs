namespace SabreTools.Models.Internal
{
    /// <summary>
    /// Format-agnostic representation of a full metadata file
    /// </summary>
    public class MetadataFile : DictionaryBase
    {
        #region Keys

        /// <remarks>Machine[]</remarks>
        [NoFilter]
        public const string MachineKey = "machine";

        /// <remarks>Header</remarks>
        [NoFilter]
        public const string HeaderKey = "header";

        #endregion
    }
}