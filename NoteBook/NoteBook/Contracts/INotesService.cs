using NoteBook.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace NoteBook.Contracts
{
    public interface INotesService
    {
        IEnumerable<NoteModel> GetAllNotes();

        IEnumerable<NoteModel> GetSyncNotes(SyncModel syncModel);

        bool CreateNote(NoteModel credentials);

        bool UpdateNote(NoteModel credentials);

        bool DeleteNote(NoteModel credentials);
    }
}
