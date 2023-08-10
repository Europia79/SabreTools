namespace SabreTools.Models.ClrMamePro
{
    /// <remarks>media</remarks>
    public class Media
    {
        /// <remarks>name</remarks>
        [Required]
        public string? Name { get; set; }

        /// <remarks>md5</remarks>
        public string? MD5 { get; set; }

        /// <remarks>sha1</remarks>
        public string? SHA1 { get; set; }

        /// <remarks>sha256</remarks>
        public string? SHA256 { get; set; }

        /// <remarks>spamsum</remarks>
        public string? SpamSum { get; set; }

        #region DO NOT USE IN PRODUCTION

        /// <remarks>Should be empty</remarks>
        public string[]? ADDITIONAL_ELEMENTS { get; set; }

        #endregion
    }
}