using System.Collections.Generic;
using WebService.Models;

namespace WebService.Services
{
    public interface INotesRepository
    {
        bool DoesItemExist(int id);
        IEnumerable<NoteModel> All();
        NoteModel Find(int id);
        void Insert(NoteModel item);
        void Update(NoteModel item);
        void Delete(int id);
    }
}
