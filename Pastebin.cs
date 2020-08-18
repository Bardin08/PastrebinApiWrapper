using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
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

            var result = WebAgent.Execute(baseAddress, content).Result;

            if (result.IsSuccessStatusCode)
            {
                pasteInfo.Link = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
            }

            return pasteInfo;
        }

        /// <summary>
        /// Generate user API key
        /// </summary>
        public async Task<string> CreateUserApiKey(string username, string password)
        {
            var url = new Uri("https://pastebin.com/api/api_login.php");
            var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("api_dev_key", DeveloperApiKey),
                    new KeyValuePair<string, string>("api_user_name", username),
                    new KeyValuePair<string, string>("api_user_password", password)
                });

            var result = WebAgent.Execute(url, content).Result;

            if (result.IsSuccessStatusCode)
            {
                UserApiKey = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
                return UserApiKey;
            }
            throw new HttpRequestException("Something went wrong");
        }

        /// <summary>
        /// Get list of user`s pastes info
        /// </summary>
        public async Task<List<PasteInfo>> GetUserPastes(int resultsLimit = 50)
        {
            var url = new Uri("https://pastebin.com/api/api_post.php");
            var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("api_option", "list"),
                    new KeyValuePair<string, string>("api_user_key", UserApiKey),
                    new KeyValuePair<string, string>("api_dev_key", DeveloperApiKey),
                    new KeyValuePair<string, string>("api_results_limit", resultsLimit.ToString())
                });

            var result = WebAgent.Execute(url, content).Result;

            if (result.IsSuccessStatusCode)
            {
                var xml = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
                var sb = new StringBuilder();
                sb.Append("<pastes> ").Append(xml).Append(" </pastes>");

                return ParsePastes(sb.ToString());
            }
            throw new Exception("Something went wrong");
        }

        /// <summary>
        /// Get paste code in raw version
        /// </summary>
        /// <param name="pasteInfo">Paste information. Require field <seealso cref="PasteInfo.Key"/></param>
        public async Task<PasteInfo> GetRawPaste(PasteInfo pasteInfo)
        {
            var url = new Uri("https://pastebin.com/api/api_raw.php");
            var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("api_option", "show_paste"),
                    new KeyValuePair<string, string>("api_user_key", UserApiKey),
                    new KeyValuePair<string, string>("api_dev_key", DeveloperApiKey),
                    new KeyValuePair<string, string>("api_paste_key", pasteInfo.Key)
                });

            var result = WebAgent.Execute(url, content).Result;

            if (result.IsSuccessStatusCode)
            {
                pasteInfo.Code = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
                return pasteInfo;
            }

            throw new HttpRequestException("Something went wrong");
        }

        /// <summary>
        /// Delete user`s paste
        /// </summary>
        /// <param name="pasteInfo">Paste info. Require field <seealso cref="PasteInfo.Key"/></param>
        public async Task<string> DeletePaste(PasteInfo pasteInfo)
        {
            var url = new Uri("https://pastebin.com/api/api_post.php");
            var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("api_option", "delete"),
                    new KeyValuePair<string, string>("api_user_key", UserApiKey),
                    new KeyValuePair<string, string>("api_dev_key", DeveloperApiKey),
                    new KeyValuePair<string, string>("api_paste_key", pasteInfo.Key)
                });

            var result = WebAgent.Execute(url, content).Result;

            if (result.IsSuccessStatusCode)
            {
                return await result.Content.ReadAsStringAsync().ConfigureAwait(false);
            }

            throw new HttpRequestException("Something went wrong");

        }

        /// <summary>
        /// Return user`s info and settings
        /// </summary>
        public async Task<UserInfo> GetUserInfoAndSettings()
        {
            var url = new Uri("https://pastebin.com/api/api_post.php");

            var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("api_option", "userdetails"),
                    new KeyValuePair<string, string>("api_user_key", UserApiKey),
                    new KeyValuePair<string, string>("api_dev_key", DeveloperApiKey),
                });

            var result = WebAgent.Execute(url, content).Result;

            if (result.IsSuccessStatusCode)
            {
                var xml = await result.Content.ReadAsStringAsync().ConfigureAwait(false);

                return ParseUserInfo(xml);
            }

            throw new Exception("Something went wrong");

        }


        #endregion

        #region Private Methods
        /// <summary>
        /// Parse user`s pastes
        /// </summary>
        /// <param name="xml">Paste bin responce</param>
        private static List<PasteInfo> ParsePastes(string xml)
        {
            using (var sr = new StringReader(xml))
            {
                var formatter = new XmlSerializer(typeof(pastes));

                var parsedPastes = formatter.Deserialize(sr) as pastes;

                return ConvertParsedPastesToPasteInfo(parsedPastes);
            }
        }

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

        /// <summary>
        /// Convert <seealso cref="pastes"/> to <seealso cref="PasteInfo"/>
        /// </summary>
        /// <param name="parsedPastes"></param>
        /// <returns>List of PasteInfos</returns>
        private static List<PasteInfo> ConvertParsedPastesToPasteInfo(pastes parsedPastes)
        {
            var userPastes = new List<PasteInfo>();

            parsedPastes.paste.ToList().ForEach(p =>
            {
                userPastes.Add(new PasteInfo
                {
                    Key = p.paste_key,
                    Name = p.paste_title,
                    Link = p.paste_url,
                    Views = p.paste_hits,
                    Size = p.paste_size,
                    Publication = new DateTime(p.paste_date),
                    Remove = new DateTime(p.paste_expire_date),
                    Language = (PasteLanguage)Enum.Parse(typeof(PasteLanguage), p.paste_format_short),
                    Private = (PastePrivate)Enum.ToObject(typeof(PastePrivate), p.paste_private)
                });
            });

            return userPastes;
        }

        /// <summary>
        /// Parse user`s info
        /// </summary>
        /// <param name="xml">Pastebin response</param>
        /// <returns>User info</returns>
        private static UserInfo ParseUserInfo(string xml)
        {
            using (var sr = new StringReader(xml))
            {
                var formatter = new XmlSerializer(typeof(user));
                var parsedUserInfo = formatter.Deserialize(sr) as user;

                return new UserInfo
                {
                    Name = parsedUserInfo.user_name,
                    FormatShort = parsedUserInfo.user_format_short,
                    AvatarUrl = parsedUserInfo.user_avatar_url,
                    Website = parsedUserInfo.user_website,
                    Email = parsedUserInfo.user_email,
                    Location = parsedUserInfo.user_location,
                    Private = (AccountPrivate)Enum.ToObject(typeof(AccountPrivate), parsedUserInfo.user_private),
                    AccountType = (AccountType)Enum.ToObject(typeof(AccountType), parsedUserInfo.user_account_type)
                };
            }
        }

        #endregion

        #endregion
    }
}
