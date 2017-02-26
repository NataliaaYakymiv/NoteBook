using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NoteBook.Contracts;
using NoteBook.Models;

namespace NoteBook.Services
{
    public class RemoteNotesService : INotesService 
    {
        public IHttpAuth AccountService { get; private set; }
        public INotesService NotesService { get; private set; }

        public RemoteNotesService(IHttpAuth accountService, INotesService notesService)
        {
            AccountService = accountService;
            NotesService = notesService;
        }

        public async Task<IEnumerable<NoteModel>> GetAllNotes()
        {
            var items = new List<NoteModel>();
            HttpResponseMessage response;
            //return items;
            using (var client = AccountService.GetAuthHttpClient())
            {
                response = client.GetAsync(Settings.Url + Settings.NoteGetAllNotesPath).Result;
            }

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                try
                {
                    items = JsonConvert.DeserializeObject<List<NoteModel>>(content);
                }
                catch (Exception)
                {
                    throw new InvalidCastException("Cannot deserialize list notes");
                }
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new HttpRequestException("Not authorized");
            }

            return items;
        }

        public async Task<IEnumerable<NoteModel>> GetSyncNotes(SyncModel syncModel)
        {
            var items = new List<NoteModel>();
            HttpResponseMessage response;

            var json = JsonConvert.SerializeObject(syncModel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using (var client = AccountService.GetAuthHttpClient())
            {
                response = await client.PostAsync(Settings.Url + Settings.NoteSyncPath, content);
            }

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var result = await response.Content.ReadAsStringAsync();
                    items = JsonConvert.DeserializeObject<SyncModel>(result).NoteModels;
                }
                catch(Exception)
                {
                    throw new InvalidCastException("Cannot deserialize list notes");
                }
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new HttpRequestException("Not authorized");
            }

            return items;
        }

        public async Task<bool> CreateNote(NoteModel credentials)
        {
            HttpResponseMessage response;

            var json = JsonConvert.SerializeObject(credentials);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using (var client = AccountService.GetAuthHttpClient())
            {
                response = await client.PostAsync(Settings.Url + Settings.NoteCreatePath, content);
            }

            if (response.IsSuccessStatusCode)
            {
                var text = await response.Content.ReadAsStringAsync();
                NoteModel tempModel;

                try
                {
                    tempModel = JsonConvert.DeserializeObject<NoteModel>(text);
                }
                catch (Exception)
                {
                    throw new InvalidCastException("Cannot deserialize note");
                }

                if (!await NotesService.CreateNote(tempModel))
                    throw new InvalidOperationException("Cannot create object in DB");
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new HttpRequestException("Not authorized");
            }

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateNote(NoteModel credentials)
        {
            HttpResponseMessage response;

            var json = JsonConvert.SerializeObject(credentials);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using (var client = AccountService.GetAuthHttpClient())
            {
                response = await client.PutAsync(Settings.Url + Settings.NoteUpdatePath, content);
            }

            if (response.IsSuccessStatusCode)
            {
                var text = await response.Content.ReadAsStringAsync();
                NoteModel tempModel;

                try
                {
                    tempModel = JsonConvert.DeserializeObject<NoteModel>(text);
                }
                catch (Exception)
                {
                    throw new InvalidCastException("Cannot deserialize note");
                }

                if (!await NotesService.UpdateNote(tempModel))
                    throw new InvalidOperationException("Cannot update object in DB");
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new HttpRequestException("Not authorized");
            }

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteNote(NoteModel credentials)
        {
            HttpResponseMessage response;

            var json = JsonConvert.SerializeObject(credentials);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using (var client = AccountService.GetAuthHttpClient())
            {
                response = await client.PostAsync(Settings.Url + Settings.NoteDeletePath, content);
            }

            if (response.IsSuccessStatusCode)
            {
                if (!await NotesService.DeleteNote(credentials))
                    throw new InvalidOperationException("Cannot delete object in DB");
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new HttpRequestException("Not authorized");
            }

            return response.IsSuccessStatusCode;
        }
    }
}
