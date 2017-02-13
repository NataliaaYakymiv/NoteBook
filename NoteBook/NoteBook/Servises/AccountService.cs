using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NoteBook.Contracts;
using NoteBook.Models;

namespace NoteBook.Servises
{
    public class AccountService : IAccountService
    {
        public string Url { get; } = "http://localhost:55023";

        private AccountService() { }
        private static AccountService Instance { set; get; }

        public static AccountService GetService()
        {
            if (Instance != null)
                return Instance;
            return new AccountService();
        }

        public async Task<string> Login(FormCredentials credentials)
        {
            throw  new NotImplementedException();
        }

        public async Task<string> Register(FormCredentials credentials)
        {
            using (var client = new HttpClient())
            {
                var json = JsonConvert.SerializeObject(credentials);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var result = await client.PostAsync("http://192.168.1.149:81/api/Account/register", content);

                return await result.Content.ReadAsStringAsync();
            }
        }
    }
}