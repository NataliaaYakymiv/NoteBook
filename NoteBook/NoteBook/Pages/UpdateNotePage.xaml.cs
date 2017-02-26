using NoteBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using NoteBook.Contracts;
using NoteBook.Services;
using Xamarin.Forms;

namespace NoteBook.Pages
{
    public partial class UpdateNotePage : ContentPage
    {
        public INotesService NotesService { get; private set; }
        public NoteModel NoteModel { get; private set; }

        private UpdateNotePage()
        {
            Title = "Create note";
            InitializeComponent();
        }

        public UpdateNotePage(INotesService notesService) : this()
        {
            NotesService = notesService;
            StateLabel.Text = string.Empty;
        }

        public void SetNoteModel(NoteModel model)
        {
            ForceLayout();
            NoteModel = model;

            NoteNameEntry.Text = model.NoteName;
            NoteTextEntry.Text = model.NoteText;
            CreatedLabel.Text = "Created: " + model.Create;
            UpdatedLabel.Text = !string.IsNullOrEmpty(model.Update) ? "Last update: " + model.Update : string.Empty;
        }

        public async void OnUpdateNote(object sender, EventArgs e)
        {
            NoteModel.NoteName = NoteNameEntry.Text;
            NoteModel.NoteText = NoteTextEntry.Text;

            try
            {
                ActivityIndicatorUdpateNote.IsRunning = true;
                ActivityIndicatorUdpateNote.IsVisible = true;
                UpdateBtn.IsEnabled = false;

                var result = await NotesService.UpdateNote(NoteModel);

                ActivityIndicatorUdpateNote.IsRunning = false;
                ActivityIndicatorUdpateNote.IsVisible = false;
                UpdateBtn.IsEnabled = true;

                if (result)
                {
                    NoteNameEntry.Text = string.Empty;
                    NoteTextEntry.Text = string.Empty;

                    await Navigation.PopAsync();
                }
                else
                {
                    StateLabel.Text = "Something wrong, try again";
                }
            }
            catch (HttpRequestException)
            {
                await Navigation.PopToRootAsync();
            }
            catch (InvalidOperationException exception)
            {
                StateLabel.Text = exception.Message;
            }
            catch (InvalidCastException exception)
            {
                StateLabel.Text = exception.Message;
            }
        }
    }
}
