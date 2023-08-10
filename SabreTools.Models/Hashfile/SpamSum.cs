namespace SabreTools.Models.Hashfile
{
    /// <summary>
    /// SpamSum File
    /// </summary>
    public class SpamSum
    {
        [Required]
        public string? Hash { get; set; }

        [Required]
        public string? File { get; set; }
    }
}