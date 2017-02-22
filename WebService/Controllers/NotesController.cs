using System;
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
        [Route("GetAllNotes")]
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
                    result = Request.CreateResponse(HttpStatusCode.BadRequest);// BadRequest("Item = null");
                }
                else
                {
                    var itemExists = NotesRepository.DoesItemExist(AccountRepository.GetIdByUserName(User.Identity.Name), item.NoteId);
                    if (itemExists)
                    {
                        result = Request.CreateResponse(HttpStatusCode.BadRequest); //result = BadRequest("Comflict");
                    }
                    else
                    {
                        NotesRepository.Insert(AccountRepository.GetIdByUserName(User.Identity.Name), item);
                        result = Request.CreateResponse(HttpStatusCode.OK);//Ok("OK");
                    }
                }
            }
            catch (Exception)
            {
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
                        result = BuildSuccessResult(HttpStatusCode.NoContent);
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
    }

}



