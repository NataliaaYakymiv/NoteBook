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


        public IEnumerable<NoteModel> GetAllNotes()
        {
            return _database.Table<NoteModel>().ToList();
        }

        public IEnumerable<NoteModel> GetSyncNotes(SyncModel syncModel)
        {
            return GetAllNotes().Where(item => Convert.ToDateTime(item.Create) > syncModel.LastModify || Convert.ToDateTime(item.Update) > syncModel.LastModify || Convert.ToDateTime(item.Delete) > syncModel.LastModify).ToList();
        }

        public bool CreateNote(NoteModel note)
        {
            return _database.Insert(note) > 0;
        }

        public bool UpdateNote(NoteModel note)
        {
            return _database.Update(note) > 0;
        }

        public bool DeleteNote(NoteModel note)
        {
            return _database.Delete<NoteModel>(note.NoteId) > 0;
        }
    }
}
