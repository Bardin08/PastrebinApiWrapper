using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;

using PastebinApiWrapper.Models;
using PastebinApiWrapper.Models.Enums;
using PastebinApiWrapper.Models.SerializeModels;

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
        /// Create new paste as guest
        /// </summary>
        /// <param name="pasteInfo">Paste information. Required fields: <seealso cref="PasteInfo.Private"/>, <see cref="PasteInfo.Name"/>,
        /// <seealso cref="PasteInfo.ExpireDate"/>, <seealso cref="PasteInfo.Language"/>, <seealso cref="PasteInfo.Code"/></param>
        /// <returns>Paste information <seealso cref="PasteInfo"/></returns>
        public async Task<PasteInfo> CreateNewPasteAsGuestAsync(PasteInfo pasteInfo)
        {
            return await CreateNewPasteAsUserAsync(pasteInfo, "").ConfigureAwait(false);
        }
        /// <summary>
        /// Create new paste as user
        /// </summary>
        /// <param name="pasteInfo">Paste information. Required fields: <seealso cref="PasteInfo.Private"/>, <see cref="PasteInfo.Name"/>,
        /// <seealso cref="PasteInfo.ExpireDate"/>, <seealso cref="PasteInfo.Language"/>, <seealso cref="PasteInfo.Code"/></param>
        /// <returns>Paste information <seealso cref="PasteInfo"/></returns>
        public async Task<PasteInfo> CreateNewPasteAsUserAsync(PasteInfo pasteInfo, string userApiKey)
        {
            var baseAddress = new Uri("https://pastebin.com/api/api_post.php");
            using (var client = new HttpClient())
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("api_option", "paste"),
                    new KeyValuePair<string, string>("api_user_key", userApiKey),
                    new KeyValuePair<string, string>("api_paste_private", ((int)pasteInfo.Private).ToString()),
                    new KeyValuePair<string, string>("api_paste_name", pasteInfo.Name),
                    new KeyValuePair<string, string>("api_paste_expire_date", ConvertPasteExpireDateToString(pasteInfo.ExpireDate)),
                    new KeyValuePair<string, string>("api_paste_format", Enum.GetName(typeof(PasteLanguage), pasteInfo.Language)),
                    new KeyValuePair<string, string>("api_dev_key", DeveloperApiKey),
                    new KeyValuePair<string, string>("api_paste_code", pasteInfo.Code)
                });

                var result = await client.PostAsync(baseAddress, content).ConfigureAwait(false);

                if (result.IsSuccessStatusCode)
                {
                    pasteInfo.Link = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
            }

            return pasteInfo;
        }
        /// <summary>
        /// Generate user API key
        /// </summary>
        public async Task<string> CreateUserApiKey(string username, string password)
        {
            var baseAddress = new Uri("https://pastebin.com/api/api_login.php");
            using (var client = new HttpClient() { BaseAddress = baseAddress })
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("api_dev_key", DeveloperApiKey),
                    new KeyValuePair<string, string>("api_user_name", username),
                    new KeyValuePair<string, string>("api_user_password", password)
                });

                var result = await client.PostAsync(baseAddress, content).ConfigureAwait(false);

                if (result.IsSuccessStatusCode)
                {
                    UserApiKey = await result.Content.ReadAsStringAsync();
                    return UserApiKey;
                }
            }
            throw new HttpRequestException("Something went wrong");
        }
        /// <summary>
        /// Get list of user`s pastes
        /// </summary>
        /// <param name="resultLimit">The number of pastes that will be returned</param>
        public async Task<List<PasteInfo>> GetUserPastes(int resultsLimit = 50)
        {
            var baseUrl = new Uri("https://pastebin.com/api/api_post.php");

            using (var client = new HttpClient())
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("api_option", "list"),
                    new KeyValuePair<string, string>("api_user_key", UserApiKey),
                    new KeyValuePair<string, string>("api_dev_key", DeveloperApiKey),
                    new KeyValuePair<string, string>("api_results_limit", resultsLimit.ToString())
                });

                var result = await client.PostAsync(baseUrl, content).ConfigureAwait(false);

                if (result.IsSuccessStatusCode)
                {
                    var xml = await result.Content.ReadAsStringAsync().ConfigureAwait(false);

                    using (var sr = new StringReader(xml))
                    {
                        var formatter = new XmlSerializer(typeof(List<XMLResponse>));

                        var returnedPastes = formatter.Deserialize(sr) as List<XMLResponse>;
                        var userPastes = new List<PasteInfo>();

                        returnedPastes.ForEach(p =>
                        {
                            userPastes.Add(new PasteInfo
                            {
                                Key = p.paste_key,
                                Name = p.paste_title,
                                Link = p.paste_url,
                                Views = (int)p.paste_hits,
                                Size = p.paste_size,
                                Publication = new DateTime((long)p.paste_date),
                                Remove = new DateTime((long)p.paste_expire_date),
                                Language = (PasteLanguage)Enum.Parse(typeof(PasteLanguage), p.paste_format_short),
                                Private = (PastePrivate)Enum.ToObject(typeof(PastePrivate), p.paste_private)
                            });
                        });

                        return userPastes;
                    }
                }
            }
            throw new Exception("Something went wrong");
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
