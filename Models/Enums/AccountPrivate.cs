namespace PastebinApiWrapper.Models.Enums
{
    /// <summary>
    /// User`s account privity
    /// </summary>
    public enum AccountPrivate
    {
        /// <summary>
        /// Open account
        /// </summary>
        Public = 0,
        /// <summary>
        /// Not mentioned
        /// </summary>
        Unlisted = 1,
        /// <summary>
        /// Private account
        /// </summary>
        Private = 2
    }
}
