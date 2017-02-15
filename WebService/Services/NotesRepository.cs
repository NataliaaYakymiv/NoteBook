using System.Collections.Generic;
using System.Linq;
using WebService.Models;

namespace WebService.Services
{
    public class NotesRepository : INotesRepository
    {
        private List<NotesItem> _notesList;

        public NotesRepository()
        {
            InitializeData();
        }

        public IEnumerable<NotesItem> All
        {
            get { return _notesList; }
        }

        public bool DoesItemExist(string id)
        {
            return _notesList.Any(item => item.Id == id);
        }

        public NotesItem Find(string id)
        {
            return _notesList.Where(item => item.Id == id).FirstOrDefault();
        }

        public void Insert(NotesItem item)
        {
            _notesList.Add(item);
        }

        public void Update(NotesItem item)
        {
            var todoItem = this.Find(item.Id);
            var index = _notesList.IndexOf(todoItem);
            _notesList.RemoveAt(index);
            _notesList.Insert(index, item);
        }

        public void Delete(string id)
        {
            _notesList.Remove(this.Find(id));
        }

        #region Helpers

        private void InitializeData()
        {
            _notesList = new List<NotesItem>();

            var todoItem1 = new NotesItem()
            {
                Id = "6bb8a868-dba1-4f1a-93b7-24ebce87e243",
                Name = "Learn app development",
                Text = "Attend Xamarin University",
                //Done = true
            };

            var todoItem2 = new NotesItem()
            {
                Id = "b94afb54-a1cb-4313-8af3-b7511551b33b",
                Name = "Develop apps",
                Text = "Use Xamarin Studio/Visual Studio",
                //Done = false
            };

            var todoItem3 = new NotesItem()
            {
                Id = "ecfa6f80-3671-4911-aabe-63cc442c1ecf",
                Name = "Publish apps",
                Text = "All app stores",
                //Done = false,
            };

            _notesList.Add(todoItem1);
            _notesList.Add(todoItem2);
            _notesList.Add(todoItem3);
        }

        #endregion
    }
}