using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NoteBook.Contracts;
using NoteBook.Models;

namespace NoteBook.Services
{
    public class AccountService : IAccountService, IHttpAuth
    {

        public async Task<bool> Login(AccountModels.LoginModel credentials)
        {
            HttpResponseMessage result;

            var json = JsonConvert.SerializeObject(credentials);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {
                result = await client.PostAsync(Settings.Url + Settings.LoginPath, content).ConfigureAwait(false);
            }

            if (result.IsSuccessStatusCode)
            {
                UserSettings.UserName = credentials.UserName;
                var aspxauth = result.Headers.GetValues("Set-Cookie").First(x => x.StartsWith(".ASPXAUTH"));
                SetAuthKey(aspxauth);
            }

            return result.IsSuccessStatusCode;
        }

        public async Task<bool> ExternalLogin(string url)
        {
            HttpResponseMessage result;

            using (var client = new HttpClient())
            {
                result = await client.GetAsync(url).ConfigureAwait(false);
            }

            if (result.IsSuccessStatusCode)
            {
                UserSettings.UserName = result.Content.ReadAsStringAsync().Result;
                var aspxauth  = result.Headers.GetValues("Set-Cookie").First(x => x.StartsWith(".ASPXAUTH"));
                SetAuthKey(aspxauth);
            }

            return result.IsSuccessStatusCode;
        }

        public async Task<bool> Register(AccountModels.RegisterModel credentials)
        {
            HttpResponseMessage result;

            var json = JsonConvert.SerializeObject(credentials);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {
                result = await client.PostAsync(Settings.Url + Settings.RegisterPath, content).ConfigureAwait(false);
            }

            return result.IsSuccessStatusCode;
        }

        public async Task Logout()
        {
            using (var client = GetAuthHttpClient())
            {
                 await client.GetAsync(Settings.Url + Settings.LogoutPath).ConfigureAwait(false);
            }
            UserSettings.AuthValue = string.Empty;
            UserSettings.Expiress = string.Empty;
            UserSettings.SyncDate = string.Empty;
            UserSettings.UserName = string.Empty;
        }

        public HttpClient GetAuthHttpClient()
        {
            var handler = new HttpClientHandler {CookieContainer = new CookieContainer()};

            if (!string.IsNullOrEmpty(UserSettings.AuthKey) && !string.IsNullOrEmpty(UserSettings.AuthValue))
            {
                handler.CookieContainer.Add(new Uri(Settings.Url), new Cookie(UserSettings.AuthKey, UserSettings.AuthValue));
            }

            return new HttpClient(handler);
        }

        private static void SetAuthKey(string aspxauth)
        {
            if (string.IsNullOrEmpty(aspxauth)) return;

            UserSettings.AuthKey = aspxauth.Split('=')[0];
            UserSettings.AuthValue = aspxauth.Split('=')[1].Split(';')[0];
            string time = Regex.Match(aspxauth, @"(?<=expires=)(.*)(?= GMT;)").ToString();
            if (!string.IsNullOrEmpty(time))
            {
                var myDate = DateTime.ParseExact(time, "ddd, dd-MMM-yyyy HH:mm:ss",
                    System.Globalization.CultureInfo.InvariantCulture).AddHours(2);
                UserSettings.Expiress = myDate.ToString();
            }
            else
            {
                var tempTime = DateTime.Now.AddDays(7);
                UserSettings.Expiress = tempTime.ToString();
            }
        }

        public bool IsLoged()
        {
            if (!string.IsNullOrEmpty(UserSettings.Expiress))
            {
                return DateTime.Now < Convert.ToDateTime(UserSettings.Expiress);
            }
            return false;
        }
    }
}