using NoteBook.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace NoteBook.Contracts
{
    public interface INotesService
    {
        Task<IEnumerable<NoteModel>> GetAllNotes();

        Task<IEnumerable<NoteModel>> GetSyncNotes(SyncModel syncModel);

        Task<bool> CreateNote(NoteModel credentials);

        Task<bool> UpdateNote(NoteModel credentials);

        Task<bool> DeleteNote(NoteModel credentials);
    }
}
