using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NoteBook.Contracts;
using NoteBook.Models;

namespace NoteBook.Services
{
    public class AccountService : IAccountService
    {
        public string Url { get; } = Constants.URL;

        public string RegisterPath { get; } = "api/Account/register";
        public string LoginPath { get; } = "api/Account/login";
        public string ExternalLoginPath { get; } = "api/Account/externallogin";
        public string ExternalLoginCallbackPath { get; } = "api/Account/externallogincallback";
        public string ExternalLoginConfirmationPath { get; } = "api/Account/externalloginconfirmation";
        public string ExternalLoginFailurePath { get; } = "api/Account/externalloginfailure";
        public string ExternalLoginFinalPath { get; } = "api/account/ExternalLoginFinal";

        public string AuthKey { get; private set; }

        private AccountService() { }
        private static AccountService Instance { set; get; }

        public static AccountService GetService()
        {
            return Instance ?? (Instance = new AccountService());
        }

        public async Task<HttpResponseMessage> Login(AccountModels.LoginModel credentials)
        {
            using (var client = new HttpClient())
            {
                var json = JsonConvert.SerializeObject(credentials);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var result = await client.PostAsync(Url + LoginPath, content).ConfigureAwait(false);

                AuthKey = result.Headers.GetValues("Set-Cookie").First(x => x.StartsWith(".ASPXAUTH"));

                return result;
            }
        }

        public async Task<HttpResponseMessage> ExternalLogin(string url)
        {
            using (var client = new HttpClient())
            {
                var result = await client.GetAsync(url).ConfigureAwait(false);

                AuthKey = result.Headers.GetValues("Set-Cookie").First(x => x.StartsWith(".ASPXAUTH"));

                return result;
            }
        }

        public async Task<HttpResponseMessage> Register(AccountModels.RegisterModel credentials)
        {
            using (var client = new HttpClient())
            {
                var json = JsonConvert.SerializeObject(credentials);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var result = await client.PostAsync(Url + RegisterPath, content).ConfigureAwait(false);

                return result;
            }
        }

        public HttpClient GetAuthHttpClient()
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.CookieContainer = new CookieContainer();
            if (AuthKey != null || AuthKey != string.Empty)
            {
                var name = AuthKey.Split('=')[0];
                var value = AuthKey.Split('=')[1].Split(';')[0];
                handler.CookieContainer.Add(new Uri(Url), new Cookie(name, value));
            }
            return new HttpClient(handler);
        }
    }
}