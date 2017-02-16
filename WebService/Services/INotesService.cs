using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebService.Models;

namespace WebService.Services
{
    public interface INotesService
    {
        bool DoesItemExist(string id);
        NoteModel Find(string id);
        IEnumerable<NoteModel> GetData();
        void InsertData(NoteModel item);
        void UpdateData(NoteModel item);
        void DeleteData(string id);
    }
}