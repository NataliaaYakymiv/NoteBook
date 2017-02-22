using System.Collections.Generic;
using System.Threading.Tasks;
using NoteBook.Models;

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
            return restService.GetAllNotes();
        }


    }
}
