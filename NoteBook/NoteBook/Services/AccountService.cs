using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NoteBook.Contracts;
using NoteBook.Helpers;
using NoteBook.Models;

namespace NoteBook.Services
{
    public class AccountService : IAccountService
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
                AuthHelper.SetAuthKey(aspxauth);
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
                AuthHelper.SetAuthKey(aspxauth);
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
            using (var client = AuthHelper.GetAuthHttpClient())
            {
                 await client.GetAsync(Settings.Url + Settings.LogoutPath).ConfigureAwait(false);
            }
            AuthHelper.ClearAll();
            NotesHelper.ClearLocal(new NoteService(Settings.DatabaseName));
        }

    }
}