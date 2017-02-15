using System.Collections.Generic;
using WebService.Models;

namespace WebService.Services
{
    public interface INotesService
    {
        bool DoesItemExist(string id);
        NotesItem Find(string id);
        IEnumerable<NotesItem> GetData();
        void InsertData(NotesItem item);
        void UpdateData(NotesItem item);
        void DeleteData(string id);
    }
}