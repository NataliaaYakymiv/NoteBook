using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebService.Models;
using WebService.Services;

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

        public NoteModel Find(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException("id");
            }

            return _repository.Find(id);
        }

        public IEnumerable<NoteModel> GetData()
        {
            return _repository.All;
        }

        public void InsertData(NoteModel item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            _repository.Insert(item);
        }

        public void UpdateData(NoteModel item)
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