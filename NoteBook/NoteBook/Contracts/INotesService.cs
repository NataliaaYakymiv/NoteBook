using NoteBook.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace NoteBook.Contracts
{
    public interface INotesService
    {
        Task<List<NoteModel>> GetAllNotes();

        HttpResponseMessage CreateNote(NoteModel credentials);

        HttpResponseMessage UpdateNote(NoteModel credentials);

        HttpResponseMessage DeleteNote(NoteModel credentials);
    }
}
