using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using PastebinApiWrapper.Models;
using PastebinApiWrapper.Models.Enums;

namespace PastebinApiWrapper
{
    public class Pastebin
    {
        #region Properties
        /// <summary>
        /// Api key requires to work with API
        /// </summary>
        public string DeveloperApiKey { get; set; }
        /// <summary>
        /// User API key requires to work with user account
        /// </summary>
        public string UserApiKey { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// <seealso cref="Pastebin"/> constructor
        /// </summary>
        /// <param name="developerApiKey">Developer API key</param>
        public Pastebin(string developerApiKey)
        {
            if (string.IsNullOrEmpty(developerApiKey) || string.IsNullOrWhiteSpace(developerApiKey))
            {
                throw new ArgumentException(nameof(developerApiKey), "Developer key can`t be null, empty or whitespace");
            }

            DeveloperApiKey = developerApiKey;
        }
        #endregion

        #region Methods

        #region Public Methods
        /// <summary>
        /// Create new paste
        /// </summary>
        /// <param name="pasteInfo">Paste information</param>
        /// <returns>Paste information <seealso cref="PasteInfo"/></returns>
        public async Task<PasteInfo> CreateNewPasteAsync(PasteInfo pasteInfo)
        {
            var baseAddress = new Uri("https://pastebin.com/api/api_post.php");
            var cookieContainer = new CookieContainer();
            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("api_option", "paste"),
                    new KeyValuePair<string, string>("api_user_key", UserApiKey),
                    new KeyValuePair<string, string>("api_paste_private", ((int)pasteInfo.Private).ToString()),
                    new KeyValuePair<string, string>("api_paste_name", pasteInfo.Name),
                    new KeyValuePair<string, string>("api_paste_expire_date", ConvertPasteExpireDateToString(pasteInfo.ExpireDate)),
                    new KeyValuePair<string, string>("api_paste_format", Enum.GetName(typeof(PasteLanguage), pasteInfo.Language)),
                    new KeyValuePair<string, string>("api_dev_key", DeveloperApiKey),
                    new KeyValuePair<string, string>("api_paste_code", pasteInfo.Code)
                });

                cookieContainer.Add(baseAddress, new Cookie("CookieName", "cookie_value"));
                var result = await client.PostAsync(baseAddress, content).ConfigureAwait(false);

                if (result.IsSuccessStatusCode)
                {
                    pasteInfo.Link = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
            }

            return pasteInfo;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Convert <seealso cref="PastebinApiWrapper.Enums.PasteExpireDate"/> to string
        /// </summary>
        private string ConvertPasteExpireDateToString(PasteExpireDate pasteExpireDate)
        {
            if (pasteExpireDate == PasteExpireDate.Never)
            {
                return "N";
            }
            else if (pasteExpireDate == PasteExpireDate.OneDay)
            {
                return "1D";
            }
            else if (pasteExpireDate == PasteExpireDate.OneHour)
            {
                return "1H";
            }
            else if (pasteExpireDate == PasteExpireDate.OneMonth)
            {
                return "1M";
            }
            else if (pasteExpireDate == PasteExpireDate.OneWeek)
            {
                return "1W";
            }
            else if (pasteExpireDate == PasteExpireDate.OneYear)
            {
                return "1Y";
            }
            else if (pasteExpireDate == PasteExpireDate.SixMonths)
            {
                return "6M";
            }
            else if (pasteExpireDate == PasteExpireDate.TenMinutes)
            {
                return "10M";
            }
            else if (pasteExpireDate == PasteExpireDate.TwoWeeks)
            {
                return "2W";
            }
            throw new ArgumentException("Incorrect value", nameof(pasteExpireDate));
        }
        #endregion

        #endregion
    }
}
