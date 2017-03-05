using System.Linq;
using NoteBook.Contracts;

namespace NoteBook.Helpers
{
    public class NotesHelper
    {
        public static async void ClearLocal(INotesService notesService)
        {
            var items = notesService.GetAllNotes().Result.ToList();
            foreach (var item in items)
            {
                await notesService.DeleteNote(item);
            }
        }
    }
}