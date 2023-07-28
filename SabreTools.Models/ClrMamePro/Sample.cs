namespace SabreTools.Models.ClrMamePro
{
    /// <remarks>sample</remarks>
    public class Sample
    {
        /// <remarks>name</remarks>
        public string Name { get; set; }

        #region DO NOT USE IN PRODUCTION

        /// <remarks>Should be empty</remarks>
        public string[]? ADDITIONAL_ELEMENTS { get; set; }

        #endregion
    }
}