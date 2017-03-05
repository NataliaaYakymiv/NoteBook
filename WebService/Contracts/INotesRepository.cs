using System;
using System.Collections.Generic;
using WebService.Models;

namespace WebService.Contracts
{
    public interface INotesRepository
    {
        bool DoesItemExist(int idUser, string idNote);
        IEnumerable<NoteModel> All(int iduser);
        IEnumerable<NoteModel> HasChanges(int idUser, SyncModel model);
        NoteModel Find(int idUser, string idNote);
        void Insert(int idUser, NoteModel note);
        void Update(int idUser, NoteModel note);
        void Delete(int idUser, string idNote);
        void SetImage(string noteId, string path);
    }
}
