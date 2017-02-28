using System;
using System.Collections.Generic;
using System.Linq;
using NoteBook.Contracts;
using NoteBook.Models;

namespace NoteBook.Services
{
    public class NotesItemManager
    {
        public INotesService RemoteNoteService { get; private set; }
        public INotesService LocalNoteService { get; private set; }

        public NotesItemManager(INotesService remoteNoteService, INotesService localNotesService)
        {
            RemoteNoteService = remoteNoteService;
            LocalNoteService = localNotesService;
        }

        public void SetService(INotesService remoteNoteService, INotesService localNotesService)
        {
            RemoteNoteService = remoteNoteService;
            LocalNoteService = localNotesService;
        }

        public async void ClearLocal()
        {
            var items = App.Database as NoteService;
            var temp = items.Get().Result;
            foreach (var item in temp)
            {
                await App.Database.DeleteNote(item);
            }
        }

        //public async void Sync()
        //{
        //    SyncModel syncModel = new SyncModel();
        //    syncModel.LastModify = Convert.ToDateTime(UserSettings.SyncDate);
        //    var notes1 = App.Database.GetAllNotes().Result.ToList();
        //    var notes = (await LocalNoteService.GetSyncNotes(syncModel)).ToList() ?? new List<NoteModel>();


        //    syncModel.NoteModels = notes;


        //    var items = (await RemoteNoteService.GetSyncNotes(syncModel)).ToList();

        //    //var notes3 = App.Database.GetAllNotes().ToList();
        //    foreach (NoteModel t in notes)
        //    {
        //        await LocalNoteService.DeleteNote(t);
        //    }
        //    foreach (var item in items)
        //    {
        //        try
        //        {
        //            await LocalNoteService.CreateNote(item);
        //        }
        //        catch (Exception)
        //        {
        //            await LocalNoteService.UpdateNote(item);
        //        }
        //    }

        //    //for (int i = 0; i < items.Count; i++)
        //    //{
        //    //    var i1 = i;
        //    //    if (notes.Find(x => items[i1].NoteId == x.NoteId) != null)
        //    //    {
        //    //        await LocalNoteService.UpdateNote(items[i]);
        //    //        notes.Remove(notes.Find(x => items[i1].NoteId == x.NoteId));
        //    //    }
        //    //    else
        //    //    {
        //    //        await LocalNoteService.CreateNote(items[i]);
        //    //    }
        //    //}
        //    //var notes2 = App.Database.GetAllNotes().ToList();
            
        //    UserSettings.SyncDate = DateTime.Now.ToString();
        //    //var notes4 = App.Database.GetAllNotes().ToList();
        //    //time = DateTime.Now;
        //}
    }
}
