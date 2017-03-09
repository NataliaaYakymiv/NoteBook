using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using WebService.Models;
using WebService.Contracts;

namespace WebService.Repositories
{
    public class NotesRepository : INotesRepository
    {
        private NotesContext _db;

        public NotesRepository()
        {
            InitializeData();
        }

        public bool DoesItemExist(int idUser, string idNote)
        {
            return _db.NoteModels.Any(item => item.NoteId.Equals(idNote) && item.UserId == idUser);
        }

        public IEnumerable<NoteModel> All(int iduser)
        {
            return _db.NoteModels.Where(item => item.UserId == iduser && item.Delete == null).ToList();
        }

        public NoteModel Find(int idUser, string idNote)
        {
            return _db.NoteModels.FirstOrDefault(item => item.NoteId.Equals(idNote) && item.UserId == idUser);
        }

        public IEnumerable<NoteModel> HasChanges(int idUser, SyncModel model)
        {
            var list = new List<NoteModel>();

            var last = DateTime.Parse(model.LastModify);
            var notes = _db.NoteModels.Where(item => (item.Create > last || item.Update > last || item.Delete > last) && item.UserId == idUser).ToList();

            if (notes.Count == 0) // no changes in remote 
            {
                foreach (var notemodels in model.NoteModels)
                {
                    if (notemodels.Delete == null)
                    {
                        var item = Find(idUser, notemodels.NoteId);
                        if (item != null)
                        {
                            Update(idUser, notemodels);
                        }
                        else
                        {
                            Insert(idUser, notemodels);
                        }
                    }
                    else
                    {
                        Delete(idUser, notemodels.NoteId);
                    }
                    list.Add(Find(idUser, notemodels.NoteId));
                }
            }

            else if (model.NoteModels.Count == 0) // no changes in local 
            {
                list = notes;
            }
            else // changes in local and remote 
            {
                for (int i = 0; i < model.NoteModels.Count; i++)
                {
                    var note = Find(idUser, model.NoteModels[i].NoteId);
                    if (note != null)
                    {
                        if (model.NoteModels[i].Delete != null)
                        {
                            Delete(idUser, model.NoteModels[i].NoteId);
                        }
                        else
                        {
                            Update(idUser, model.NoteModels[i]);
                        }
                    }
                    else
                    {
                        Insert(idUser, model.NoteModels[i]);
                        
                    }
                    list.Add(Find(idUser, model.NoteModels[i].NoteId));
                }
                foreach (var n in notes)
                {
                    list.Add(Find(idUser, n.NoteId));
                }
            }

            return list;
        }

        public void Insert(int idUser, NoteModel item)
        {
            item.UserId = idUser;
            item.Create = DateTime.Now;
            _db.NoteModels.Add(item);
            _db.SaveChanges();
        }


        public void Update(int idUser, NoteModel noteModel)
        {
            var note = Find(idUser, noteModel.NoteId);
            if (note != null)
            {
                note.NoteName = noteModel.NoteName;
                note.NoteText = noteModel.NoteText;
                note.Image = noteModel.Image;
                note.Update = DateTime.Now;
                _db.Entry(note).State = EntityState.Modified;

                _db.SaveChanges();
            }
        }


        public void Delete(int idUser, string idNote)
        {
            var note = Find(idUser, idNote);
            if (note != null)
            {
                note.Delete = DateTime.Now;
                _db.Entry(note).State = EntityState.Modified;
                _db.SaveChanges();
            }
        }

        public void SetImage(string noteId, string path)
        {
            var result = _db.NoteModels.FirstOrDefault(item => item.NoteId == noteId);
            if (result != null)
            {
                result.Image = path;
                result.Update = DateTime.Now;
                _db.Entry(result).State = EntityState.Modified;
                _db.SaveChanges();
            }
        }

        #region Helpers

        private void InitializeData()
        {
            _db = new NotesContext();
        }
        #endregion
    }
}