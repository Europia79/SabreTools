namespace SabreTools.Models.Hashfile
{
    /// <summary>
    /// SHA-1 File
    /// </summary>
    public class SHA1
    {
        [Required]
        public string? Hash { get; set; }

        [Required]
        public string? File { get; set; }
    }
}