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
    public class TodoItemsController : BaseApiController
    {
        static readonly INotesRepository NotesRepository = new NotesRepository();
        static readonly IAccountRepository AccountRepository = new AccountRepository();

        [HttpGet]
        [Route("Refresh")]
        public HttpResponseMessage Get()
        {
            var notes = NotesRepository.All(AccountRepository.GetIbByUserName(User.Identity.Name));
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
        [Route("Create")]
        public IHttpActionResult Create(NoteModel item)
        {
            IHttpActionResult result;

            try
            {
                if (item == null ||
                    string.IsNullOrWhiteSpace(item.NoteName) ||
                    string.IsNullOrWhiteSpace(item.NoteText))
                {
                    result = BadRequest("Item = null");
                }
                else
                {
                    var itemExists = NotesRepository.DoesItemExist(AccountRepository.GetIbByUserName(User.Identity.Name), item.NoteId);
                    if (itemExists)
                    { 
                        result = BadRequest("Comflict");
                    }
                    else
                    {
                        NotesRepository.Insert(AccountRepository.GetIbByUserName(User.Identity.Name), item);
                        result = Ok("OK");
                    }
                }
            }
            catch (Exception)
            {
                result = BadRequest("Could not create");
            }

            return result;
        }

        [HttpPut]
        [Route("Edit")]
        public HttpResponseMessage Edit(NoteModel item)
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

                    var todoItem = NotesRepository.Find(AccountRepository.GetIbByUserName(User.Identity.Name),
                        item.NoteId);
                    if (todoItem != null)
                    {
                        NotesRepository.Update(AccountRepository.GetIbByUserName(User.Identity.Name), item);
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
        [Route("Delete")]
        public HttpResponseMessage Delete(NoteModel item)
        {
            HttpResponseMessage result;

            try
            {
                var todoItem = NotesRepository.Find(AccountRepository.GetIbByUserName(User.Identity.Name), item.NoteId);
                if (todoItem != null)
                {
                    NotesRepository.Delete(AccountRepository.GetIbByUserName(User.Identity.Name), item.NoteId);
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



