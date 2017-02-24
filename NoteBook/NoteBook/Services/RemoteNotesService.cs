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
    class RemoteNotesService : INotesService 
    {
        public string Url { get; } = Constants.URL;

        public string NoteGetAllNotesPath { get; } = "api/Notes/GetAllNotes";
        public string NoteCreatePath { get; } = "api/Notes/CreateNote";
        public string NoteUpdatePath { get; } = "api/Notes/UpdateNote";
        public string NoteDeletePath { get; } = "api/Notes/DeleteNote";
        public string NoteSyncPath { get; } = "api/Notes/GetSyncNotes";

        //private NotesService() { }
        //private static NotesService Instance { set; get; }
        //public List<NoteModel> Items { get; private set; }


        //public static NotesService GetService()
        //{
        //    return Instance ?? (Instance = new NotesService());
        //}

        public IEnumerable<NoteModel> GetAllNotes()
        {
            var items = new List<NoteModel>();
            using (var client = AccountService.GetService().GetAuthHttpClient())
            {
                try
                {
                    var response = client.GetAsync(Url + NoteGetAllNotesPath).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var content = response.Content.ReadAsStringAsync().Result;
                        items = JsonConvert.DeserializeObject<List<NoteModel>>(content);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(@"				ERROR {0}", ex.Message);
                }
            }
            return items;
        }

        public IEnumerable<NoteModel> GetSyncNotes(SyncModel syncModel)
        {
            var items = new List<NoteModel>();
            using (var client = AccountService.GetService().GetAuthHttpClient())
            {
                try
                {
                    var json = JsonConvert.SerializeObject(syncModel);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = client.PostAsync(Url + NoteSyncPath, content).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var result = response.Content.ReadAsStringAsync().Result;
                        items = JsonConvert.DeserializeObject<SyncModel>(result).NoteModels;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(@"				ERROR {0}", ex.Message);
                }
            }
            return items;
        }

        public bool CreateNote(NoteModel credentials)
        {
            HttpResponseMessage response;

            using (var client = AccountService.GetService().GetAuthHttpClient())
            {
                var json = JsonConvert.SerializeObject(credentials);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                response = client.PostAsync(Url + NoteCreatePath, content).Result;
            }

            var note = JsonConvert.DeserializeObject<NoteModel>(response.Content.ReadAsStringAsync().Result);

            App.Database.CreateNote(note);
            return response.IsSuccessStatusCode;
        }

        public bool UpdateNote(NoteModel credentials)
        {
            HttpResponseMessage response;

            using (var client = AccountService.GetService().GetAuthHttpClient())
            {
                var json = JsonConvert.SerializeObject(credentials);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                response = client.PutAsync(Url + NoteUpdatePath, content).Result;
            }
            var note = JsonConvert.DeserializeObject<NoteModel>(response.Content.ReadAsStringAsync().Result);

            App.Database.UpdateNote(note);
            return response.IsSuccessStatusCode;
        }

        public bool DeleteNote(NoteModel credentials)
        {
            HttpResponseMessage response;

            using (var client = AccountService.GetService().GetAuthHttpClient())
            {
                var json = JsonConvert.SerializeObject(credentials);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                response =  client.PostAsync(Url + NoteDeletePath, content).Result;
            }

            App.Database.DeleteNote(credentials);
            return response.IsSuccessStatusCode;
        }
    }
}
