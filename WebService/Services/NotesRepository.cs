using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebService.Models;
using WebService.Services;

namespace WebService.Services
{
    public class NotesRepository : INotesRepository
    {
        private NotesContext db;

        public NotesRepository()
        {
            InitializeData();
        }

        public IEnumerable<NoteModel> All()
        {
            return db.NoteModels.ToList();
        }

        public bool DoesItemExist(int id)
        {
            return db.NoteModels.Any(item => item.NoteId == id);
        }

        public NoteModel Find(int id)
        {
            return db.NoteModels.Where(item => item.NoteId == id).FirstOrDefault();
        }

        public void Insert(NoteModel item)
        {
            db.NoteModels.Add(item);
            db.SaveChanges();
        }

        public void Update(NoteModel item)
        {
            var todoItem = this.Find(item.NoteId);
            var i = item;
            //var index = db.NoteModels.ToList().IndexOf(todoItem);
            //db.NoteModels.ToList().RemoveAt(index);
            //db.NoteModels.ToList().Insert(index, item);
            db.NoteModels.Remove(this.Find(item.NoteId));
            db.NoteModels.Add(i);
            //db.NoteModels.Add(todoItem);
            db.SaveChanges();
        }

        public void Delete(int id)
        {
            db.NoteModels.Remove(this.Find(id));
            db.SaveChanges();
        }

        #region Helpers

        private void InitializeData()
        {
            db = new NotesContext();
            //using (NotesContext db = new NotesContext())
            //{
            //    //_notesList = db.NoteModels;

            //   // _notesList = notes.ToList();

            //}


            //    var todoItem1 = new NoteModel()
            //    {
            //        NoteId = "6bb8a868-dba1-4f1a-93b7-24ebce87e243",
            //        NoteName = "Learn app development",
            //        NoteText = "Attend Xamarin University",
            //        //Done = true
            //    };

            //    var todoItem2 = new NoteModel()
            //    {
            //        NoteId = "b94afb54-a1cb-4313-8af3-b7511551b33b",
            //        NoteName = "Develop apps",
            //        NoteText = "Use Xamarin Studio/Visual Studio",
            //        //Done = false
            //    };

            //    var todoItem3 = new NoteModel()
            //    {
            //        NoteId = "ecfa6f80-3671-4911-aabe-63cc442c1ecf",
            //        NoteName = "Publish apps",
            //        NoteText = "All app stores",
            //        //Done = false,
            //    };

            //    _notesList.Add(todoItem1);
            //    _notesList.Add(todoItem2);
            //    _notesList.Add(todoItem3);
            //}
        }
        #endregion
    }
}