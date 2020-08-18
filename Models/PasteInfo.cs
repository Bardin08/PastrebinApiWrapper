using System;

using PastebinApiWrapper.Models.Enums;

namespace PastebinApiWrapper.Models
{
    /// <summary>
    /// Paste information
    /// </summary>
    public class PasteInfo
    {
        /// <summary>
        /// Paste unique key
        /// </summary>
        public string Key { get; set; }
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
        /// Paste views
        /// </summary>
        public int Views { get; set; }
        /// <summary>
        /// Paste size in kB
        /// </summary>
        public int Size { get; set; }
        /// <summary>
        /// Paste publication date
        /// </summary>
        public DateTime Publication { get; set; }
        /// <summary>
        /// Paste removal date
        /// </summary>
        public DateTime Remove { get; set; }
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
