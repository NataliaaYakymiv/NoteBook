using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
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

        [HttpPost]
        [Route("PostImage")]
        public async Task<HttpResponseMessage> PostFormData(string filename)
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string root = HttpContext.Current.Server.MapPath("~/Userimage");
            var provider = new MultipartFileStreamProvider(root);

            try
            {
                // Read the form data.
                await Request.Content.ReadAsMultipartAsync(provider);

                // This illustrates how to get the file names.
                foreach (MultipartFileData file in provider.FileData)
                {
                    //file.LocalFileName
                    //System.IO.File.Move(file.LocalFileName, root + Guid.NewGuid() + file.Headers.ContentDisposition.FileName);
                    Trace.WriteLine(file.Headers.ContentDisposition.FileName);
                    Trace.WriteLine("Server file path: " + file.LocalFileName);
                    Trace.WriteLine(filename);
                }
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (System.Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }
        //public  IHttpActionResult PostImage()
        //{
        //    //var file = HttpContext.Current.Request.Files.Count > 0 ?
        //    // HttpContext.Current.Request.Files[0] : null;

        //    //if (file != null && file.ContentLength > 0)
        //    //{
        //    //    var fileName = Path.GetFileName(file.FileName);

        //    //    var path = Path.Combine(
        //    //        HttpContext.Current.Server.MapPath("~/uploads"),
        //    //        fileName
        //    //    );

        //    //    file.SaveAs(path);
        //    //}

        //    //return Ok(file != null ? "/Userimage/" + file.FileName : "");
        //    Dictionary<string, object> dict = new Dictionary<string, object>();
        //    try
        //    {

        //        var httpRequest = HttpContext.Current.Request;

        //        foreach (string file in httpRequest.Files)
        //        {
        //            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);

        //            var postedFile = httpRequest.Files[file];
        //            if (postedFile != null && postedFile.ContentLength > 0)
        //            {

        //                int MaxContentLength = 1024 * 1024 * 1; //Size = 1 MB  

        //                IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };
        //                var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
        //                var extension = ext.ToLower();
        //                if (!AllowedFileExtensions.Contains(extension))
        //                {

        //                    var message = string.Format("Please Upload image of type .jpg,.gif,.png.");

        //                    dict.Add("error", message);
        //                    return BadRequest(dict.ToString());
        //                }
        //                else if (postedFile.ContentLength > MaxContentLength)
        //                {

        //                    var message = string.Format("Please Upload a file upto 1 mb.");

        //                    dict.Add("error", message);
        //                    return BadRequest();
        //                }
        //                else
        //                {



        //                    var filePath = HttpContext.Current.Server.MapPath("~/Userimage/" + postedFile.FileName + extension);

        //                    postedFile.SaveAs(filePath);

        //                }
        //            }

        //            var message1 = string.Format("Image Updated Successfully.");
        //            return Ok(); 
        //        }
        //        var res = string.Format("Please Upload a image.");
        //        dict.Add("error", res);
        //        return NotFound();
        //    }
        //    catch (Exception ex)
        //    {
        //        var res = string.Format("some Message");
        //        dict.Add("error", res);
        //        return NotFound();
        //    }
        //}
    }

}



