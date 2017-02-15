using System;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;
using WebService.Models;
using WebService.Services;


namespace WebService.Controllers
{
    public class TodoItemsController : BaseApiController
    {
        static readonly INotesService notesService = new NotesService(new NotesRepository());

        [HttpGet]
        //[BasicAuthentication(RequireSsl = false)]
        public HttpResponseMessage Get()
        {
            return base.BuildSuccessResult(HttpStatusCode.OK, notesService.GetData());
        }

        [HttpPost]
        //[BasicAuthentication(RequireSsl = false)]
        public HttpResponseMessage Create(NotesItem item)
        {
            try
            {
                if (item == null ||
                    string.IsNullOrWhiteSpace(item.Name) ||
                    string.IsNullOrWhiteSpace(item.Text))
                {
                    return base.BuildErrorResult(HttpStatusCode.BadRequest, ErrorCode.NotesItemIDInUse.ToString());
                }

                // Determine if the ID already exists
                var itemExists = notesService.DoesItemExist(item.Id);
                if (itemExists)
                {
                    //return base.BuildErrorResult(HttpStatusCode.Conflict, ErrorCode.TodoItemIDInUse.ToString());
                    return base.BuildErrorResult(HttpStatusCode.Conflict, ErrorCode.NotesItemIDInUse.ToString());
                }
                notesService.InsertData(item);
            }
            catch (Exception)
            {
                return base.BuildErrorResult(HttpStatusCode.BadRequest, ErrorCode.CouldNotCreateItem.ToString());
            }

            return base.BuildSuccessResult(HttpStatusCode.Created);
        }

        [HttpPut]
        //[BasicAuthentication(RequireSsl = false)]
        public HttpResponseMessage Edit(string id, NotesItem item)
        {
            try
            {
                if (item == null ||
                    string.IsNullOrWhiteSpace(item.Name) ||
                    string.IsNullOrWhiteSpace(item.Text))
                {
                    return base.BuildErrorResult(HttpStatusCode.BadRequest, ErrorCode.NotesItemNameAndNotesRequired.ToString());
                }

                var todoItem = notesService.Find(id);
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

        [HttpDelete]
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