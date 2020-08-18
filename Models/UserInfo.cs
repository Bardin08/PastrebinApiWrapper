using PastebinApiWrapper.Models.Enums;

namespace PastebinApiWrapper.Models
{
    /// <summary>
    /// User information and settings
    /// </summary>
    public class UserInfo
    {
        /// <summary>
        /// Username
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// User`s bio
        /// </summary>
        public string FormatShort { get; set; }
        /// <summary>
        /// Link to user`s avatar 
        /// </summary>
        public string AvatarUrl { get; set; }
        /// <summary>
        /// Link to user`s website
        /// </summary>
        public string Website { get; set; }
        /// <summary>
        /// User`s email
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// User`s location
        /// </summary>
        public string Location { get; set; }
        /// <summary>
        /// User`s account privity
        /// </summary>
        public AccountPrivate Private { get; set; }
        /// <summary>
        /// User`s account type
        /// </summary>
        public AccountType AccountType { get; set; }
    }
}
