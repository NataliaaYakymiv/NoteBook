using NoteBook.Models;
using NoteBook.Servises;
using System;

using Xamarin.Forms;

namespace NoteBook.Pages
{
    public partial class CreateNotePage : ContentPage
    {
        public CreateNotePage()
        {
            InitializeComponent();
        }
        public async void OnCreateNote(object sender, EventArgs e)
        {
            NoteModel credentials = new NoteModel();
            credentials.NoteId = "hkdll";
            credentials.NoteName = "name";//NoteNameEntry.Text;
            credentials.NoteText = "text";//NoteTextEntry.Text;

            var response = NotesService.GetService().Create(credentials);

            var v = credentials;
            NoteNameEntry.Text = string.Empty;
            StateLabel.Text = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                NoteTextEntry.Text = string.Empty;
            }
        }
    }
}
