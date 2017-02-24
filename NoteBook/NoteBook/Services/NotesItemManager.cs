using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NoteBook.Models;
using NoteBook.Services;

namespace NoteBook.Contracts
{
    public class NotesItemManager
    {
        public INotesService NoteService { get; private set; }


        public DateTime time;

        public NotesItemManager(INotesService noteService)
        {
            NoteService = noteService;
        }

        public void SetService(INotesService noteService)
        {
            NoteService = noteService;
        }

        public void Sync()
        {
            SyncModel syncModel = new SyncModel();
            var notes1 = App.Database.GetAllNotes().ToList();
            var notes = App.Database.GetAllNotes().ToList();
            
            
            syncModel.LastModify = time;
            syncModel.NoteModels = notes;


            var items = NoteService.GetSyncNotes(syncModel).ToList();

            var notes3 = App.Database.GetAllNotes().ToList();

            for (int i = 0; i < items.Count; i++)
            {
                var i1 = i;
                if (notes.Find(x => items[i1].NoteId == x.NoteId) != null)
                {
                    App.Database.UpdateNote(items[i]);
                    notes.Remove(notes.Find(x => items[i1].NoteId == x.NoteId));
                }
                else
                {
                    App.Database.CreateNote(items[i]);
                }
            }
            var notes2 = App.Database.GetAllNotes().ToList();
            foreach (NoteModel t in notes)
            {
                App.Database.DeleteNote(t);
            }
            var notes4 = App.Database.GetAllNotes().ToList();
            time = DateTime.Now;
        }
    }
}
