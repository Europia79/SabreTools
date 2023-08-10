namespace SabreTools.Models.EverdriveSMDB
{
    /// <summary>
    /// SHA-256 \t Machine Name/Filename \t SHA-1 \t MD5 \t CRC32 \t Size (Optional)
    /// </summary>
    public class Row
    {
        [Required]
        public string? SHA256 { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public string? SHA1 { get; set; }

        [Required]
        public string? MD5 { get; set; }

        [Required]
        public string? CRC32 { get; set; }

        public string? Size { get; set; }

        #region DO NOT USE IN PRODUCTION

        /// <remarks>Should be empty</remarks>
        public string[]? ADDITIONAL_ELEMENTS { get; set; }

        #endregion
    }
}