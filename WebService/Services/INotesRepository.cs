using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebService.Models;

namespace WebService.Services
{
    public interface INotesRepository
    {
        bool DoesItemExist(string id);
        IEnumerable<NoteModel> All { get; }
        NoteModel Find(string id);
        void Insert(NoteModel item);
        void Update(NoteModel item);
        void Delete(string id);
    }
}
