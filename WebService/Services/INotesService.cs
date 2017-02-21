using System.Collections.Generic;
using WebService.Models;

namespace WebService.Services
{
    public interface INotesService
    {
        bool DoesItemExist(int id);
        NoteModel Find(int id);
        IEnumerable<NoteModel> GetData();
        void InsertData(NoteModel item);
        void UpdateData(NoteModel item);
        void DeleteData(int id);
    }
}