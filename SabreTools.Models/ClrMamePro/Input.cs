namespace SabreTools.Models.ClrMamePro
{
    /// <remarks>input</remarks>
    public class Input
    {
        /// <remarks>players, Numeric/remarks>
        [Required]
        public string? Players { get; set; }

        /// <remarks>control</remarks>
        public string? Control { get; set; }

        /// <remarks>buttons, Numeric</remarks>
        [Required]
        public string? Buttons { get; set; }

        /// <remarks>coins, Numeric</remarks>
        public string? Coins { get; set; }

        /// <remarks>tilt, (yes|no) "no"</remarks>
        public string? Tilt { get; set; }

        /// <remarks>service, (yes|no) "no"</remarks>
        public string? Service { get; set; }

        #region DO NOT USE IN PRODUCTION

        /// <remarks>Should be empty</remarks>
        public object[]? ADDITIONAL_ELEMENTS { get; set; }

        #endregion
    }
}