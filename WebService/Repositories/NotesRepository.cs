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
            return _db.NoteModels.Where(item => item.UserId == iduser).ToList();
        }

        public NoteModel Find(int idUser, string idNote)
        {
            return _db.NoteModels.FirstOrDefault(item => item.NoteId.Equals(idNote) && item.UserId == idUser);
        }

        public IEnumerable<NoteModel> HasChanges(int idUser, SyncModel model)
        {
            var list = new List<NoteModel>();
            
            var notes = _db.NoteModels.Where(item => (item.Create > model.LastModify || item.Update > model.LastModify || item.Delete > model.LastModify) && item.UserId == idUser).ToList();

            if (notes.Count == 0) // no changes in remote
            {
                foreach (var notemodels in model.NoteModels)
                {
                    if (notemodels.Delete == null)
                    {
                        var item = Find(idUser, notemodels.NoteId);
                        if (item != null)
                        {
                            UpdateFromLocal(idUser, notemodels);
                        }
                        else
                        {
                            InsertFromLocal(idUser, notemodels);
                        }
                        list.Add(Find(idUser, notemodels.NoteId));
                    }
                    else
                    {
                        Delete(idUser, notemodels.NoteId);
                    }
                }

                
            }
            
            else if (model.NoteModels.Count == 0) // no changes in local
            {
                list = notes;
            }
            else // changes in local and remote
            {
                for (int i = 0; i < notes.Count; i++)
                {
                    for (int j = 0; j < model.NoteModels.Count; j++)
                    {
                        if ((model.NoteModels[j].Delete != null && notes[i].Update < model.NoteModels[j].Delete) ||
                            (model.NoteModels[j].Delete != null && notes[i].Update == null))
                        {
                            Delete(idUser, model.NoteModels[j].NoteId);
                            notes.Remove(model.NoteModels[j]);
                            j--;
                            continue;
                        }

                        if (model.NoteModels[j].Update != null)
                        {
                            if (model.NoteModels[j].NoteId == notes[i].NoteId)
                            {
                                if ((model.NoteModels[j].Update > notes[i].Update) || notes[i].Update == null)
                                {
                                    Update(idUser, model.NoteModels[j]);
                                    list.Add(model.NoteModels[j]);
                                }
                                notes.Remove(notes[i]);
                                model.NoteModels.Remove(model.NoteModels[j]);
                                i--;
                                j--;
                                break;
                            }
                        }

                        InsertFromLocal(idUser, model.NoteModels[j]);
                        list.Add(model.NoteModels[j]);
                    }
                }

            }

            return list;
        }

        public void Insert(int idUser, NoteModel item)
        {
            item.NoteId = Guid.NewGuid().ToString();
            item.UserId = idUser;
            item.Create = DateTime.Now;
            _db.NoteModels.Add(item);
            _db.SaveChanges();
        }

        public void InsertFromLocal(int idUser, NoteModel item)
        {
            item.UserId = idUser;
            _db.NoteModels.Add(item);
            _db.SaveChanges();

        }

        public void Update(int idUser, NoteModel note)
        {
            var result = _db.NoteModels.SingleOrDefault(item => item.NoteId == note.NoteId && item.UserId == idUser);
            if (result != null)
            {
                result.NoteName = note.NoteName;
                result.NoteText = note.NoteText;
                result.Update = DateTime.Now;
                _db.Entry(result).State = EntityState.Modified;

                _db.SaveChanges();
            }
        }

        public void UpdateFromLocal(int idUser, NoteModel note)
        {
            var result = _db.NoteModels.SingleOrDefault(item => item.NoteId == note.NoteId && item.UserId == idUser);
            if (result != null)
            {
                result.NoteName = note.NoteName;
                result.NoteText = note.NoteText;
                result.Update = note.Update;
                _db.Entry(result).State = EntityState.Modified;

                _db.SaveChanges();
            }
        }

        public void Delete(int idUser, string idNote)
        {
            var note = Find(idUser, idNote);
            if (note!=null)
            {
                _db.NoteModels.Remove(note);
            }
            _db.SaveChanges();
        }

        

        #region Helpers

        private void InitializeData()
        {
            _db = new NotesContext();
        }
        #endregion
    }
}