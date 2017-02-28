using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NoteBook.Contracts;
using NoteBook.Models;
using SQLite;
using Xamarin.Forms;

namespace NoteBook.Services
{
    public class LocalNotesService : INotesService 
    {
        readonly SQLiteConnection _database;
        public LocalNotesService(string filename)
        {
            string databasePath = DependencyService.Get<ISQLite>().GetDatabasePath(filename);
            _database = new SQLiteConnection(databasePath);
            _database.CreateTable<NoteModel>();
        }

        public Task<IEnumerable<NoteModel>> GetAllNotes()
        {
            Task<IEnumerable<NoteModel>> task = new Task<IEnumerable<NoteModel>>(() => _database.Table<NoteModel>().Where(item => item.Delete == null));
            task.Start();
            return task;   
        }

        public Task<IEnumerable<NoteModel>> GetSyncNotes(DateTime time)
        {
            //Task<IEnumerable<NoteModel>> task = new Task<IEnumerable<NoteModel>>(() => GetAllNotes().Result.Where(item => Convert.ToDateTime(item.Create) > time || Convert.ToDateTime(item.Update) > time || Convert.ToDateTime(item.Delete) > time).ToList());
            Task<IEnumerable<NoteModel>> task = new Task<IEnumerable<NoteModel>>(() => _database.Table<NoteModel>().Where(item => item.Delete == null));
            task.Start();
            return task;
        }

        public Task<bool> CreateNote(NoteModel note)
        {
            note.NoteId = Guid.NewGuid().ToString();
            note.Create = Convert.ToString(DateTime.Now);

            Task<bool> task = new Task<bool>(() => _database.Insert(note) > 0);
            task.Start();
            return task;
        }

        public Task<bool> UpdateNote(NoteModel note)
        {
            note.Update = Convert.ToString(DateTime.Now);

            Task<bool> task = new Task<bool>(() => _database.Update(note) > 0);
            task.Start();
            return task;
        }

        public Task<bool> DeleteNote(NoteModel note)
        {
            note.Delete = Convert.ToString(DateTime.Now);

            Task<bool> task = new Task<bool>(() => _database.Update(note) > 0);
            task.Start();
            return task;
        }

    }
}