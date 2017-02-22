using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NoteBook.Contracts;
using NoteBook.Models;

namespace NoteBook.Services
{
    class NotesService : INotesService
    {
        public string Url { get; } = Constants.URL;

        public string NoteGetAllNotesPath { get; } = "api/Notes/GetAllNotes";
        public string NoteCreatePath { get; } = "api/Notes/CreateNote";
        public string NoteUpdatePath { get; } = "api/Notes/UpdateNote";
        public string NoteDeletePath { get; } = "api/Notes/DeleteNote";

        private NotesService() { }
        private static NotesService Instance { set; get; }
        public List<NoteModel> Items { get; private set; }


        public static NotesService GetService()
        {
            return Instance ?? (Instance = new NotesService());
        }
        
        public async Task<List<NoteModel>> GetAllNotes()
        {
            Items = new List<NoteModel>();
            using (var client = AccountService.GetService().GetAuthHttpClient())
            {
                try
                {
                    var response = await client.GetAsync(Url + NoteGetAllNotesPath);
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

        public HttpResponseMessage CreateNote(NoteModel credentials)
        {
            using (var client = AccountService.GetService().GetAuthHttpClient())
            {
                var json = JsonConvert.SerializeObject(credentials);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var result = client.PostAsync(Url + NoteCreatePath, content).Result;

                return result;
            }
        }

        public HttpResponseMessage UpdateNote(NoteModel credentials)
        {
            using (var client = AccountService.GetService().GetAuthHttpClient())
            {
                var json = JsonConvert.SerializeObject(credentials);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var result = client.PutAsync(Url + NoteUpdatePath, content).Result;

                return result;
            }

        }

        public HttpResponseMessage DeleteNote(NoteModel credentials)
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
