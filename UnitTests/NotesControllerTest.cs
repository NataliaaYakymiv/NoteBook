using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Effort;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using WebService.Contracts;
using WebService.Controllers;
using WebService.Models;
using WebService.Repositories;

namespace UnitTests
{
    [TestClass]
    public class NotesControllerTest
    {
        private NotesRepository _notesRepository;
        private NotesController _notesController;
        private NotesContext _notesContext;

        public NotesControllerTest()
        {
            var connection = DbConnectionFactory.CreateTransient();
            _notesContext = new NotesContext(connection); // moq table notes
            _notesRepository = new NotesRepository(_notesContext);

            var mockAccountRepository = new Mock<IAccountRepository>(); // moq account repo

            _notesController = new NotesController(_notesRepository, mockAccountRepository.Object)
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };

            Init();
        }

        #region InitContext
        private void Init()
        {
            var note1 = new NoteModel() { NoteId = "1", NoteName = "Name1", NoteText = "Text1", Id = 0, Create = new DateTime(2017, 01, 01, 01, 01,01, 01)};
            var note2 = new NoteModel() { NoteId = "2", NoteName = "Name2", NoteText = "Text2", Id = 0, Create = new DateTime(2017, 02, 01, 01, 01, 01, 01) };
            var note3 = new NoteModel() { NoteId = "3", NoteName = "Name3", NoteText = "Text3", Id = 0, Create = new DateTime(2017, 03, 01, 01, 01, 01, 01) };
            var note4 = new NoteModel() { NoteId = "4", NoteName = "Name4", NoteText = "Text4", Id = 0, Create = new DateTime(2017, 04, 01, 01, 01, 01, 01) };

            _notesContext.NoteModels.Add(note1);
            _notesContext.NoteModels.Add(note2);
            _notesContext.NoteModels.Add(note3);
            _notesContext.NoteModels.Add(note4);
        }
        #endregion

