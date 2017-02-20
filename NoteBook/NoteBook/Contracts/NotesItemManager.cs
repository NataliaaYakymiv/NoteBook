using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NoteBook.Contracts;
using NoteBook.Models;
using NoteBook.Servises;

namespace NoteBook.Contracts
{
    public class NotesItemManager
    {
        INotesService restService;

        public NotesItemManager(INotesService service)
        {
           restService = service;
        }

        public Task<List<NoteModel>> GetTasksAsync()
        {
            return restService.RefreshDataAsync();
        }

        public Task SaveTaskAsync(NoteModel item, bool isNewItem = false)
        {
            return restService.SaveTodoItemAsync(item, isNewItem);
        }

        public Task DeleteTaskAsync(NoteModel item)
        {
            return restService.DeleteTodoItemAsync(item.NoteId.ToString());
        }
    }
}
