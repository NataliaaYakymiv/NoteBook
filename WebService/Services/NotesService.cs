using System;
using System.Collections.Generic;
using WebService.Models;

namespace WebService.Services
{
    public class NotesService : INotesService
    {
        private readonly INotesRepository _repository;

        public NotesService(INotesRepository repository)
        {
            if (repository == null)
            {
                throw new ArgumentNullException("repository");
            }

            _repository = repository;
        }

        public bool DoesItemExist(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException(id);
            }

            return _repository.DoesItemExist(id);
        }

        public NotesItem Find(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException("id");
            }

            return _repository.Find(id);
        }

        public IEnumerable<NotesItem> GetData()
        {
            return _repository.All;
        }

        public void InsertData(NotesItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            _repository.Insert(item);
        }

        public void UpdateData(NotesItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            _repository.Update(item);
        }

        public void DeleteData(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException("id");
            }

            _repository.Delete(id);
        }
    }
}