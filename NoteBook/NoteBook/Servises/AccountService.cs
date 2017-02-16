using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NoteBook.Contracts;
using NoteBook.Models;

namespace NoteBook.Servises
{
    public class AccountService : IAccountService
    {
        //Yura
        //public string Url { get; } = "http://192.168.1.149:81/";
        //Natalia
        public string Url { get; } = "http://192.168.0.102:81/";
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
                
                var result = await client.PostAsync("http://192.168.0.102:81/api/Account/login", content).ConfigureAwait(false);
                
                AuthKey = result.Headers.GetValues(HttpRequestHeader.Authorization.ToString()).First(x => x.StartsWith("Basic"));

                return result;
            }
        }

        public async Task<HttpResponseMessage> Register(AccountModels.RegisterModel credentials)
        {
            using (var client = new HttpClient())
            {
                var json = JsonConvert.SerializeObject(credentials);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var result = await client.PostAsync("http://192.168.0.102:81/api/Account/register", content).ConfigureAwait(false);

                return result;
            }
        }

        public async Task<HttpResponseMessage> GetInt()
        {
            using (var client = new HttpClient())
            {

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add(HttpRequestHeader.Authorization.ToString(), AuthKey);
                var result = await client.GetAsync("http://192.168.0.102:81/api/Account/GetNumber").ConfigureAwait(false);

                return result;
            }
        }

        
    }
}