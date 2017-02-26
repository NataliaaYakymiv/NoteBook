using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebService.Models;
using WebService.Repositories;
using WebService.Contracts;

namespace WebService.Controllers
{
    [Authorize]
    [RoutePrefix("api/Notes")]
    public class NotesController : BaseApiController
    {
        static readonly INotesRepository NotesRepository = new NotesRepository();
        static readonly IAccountRepository AccountRepository = new AccountRepository();

        [HttpGet]
        //[Route("GetAllNotes")]
        public HttpResponseMessage GetAllNotes()
        {
            var notes = NotesRepository.All(AccountRepository.GetIdByUserName(User.Identity.Name));
            HttpResponseMessage response;
            if (notes == null)
            {
                response = Request.CreateResponse(HttpStatusCode.NoContent);
            }
            else
            {
                response = Request.CreateResponse(HttpStatusCode.OK, notes);
            }

            return response;
        }



        [HttpPost]
        [Route("CreateNote")]
        public HttpResponseMessage CreateNote(NoteModel item)
        {
            HttpResponseMessage result;

            try
            {
                if (item == null ||
                    string.IsNullOrWhiteSpace(item.NoteName) ||
                    string.IsNullOrWhiteSpace(item.NoteText))
                {
                    result = Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                else
                {
                    var itemExists = NotesRepository.DoesItemExist(
                        AccountRepository.GetIdByUserName(User.Identity.Name), item.NoteId);
                    if (itemExists)
                    {
                        result = Request.CreateResponse(HttpStatusCode.BadRequest);
                    }
                    else
                    {
                        NotesRepository.Insert(AccountRepository.GetIdByUserName(User.Identity.Name), item);
                        var note = NotesRepository.Find(AccountRepository.GetIdByUserName(User.Identity.Name),
                            item.NoteId);
                        result = Request.CreateResponse(HttpStatusCode.OK, note);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Print("Bug!!!!!!!!!!!!!!!!!!!! "  + e.Message);
                result = Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            return result;
        }

        [HttpPut]
        [Route("UpdateNote")]
        public HttpResponseMessage UpdateNote(NoteModel item)
        {
            HttpResponseMessage result;

            try
            {
                if (item == null ||
                    string.IsNullOrWhiteSpace(item.NoteName) ||
                    string.IsNullOrWhiteSpace(item.NoteText))
                {
                    result = BuildErrorResult(HttpStatusCode.BadRequest,
                        ErrorCode.NotesItemNameAndNotesRequired.ToString());
                }
                else
                {

                    var todoItem = NotesRepository.Find(AccountRepository.GetIdByUserName(User.Identity.Name),
                        item.NoteId);
                    if (todoItem != null)
                    {
                        NotesRepository.Update(AccountRepository.GetIdByUserName(User.Identity.Name), item);
                        var note = NotesRepository.Find(AccountRepository.GetIdByUserName(User.Identity.Name),
                            item.NoteId);
                        result = BuildSuccessResult(HttpStatusCode.OK, note);
                    }
                    else
                    {
                        result = BuildErrorResult(HttpStatusCode.NotFound, ErrorCode.RecordNotFound.ToString());
                    }
                }
            }
            catch (Exception)
            {
                result = BuildErrorResult(HttpStatusCode.BadRequest, ErrorCode.CouldNotUpdateItem.ToString());
            }

            return result;
        }

        [HttpPost]
        [Route("DeleteNote")]
        public HttpResponseMessage DeleteNote(NoteModel item)
        {
            HttpResponseMessage result;

            try
            {
                var todoItem = NotesRepository.Find(AccountRepository.GetIdByUserName(User.Identity.Name), item.NoteId);
                if (todoItem != null)
                {
                    NotesRepository.Delete(AccountRepository.GetIdByUserName(User.Identity.Name), item.NoteId);
                    result = BuildSuccessResult(HttpStatusCode.NoContent);
                }
                else
                {
                    result = BuildErrorResult(HttpStatusCode.NotFound, ErrorCode.RecordNotFound.ToString());
                }
            }
            catch (Exception)
            {
                result = BuildErrorResult(HttpStatusCode.BadRequest, ErrorCode.CouldNotDeleteItem.ToString());
            }

            return result;
        }

        [HttpPost]
        [Route("GetSyncNotes")]
        public HttpResponseMessage GetAsyncNotes(SyncModel model)
        {
            SyncModel syncModel = new SyncModel();
            var notes =
                NotesRepository.HasChanges(AccountRepository.GetIdByUserName(User.Identity.Name), model).ToList();
            HttpResponseMessage response;
            syncModel.NoteModels = notes;
            syncModel.LastModify = DateTime.Now;
            response = Request.CreateResponse(HttpStatusCode.OK, syncModel);


            return response;
        }
    }

}



