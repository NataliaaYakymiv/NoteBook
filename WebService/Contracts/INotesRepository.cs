using System.Collections.Generic;
using WebService.Models;

namespace WebService.Contracts
{
    public interface INotesRepository
    {
        bool DoesItemExist(int idUser, int id);
        IEnumerable<NoteModel> All(int iduser);
        NoteModel Find(int idUser, int idNote);
        void Insert(int idUser, NoteModel note);
        void Update(int idUser, NoteModel note);
        void Delete(int idUser, int idNote);
    }
}
