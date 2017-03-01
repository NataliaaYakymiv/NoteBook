using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Java.IO;
using Newtonsoft.Json;
using NoteBook.Contracts;
using NoteBook.Models;
using XLabs.Platform.Services.Media;

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

        public Task<IEnumerable<NoteModel>> GetSyncNotes(DateTime time)
        {
            var syncModel = new SyncModel {LastModify = time };
            var notes = NotesService.GetSyncNotes(time).Result.ToList() ?? new List<NoteModel>();


            syncModel.NoteModels = notes;

            HttpResponseMessage response;

            var json = JsonConvert.SerializeObject(syncModel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using (var client = AccountService.GetAuthHttpClient())
            {
                response = client.PostAsync(Settings.Url + Settings.NoteSyncPath, content).Result;
            }

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    var items = JsonConvert.DeserializeObject<SyncModel>(result).NoteModels;

                    UserSettings.SyncDate = JsonConvert.DeserializeObject<SyncModel>(result).LastModify.ToString();
                    foreach (var t in notes)
                    {
                        if (t.Delete != null)
                        {
                            NotesService.DeleteNote(t);
                        }
                    }
                    foreach (var item in items)
                    {
                        try
                        {
                            NotesService.CreateNote(item);
                        }
                        catch (Exception)
                        {
                            NotesService.UpdateNote(item);
                        }
                    }


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
           
            return NotesService.GetAllNotes();
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
                if (!NotesService.CreateNote(tempModel).Result)
                    throw new InvalidOperationException("Cannot create object in DB");
                UserSettings.SyncDate = Convert.ToDateTime(tempModel.Create).AddSeconds(1).ToString();
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new HttpRequestException("Not authorized");
            }


            return response.IsSuccessStatusCode;
        }

        public async Task PostItem(byte[] item)
        {
            using (var client = AccountService.GetAuthHttpClient())
            {
                var da = new ByteArrayContent(item);

                try
                {
                    var multi = new MultipartContent();
                    multi.Add(da);
                    var response = await client.PostAsync(Settings.Url + Settings.NoteAddImagePath, multi);
                }
                catch (Exception ex)
                {
                   // Console.Write(ex.Message);
                }
            }
        }

        private void upload(MediaFile mediaFile)
        {
            try
            {

                byte[] data = ReadFully(mediaFile.Source);
                var imageStream = new ByteArrayContent(data);


                var multi = new MultipartContent();
                multi.Add(imageStream);
                var client = new System.Net.Http.HttpClient();
                client.BaseAddress = new Uri("http://xyz.com/");
                var result = client.PostAsync("api/Default", multi).Result;
            }
            catch (Exception e)
            {

            }

        }
        //This for converting media file stream to byte[]
        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
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

                if (!NotesService.UpdateNote(tempModel).Result)
                    await NotesService.CreateNote(tempModel);
                UserSettings.SyncDate = Convert.ToDateTime(tempModel.Update).AddSeconds(1).ToString();
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
                await NotesService.DeleteNote(credentials);
                UserSettings.SyncDate = DateTime.Now.ToString();
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new HttpRequestException("Not authorized");
            }

            return response.IsSuccessStatusCode;
        }
    }
}