        #region GetAllTest
        [TestMethod]
        public void GetAllNote_ValidNotes_OK()
        {

            // Act
            HttpResponseMessage actionResult = _notesController.GetAllNotes();
            var contentResult = JsonConvert.DeserializeObject<List<NoteModel>>(actionResult.Content.ReadAsStringAsync().Result);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, actionResult.StatusCode);
            CollectionAssert.AreEquivalent(_notesRepository.All(0).ToList(), contentResult);
        }
        #endregion

        #region GetSyncTest
        [TestMethod]
        public void GetSyncNote_NotNotesInLocal_OK()
        {
            // Arrange
            var timeSync = new DateTime(2016, 03, 01, 01, 01, 01, 01);
            SyncModel syncModel = new SyncModel() {LastModify = timeSync.ToString("G"), NoteModels = new List<NoteModel>()};

            // Act
            HttpResponseMessage actionResult = _notesController.GetAsyncNotes(syncModel);
            var contentResult = JsonConvert.DeserializeObject<SyncModel>(actionResult.Content.ReadAsStringAsync().Result);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, actionResult.StatusCode);
            Assert.IsNotNull(contentResult.LastModify);
            CollectionAssert.AreEquivalent(_notesRepository.All(0).Where(x => (x.Create > timeSync) || (x.Update > timeSync) || (x.Delete > timeSync)).ToList(), contentResult.NoteModels);
        }
        #endregion

        #region CreateNoteTest
        [TestMethod]
        public void CreateNote_ValidNote_OK()
        {
            // Arrange
            var note = new NoteModel() {NoteId = "create_test_valid", NoteName = "Name", NoteText = "Text"};

            // Act
            HttpResponseMessage actionResult = _notesController.CreateNote(note);
            var contentResult = JsonConvert.DeserializeObject<NoteModel>(actionResult.Content.ReadAsStringAsync().Result);

            // Assert

            // check success response 
            Assert.AreEqual(HttpStatusCode.OK, actionResult.StatusCode);
            Assert.IsNotNull(actionResult.Content);

            // check valid object
            Assert.AreEqual(0, contentResult.UserId);
            Assert.AreEqual(note.NoteId, contentResult.NoteId);
            Assert.AreEqual(note.NoteName, contentResult.NoteName);
            Assert.AreEqual(note.NoteText, contentResult.NoteText);
            Assert.IsNotNull(contentResult.Create);
        }

        [TestMethod]
        public void CreateNote_ValidNoteAlreadyExist_BadRequest()
        {
            // Arrange
            var note = new NoteModel() { NoteId = "create_test_valid_already_exist", NoteName = "Name", NoteText = "Text" };
            _notesRepository.Insert(0, note);
            // Act
            HttpResponseMessage actionResult = _notesController.CreateNote(note);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, actionResult.StatusCode);  
        }

        [TestMethod]
        public void CreateNote_NotValidNote_BadRequest()
        {
            // Arrange
            var note1 = new NoteModel() { NoteId = "create_test_not_valid", NoteName = null, NoteText = null };
            NoteModel note2 = null;


            // Act
            HttpResponseMessage actionResultNullFields = _notesController.CreateNote(note1);
            HttpResponseMessage actionResultNullObj = _notesController.CreateNote(note2);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, actionResultNullFields.StatusCode);
            Assert.AreEqual(HttpStatusCode.BadRequest, actionResultNullObj.StatusCode);
        }
        #endregion

        #region UpdateNoteTest
        [TestMethod]
        public void UpdateNote_ValidNote_OK()
        {
            // Arrange

            var note = new NoteModel() { NoteId = "update_valid", NoteName = "Name", NoteText = "Text" };
            _notesRepository.Insert(0, note);

            note.NoteName = "Name_update";
            note.NoteText = "Text_update";

            // Act
            HttpResponseMessage actionResult = _notesController.UpdateNote(note);
            var contentResult = JsonConvert.DeserializeObject<NoteModel>(actionResult.Content.ReadAsStringAsync().Result);

            // Assert

            // check success response 
            Assert.AreEqual(HttpStatusCode.OK, actionResult.StatusCode);
            Assert.IsNotNull(actionResult.Content);

            // check valid object
            Assert.AreEqual(0, contentResult.UserId);
            Assert.AreEqual(note.NoteId, contentResult.NoteId);
            Assert.AreEqual(note.NoteName, contentResult.NoteName);
            Assert.AreEqual(note.NoteText, contentResult.NoteText);
            Assert.IsNotNull(contentResult.Create);
            Assert.IsNotNull(contentResult.Update);
        }

        [TestMethod]
        public void UpdateNote_ValidNoteNotExist_NotFound()
        {
            // Arrange
            var note = new NoteModel() { NoteId = "update_valid_not_exist", NoteName = "Name", NoteText = "Text" };

            // Act
            HttpResponseMessage actionResult = _notesController.UpdateNote(note);

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, actionResult.StatusCode);
        }

        [TestMethod]
        public void UpdateNote_NotValidNote_BadRequest()
        {
            // Arrange
            var note1 = new NoteModel() { NoteId = "update_not_valid", NoteName = null, NoteText = null };
            NoteModel note2 = null;


            // Act
            HttpResponseMessage actionResultNullFields = _notesController.UpdateNote(note1);
            HttpResponseMessage actionResultNullObj = _notesController.UpdateNote(note2);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, actionResultNullFields.StatusCode);
            Assert.AreEqual(HttpStatusCode.BadRequest, actionResultNullObj.StatusCode);
        }
        #endregion

        #region DeleteNoteTest
        [TestMethod]
        public void DeleteNote_ValidNote_OK()
        {
            // Arrange

            var note = new NoteModel() { NoteId = "delete_valid", NoteName = "Name", NoteText = "Text" };
            _notesRepository.Insert(0, note);

            // Act
            HttpResponseMessage actionResult = _notesController.DeleteNote(note);

            // Assert 
            Assert.AreEqual(HttpStatusCode.OK, actionResult.StatusCode);
        }

        [TestMethod]
        public void DeleteNote_ValidNoteNotExist_NotFound()
        {
            // Arrange

            var note = new NoteModel() { NoteId = "delete_valid_not_esixt", NoteName = "Name", NoteText = "Text" };

            // Act
            HttpResponseMessage actionResult = _notesController.DeleteNote(note);

            // Assert 
            Assert.AreEqual(HttpStatusCode.NotFound, actionResult.StatusCode);
        }
        #endregion

    }
}
