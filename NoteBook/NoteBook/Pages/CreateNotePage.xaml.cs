using NoteBook.Models;
using System;
using System.Net.Http;
using NoteBook.Contracts;
using Xamarin.Forms;

namespace NoteBook.Pages
{
    public partial class CreateNotePage : ContentPage
    {
        public INotesService NotesService { get; private set; }

        private CreateNotePage()
        {
            Title = "Create note";
            InitializeComponent();
            StateLabel.Text = string.Empty;
        }

        public CreateNotePage(INotesService notesService) : this()
        {
            NotesService = notesService;
        }

        public async void OnCreateNote(object sender, EventArgs e)
        {
            var note = new NoteModel
            {
                NoteName = NoteNameEntry.Text,
                NoteText = NoteTextEntry.Text
            };

            try
            {
                ActivityIndicatorCreateNote.IsRunning = true;
                ActivityIndicatorCreateNote.IsVisible = true;
                NoteNameEntry.IsEnabled = false;
                NoteTextEntry.IsEnabled = false;
                CreateBtn.IsVisible= false;

                var result = await NotesService.CreateNote(note);

                ActivityIndicatorCreateNote.IsRunning = false;
                ActivityIndicatorCreateNote.IsVisible = false;
                NoteNameEntry.IsEnabled = true;
                NoteTextEntry.IsEnabled = true;
                CreateBtn.IsVisible = true;

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
