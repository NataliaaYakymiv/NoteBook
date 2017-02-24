using System;
using System.Collections.Generic;
using System.Linq;
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

        public IEnumerable<NoteModel> GetAllNotes()
        {
            return _database.Table<NoteModel>().Where(item => item.Delete == null).ToList();   
        }

        public IEnumerable<NoteModel> GetSyncNotes(SyncModel syncModel)
        {
            return GetAllNotes().Where(item => Convert.ToDateTime(item.Create) > syncModel.LastModify || Convert.ToDateTime(item.Update) > syncModel.LastModify || Convert.ToDateTime(item.Delete) > syncModel.LastModify).ToList();
        }

        public bool CreateNote(NoteModel note)
        {
            note.NoteId = Guid.NewGuid().ToString();
            note.Create = Convert.ToString(DateTime.Now);
            return _database.Insert(note) > 0;
        }

        public bool UpdateNote(NoteModel note)
        {
            note.Update = Convert.ToString(DateTime.Now);
            return _database.Update(note) > 0;
        }

        public bool DeleteNote(NoteModel note)
        {
            note.Delete = Convert.ToString(DateTime.Now);
            return _database.Update(note) > 0;
        }

    }
}