using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NoteBook.Contracts;
using NoteBook.Helpers;
using NoteBook.Models;

namespace NoteBook.Services
{
    public class RemoteNotesService : INotesService 
    {
        public INotesService NotesService { get; private set; }

        public RemoteNotesService(INotesService notesService)
        {
            NotesService = notesService;
        }

        public async Task<IEnumerable<NoteModel>> GetAllNotes()
        {
            var items = new List<NoteModel>();
            HttpResponseMessage response;

            using (var client = AuthHelper.GetAuthHttpClient())
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

        public async Task<IEnumerable<NoteModel>> GetSyncNotes()
        {
            var syncModel = new SyncModel {LastModify = UserSettings.SyncDate };
            var notes = NotesService.GetSyncNotes().Result.ToList() ?? new List<NoteModel>();

            syncModel.NoteModels = notes;

            HttpResponseMessage response;

            var json = JsonConvert.SerializeObject(syncModel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            using (var client = AuthHelper.GetAuthHttpClient())
            {
                response = client.PostAsync(Settings.Url + Settings.NoteSyncPath, content).Result;
            }

            if (response.IsSuccessStatusCode)
            {
                for (int i = 0; i < notes.Count; i++)
                {
                    if (notes[i].ImageInBytes != null && notes[i].ImageInBytes.Length > 0)
                    {
                        var i1 = i;
                        await Task.Factory.StartNew(() => UploadBytes(notes[i1])).ConfigureAwait(false);
                        notes[i].ImageInBytes = null;
                    }
                }
                try
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    var syncModelParse = JsonConvert.DeserializeObject<SyncModel>(result);
                    UserSettings.SyncDate = syncModelParse.LastModify;

                    var items = syncModelParse.NoteModels;

                    foreach (var note in notes)
                    {
                        if (note.Delete != null)
                        {
                            await Task.Factory.StartNew(() =>  NotesService.DeleteNote(note));
                        }
                    }

                    var localStorage = NotesService.GetAllNotes().Result.ToList();

                    foreach (var item in items)
                    {
                        if (localStorage.Find(x => x.NoteId == item.NoteId) == null)
                        {
                            if (item.Delete == null)
                            {
                                item.IsLocal = false;
                                await Task.Factory.StartNew(() => NotesService.CreateNote(item));
                            }
                        }
                        else
                        {
                            if (item.Delete != null)
                            {
                                await Task.Factory.StartNew(() => NotesService.DeleteNote(item));
                            }
                            else
                            {
                                item.IsLocal = false;
                                await Task.Factory.StartNew(() => NotesService.UpdateNote(item));
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    throw new InvalidCastException("Cannot deserialize list notes");
                }
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new HttpRequestException("Not authorized");
            }

            return NotesService.GetAllNotes().Result;
        }

        public async Task<bool> CreateNote(NoteModel credentials)
        {
            credentials.NoteId = Guid.NewGuid().ToString();
            HttpResponseMessage response;

            var json = JsonConvert.SerializeObject(credentials);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using (var client = AuthHelper.GetAuthHttpClient())
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

                if (credentials.MediaFile != null)
                {
                    tempModel.MediaFile = credentials.MediaFile;
                    await Task.Factory.StartNew(() => Upload(tempModel));
                }
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

            using (var client = AuthHelper.GetAuthHttpClient())
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

                if (credentials.MediaFile != null)
                {
                    tempModel.MediaFile = credentials.MediaFile;
                    await Task.Factory.StartNew(() => Upload(tempModel));
                }
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

            using (var client = AuthHelper.GetAuthHttpClient())
            {
                response = await client.PostAsync(Settings.Url + Settings.NoteDeletePath, content);
            }

            if (response.IsSuccessStatusCode)
            {
                await Task.Factory.StartNew(() => NotesService.DeleteNote(credentials));
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new HttpRequestException("Not authorized");
            }

            return response.IsSuccessStatusCode;
        }

        #region UploadingImage

        private async Task Upload(NoteModel model)
        {
            byte[] data = StreamHelper.ReadFully(model.MediaFile.Source);

            var imageStream = new ByteArrayContent(data);

            var content = new MultipartFormDataContent {imageStream};

            using (var client = AuthHelper.GetAuthHttpClient())
            {
                await client.PostAsync(Settings.Url + Settings.NoteAddImagePath + "?noteId=" + model.NoteId, content);
            }
        }


        private async Task UploadBytes(NoteModel model)
        {
            var imageStream = new ByteArrayContent(model.ImageInBytes);

            var content = new MultipartFormDataContent {imageStream};

            using (var client = AuthHelper.GetAuthHttpClient())
            {
                await client.PostAsync(Settings.Url + Settings.NoteAddImagePath + "?noteId=" + model.NoteId, content);
            }
        }

        #endregion
    }
}
