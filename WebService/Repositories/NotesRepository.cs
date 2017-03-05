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
                for (int j = 0; j < model.NoteModels.Count; j++)
                //for (int i = 0; i < notes.Count; i++) 
                {
                    //for (int j = 0; j < model.NoteModels.Count; j++) 
                    for (int i = 0; i < notes.Count; i++)
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

                        //InsertFromLocal(idUser, model.NoteModels[j]); 
                        //list.Add(model.NoteModels[j]); 
                    }
                    Insert(idUser, model.NoteModels[j]);
                    list.Add(Find(idUser, model.NoteModels[j].NoteId));
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
                note.NoteName = note.NoteName;
                note.NoteText = note.NoteText;
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