using PastebinApiWrapper.Models.Enums;

namespace PastebinApiWrapper.Models
{
    /// <summary>
    /// Paste information
    /// </summary>
    public class PasteInfo
    {
        /// <summary>
        /// Paste name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Paste code
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Paste link
        /// </summary>
        public string Link { get; set; }
        /// <summary>
        /// Paste expire date
        /// </summary>
        public PasteExpireDate ExpireDate { get; set; }
        /// <summary>
        /// Paste language
        /// </summary>
        public PasteLanguage Language { get; set; }
        /// <summary>
        /// Paste private
        /// </summary>
        public PastePrivate Private { get; set; }
    }
}
