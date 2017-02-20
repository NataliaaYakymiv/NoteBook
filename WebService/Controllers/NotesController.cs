using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;
using WebService.Models;
using WebService.Services;

namespace WebService.Controllers
{

    [System.Web.Http.Authorize]
    [System.Web.Http.RoutePrefix("api/Notes")]
    public class TodoItemsController : BaseApiController
    {
        static readonly INotesService notesService = new NotesService(new NotesRepository());

        public HttpResponseMessage Get()
        {
            var notes = notesService.GetData();
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

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("Create")]
        public IHttpActionResult Create(NoteModel item)
        {
            try
            {
                if (item == null ||
                    string.IsNullOrWhiteSpace(item.NoteName) ||
                    string.IsNullOrWhiteSpace(item.NoteText))
                {
                    return BadRequest("Item = null");
                }

                // Determine if the ID already exists
                var itemExists = notesService.DoesItemExist(item.NoteId);
                if (itemExists)
                {
                    //return base.BuildErrorResult(HttpStatusCode.Conflict, ErrorCode.TodoItemIDInUse.ToString());
                    return BadRequest("Comflict");
                }
                notesService.InsertData(item);
                return Ok("OK");
            }
            catch (Exception)
            {
                return BadRequest("could not create");
            }

            //return BadRequest("Error");
        }

        [System.Web.Http.HttpPut]
        [System.Web.Http.Route("Edit")]
        //[BasicAuthentication(RequireSsl = false)]
        public HttpResponseMessage Edit(NoteModel item)
        {
            try
            {
                if (item == null ||
                    string.IsNullOrWhiteSpace(item.NoteName) ||
                    string.IsNullOrWhiteSpace(item.NoteText))
                {
                    return base.BuildErrorResult(HttpStatusCode.BadRequest, ErrorCode.NotesItemNameAndNotesRequired.ToString());
                }

                var todoItem = notesService.Find(item.NoteId);
                if (todoItem != null)
                {
                    notesService.UpdateData(item);
                }
                else
                {
                    return base.BuildErrorResult(HttpStatusCode.NotFound, ErrorCode.RecordNotFound.ToString());
                }
            }
            catch (Exception)
            {
                return base.BuildErrorResult(HttpStatusCode.BadRequest, ErrorCode.CouldNotUpdateItem.ToString());
            }

            return base.BuildSuccessResult(HttpStatusCode.NoContent);
        }

        [System.Web.Http.HttpDelete]
        [System.Web.Http.Route("Delete")]
        //[BasicAuthentication(RequireSsl = false)]
        public HttpResponseMessage Delete(string id)
        {
            try
            {
                var todoItem = notesService.Find(id);
                if (todoItem != null)
                {
                    notesService.DeleteData(id);
                }
                else
                {
                    return base.BuildErrorResult(HttpStatusCode.NotFound, ErrorCode.RecordNotFound.ToString());
                }
            }
            catch (Exception)
            {
                return base.BuildErrorResult(HttpStatusCode.BadRequest, ErrorCode.CouldNotDeleteItem.ToString());
            }

            return base.BuildSuccessResult(HttpStatusCode.NoContent);
        }
    }
}