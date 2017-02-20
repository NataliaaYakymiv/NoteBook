using NoteBook.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace NoteBook.Contracts
{
    public interface INotesService
    {
        Task<List<NoteModel>> RefreshDataAsync();

        Task SaveTodoItemAsync(NoteModel item, bool isNewItem);

        Task DeleteTodoItemAsync(string id);

        Task<HttpResponseMessage> GetNotes();
    }
}
