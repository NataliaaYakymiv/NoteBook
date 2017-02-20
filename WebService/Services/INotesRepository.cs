using System.Collections.Generic;
using WebService.Models;

namespace WebService.Services
{
    public interface INotesRepository
    {
        bool DoesItemExist(string id);
        IEnumerable<NoteModel> All();
        NoteModel Find(string id);
        void Insert(NoteModel item);
        void Update(NoteModel item);
        void Delete(string id);
    }
}
