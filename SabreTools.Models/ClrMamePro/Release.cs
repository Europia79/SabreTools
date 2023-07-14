namespace SabreTools.Models.ClrMamePro
{
    /// <remarks>release</remarks>
    public class Release : ItemBase
    {
        /// <remarks>name</remarks>
        public string Name { get; set; }

        /// <remarks>region</remarks>
        public string Region { get; set; }

        /// <remarks>language</remarks>
        public string? Language { get; set; }

        /// <remarks>date</remarks>
        public string? Date { get; set; }

        /// <remarks>default</remarks>
        public string? Default { get; set; }

        #region DO NOT USE IN PRODUCTION

        /// <remarks>Should be empty</remarks>
        public string[]? ADDITIONAL_ELEMENTS { get; set; }

        #endregion
    }
}