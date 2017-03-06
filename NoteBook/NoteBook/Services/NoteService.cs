using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NoteBook.Contracts;
using NoteBook.Models;
using SQLite;
using Xamarin.Forms;

namespace NoteBook.Services
{
    public class NoteService : INotesService 
    {
        private readonly SQLiteConnection _database;

        public NoteService(string filename)
        {
            var databasePath = DependencyService.Get<ISQLite>().GetDatabasePath(filename);
            _database = new SQLiteConnection(databasePath);
            _database.CreateTable<NoteModel>();
        }


        public Task<IEnumerable<NoteModel>> GetAllNotes()
        {
            var task = new Task<IEnumerable<NoteModel>>(() => _database.Table<NoteModel>());
            task.Start();
            return task;
        }

        public Task<IEnumerable<NoteModel>> GetSyncNotes()
        {
            var task = new Task<IEnumerable<NoteModel>>(() => _database.Table<NoteModel>().AsEnumerable()
                    .Where(x => x.IsLocal));
            task.Start();
            return task;
        }

        public Task<bool> CreateNote(NoteModel note)
        {
            var task = new Task<bool>(() => _database.Insert(note) > 0);
            task.Start();
            return task;
        }

        public Task<bool> UpdateNote(NoteModel note)
        {
            var task = new Task<bool>(() => _database.Update(note) > 0);
            task.Start();
            return task;
        }

        public Task<bool> DeleteNote(NoteModel note)
        {
            var task = new Task<bool>(() => _database.Delete<NoteModel>(note.NoteId) > 0);
            task.Start();
            return task;
        }
    }
}
