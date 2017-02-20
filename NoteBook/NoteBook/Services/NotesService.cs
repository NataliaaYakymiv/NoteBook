using NoteBook.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoteBook.Models;
using System.Net.Http;
using Newtonsoft.Json;
using System.Diagnostics;

namespace NoteBook.Servises
{
    class NotesService : INotesService
    {
        public string Url { get; } = "http://192.168.1.127:81/";

        public string NoteGetPath { get; } = "api/Notes/Get";
        public string NoteCreatePath { get; } = "api/Notes/Create";
        public string NoteEditPath { get; } = "api/Notes/Edit";
        public string NoteRefreshPath { get; } = "api/Notes/Refresh";
        public string NoteDeletePath { get; } = "api/Notes/Delete";

        private NotesService() { }
        private static NotesService Instance { set; get; }
        public List<NoteModel> Items { get; private set; }


        public static NotesService GetService()
        {
            return Instance ?? (Instance = new NotesService());
        }

        public Task<HttpResponseMessage> GetNotes()
        {
            throw new NotImplementedException();
        }



        public async Task<List<NoteModel>> RefreshDataAsync()
        {
            Items = new List<NoteModel>();
            using (var client = AccountService.GetService().GetAuthHttpClient())
            {
                try
                {
                    var response = await client.GetAsync(Url + NoteRefreshPath);
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        Items = JsonConvert.DeserializeObject<List<NoteModel>>(content);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(@"				ERROR {0}", ex.Message);
                }
            }
            return Items;
        }


        public Task SaveTodoItemAsync(NoteModel item, bool isNewItem)
        {
            throw new NotImplementedException();
        }

        public Task DeleteTodoItemAsync(string id)
        {
            throw new NotImplementedException();
        }

        public HttpResponseMessage Create(NoteModel credentials)
        {
            using (var client = AccountService.GetService().GetAuthHttpClient())
            {
                var json = JsonConvert.SerializeObject(credentials);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var result = client.PostAsync(Url + NoteCreatePath, content).Result;

                return result;
            }
        }

        public HttpResponseMessage Edit(NoteModel credentials)
        {
            using (var client = AccountService.GetService().GetAuthHttpClient())
            {
                var json = JsonConvert.SerializeObject(credentials);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var result = client.PutAsync(Url + NoteEditPath, content).Result;

                return result;
            }

        }

        public HttpResponseMessage Delete(NoteModel credentials)
        {
            using (var client = AccountService.GetService().GetAuthHttpClient())
            {
                var json = JsonConvert.SerializeObject(credentials);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var result = client.PostAsync(Url + NoteDeletePath, content).Result;

                return result;
            }

        }
    }
}
