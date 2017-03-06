using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NoteBook.Contracts;
using NoteBook.Helpers;
using NoteBook.Models;
using SQLite;
using Xamarin.Forms;

namespace NoteBook.Services
{
    public class LocalNotesService : INotesService 
    {
        private readonly SQLiteConnection _database;

        public LocalNotesService(string filename)
        {
            var databasePath = DependencyService.Get<ISQLite>().GetDatabasePath(filename);
            _database = new SQLiteConnection(databasePath);
            _database.CreateTable<NoteModel>();
        }

        public Task<IEnumerable<NoteModel>> GetAllNotes()
        {
            var task = new Task<IEnumerable<NoteModel>>(() => _database.Table<NoteModel>().Where(item => item.Delete == null));
            task.Start();
            return task;   
        }

        public Task<IEnumerable<NoteModel>> GetSyncNotes()
        {
            var task = new Task<IEnumerable<NoteModel>>(() => _database.Table<NoteModel>().Where(item => item.Delete == null));
            task.Start();
            return task;
        }

        public Task<bool> CreateNote(NoteModel note)
        {
            note.NoteId = Guid.NewGuid().ToString();
            note.Create = DateTime.Now.ToString("G");
            note.IsLocal = true;
            if (note.MediaFile != null)
            {
                note.ImageInBytes = StreamHelper.ReadFully(note.MediaFile.Source);
                note.Image = note.MediaFile.Path;
            }
            var task = new Task<bool>(() => _database.Insert(note) > 0);
            task.Start();
            return task;
        }

        public Task<bool> UpdateNote(NoteModel note)
        {
            note.Update = DateTime.Now.ToString("G");
            note.IsLocal = true;
            if (note.MediaFile != null)
            {
                note.ImageInBytes = StreamHelper.ReadFully(note.MediaFile.Source);
                note.Image = note.MediaFile.Path;
            }

            var task = new Task<bool>(() => _database.Update(note) > 0);
            task.Start();
            return task;
        }

        public Task<bool> DeleteNote(NoteModel note)
        {
            note.Delete = DateTime.Now.ToString("G");
            note.IsLocal = true;
            var task = new Task<bool>(() => _database.Update(note) > 0);
            task.Start();
            return task;
        }

    }
}