using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using WebService.Models;
using WebService.Contracts;

namespace WebService.Repositories
{
    public class NotesRepository : INotesRepository
    {
        private NotesContext _db; 

        public NotesRepository()
        {
            InitializeData();
        }

        public bool DoesItemExist(int idUser, int id)
        {
            return _db.NoteModels.Any(item => item.NoteId == id && item.UserId == idUser);
        }

        public IEnumerable<NoteModel> All(int iduser)
        {
            return _db.NoteModels.Where(item => item.UserId == iduser).ToList();
        }

        public NoteModel Find(int idUser, int idNote)
        {
            return _db.NoteModels.FirstOrDefault(item => item.NoteId == idNote && item.UserId == idUser);
        }

        public void Insert(int idUser, NoteModel item)
        {
            item.UserId = idUser;
            _db.NoteModels.Add(item);
            _db.SaveChanges();
        }

        public void Update(int idUser, NoteModel note)
        {
            var result = _db.NoteModels.SingleOrDefault(item => item.NoteId == note.NoteId && item.UserId == idUser);
            if (result != null)
            {
                result.NoteName = note.NoteName;
                result.NoteText = note.NoteText;

                _db.Entry(result).State = EntityState.Modified;

                _db.SaveChanges();
            }
        }

        public void Delete(int idUser, int idNote)
        {
            _db.NoteModels.Remove(Find(idUser, idNote));
            _db.SaveChanges();
        }

        #region Helpers

        private void InitializeData()
        {
            _db = new NotesContext();
        }
        #endregion
    }
}