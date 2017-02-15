using System.Collections.Generic;
using WebService.Models;

namespace WebService.Services
{
    public interface INotesRepository
    {
        bool DoesItemExist(string id);
        IEnumerable<NotesItem> All { get; }
        NotesItem Find(string id);
        void Insert(NotesItem item);
        void Update(NotesItem item);
        void Delete(string id);
    }
}