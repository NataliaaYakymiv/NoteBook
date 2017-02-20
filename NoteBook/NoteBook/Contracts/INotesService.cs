using NoteBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
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
