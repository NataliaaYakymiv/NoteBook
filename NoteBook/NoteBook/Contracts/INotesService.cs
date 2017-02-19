using NoteBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NoteBook.Contracts
{
    interface INotesService
    {
        Task<HttpResponseMessage> GetNotes();
    }
}
