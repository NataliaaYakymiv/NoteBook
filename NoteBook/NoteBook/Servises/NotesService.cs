using NoteBook.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoteBook.Models;
using System.Net.Http;
using Newtonsoft.Json;

namespace NoteBook.Servises
{
    class NotesService : INotesService
    {
        public string Url { get; } = "http://192.168.0.102:81/";
        public string AuthKey { get; private set; }

        private NotesService() { }
        private static NotesService Instance { set; get; }

        public static NotesService GetService()
        {
            return Instance ?? (Instance = new NotesService());
        }

        public Task<HttpResponseMessage> GetNotes()
        {
            throw new NotImplementedException();
        }

        public HttpResponseMessage Create(NoteModel credentials)
        {
            using (var client = new HttpClient())
            {
                var json = JsonConvert.SerializeObject(credentials);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var result =  client.PostAsync("http://192.168.0.102:81/api/Notes/Create", content).Result;

                return result;
            }
        }
        public HttpResponseMessage Edit(NoteModel credentials)
        {
            using (var client = new HttpClient())
            {
                var json = JsonConvert.SerializeObject(credentials);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var result = client.PutAsync("http://192.168.0.102:81/api/Notes/Edit", content).Result;

                return result;
            }

        }
        //public HttpResponseMessage Delete(NoteModel credentials)
        //{
        //    using (var client = new HttpClient())
        //    {
        //        var json = JsonConvert.SerializeObject(credentials);
        //        var content = new StringContent(json, Encoding.UTF8, "application/json");

        //        var result = client.DeleteAsync("http://192.168.0.102:81/api/Notes/Delete").Result;

        //        return result;
        //    }

        //}
    }
}
