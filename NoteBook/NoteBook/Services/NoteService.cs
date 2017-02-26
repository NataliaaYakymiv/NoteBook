using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoteBook.Contracts;
using NoteBook.Models;
using SQLite;
using Xamarin.Forms;

namespace NoteBook.Services
{
    public class NoteService : INotesService 
    {
        readonly SQLiteConnection _database;

        public NoteService(string filename)
        {
            string databasePath = DependencyService.Get<ISQLite>().GetDatabasePath(filename);
            _database = new SQLiteConnection(databasePath);
            _database.CreateTable<NoteModel>();
        }


        public Task<IEnumerable<NoteModel>> GetAllNotes()
        {
            Task<IEnumerable<NoteModel>> task = new Task<IEnumerable<NoteModel>>(() => _database.Table<NoteModel>());
            task.Start();
            return task;
        }

        public Task<IEnumerable<NoteModel>> GetSyncNotes(SyncModel syncModel)
        {
            Task<IEnumerable<NoteModel>> task = new Task<IEnumerable<NoteModel>>(() => GetAllNotes().Result.Where(item => Convert.ToDateTime(item.Create) > syncModel.LastModify || Convert.ToDateTime(item.Update) > syncModel.LastModify || Convert.ToDateTime(item.Delete) > syncModel.LastModify));
            task.Start();
            return task;
        }

        public Task<bool> CreateNote(NoteModel note)
        {
            Task<bool> task = new Task<bool>(() => _database.Insert(note) > 0);
            task.Start();
            return task;
        }

        public Task<bool> UpdateNote(NoteModel note)
        {
            Task<bool> task = new Task<bool>(() => _database.Update(note) > 0);
            task.Start();
            return task;
        }

        public Task<bool> DeleteNote(NoteModel note)
        {
            Task<bool> task = new Task<bool>(() => _database.Delete<NoteModel>(note.NoteId) > 0);
            task.Start();
            return task;
        }
    }
}
