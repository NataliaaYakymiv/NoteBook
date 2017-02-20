using NoteBook.Models;
using NoteBook.Servises;
using System;

using Xamarin.Forms;

namespace NoteBook.Pages
{
    public partial class UpdateNotePage : ContentPage
    {
        public UpdateNotePage()
        {
            InitializeComponent();
        }
        public async void OnUpdateNote(object sender, EventArgs e)
        {
            NoteModel credentials = new NoteModel();
            credentials.NoteId = "b94afb54-a1cb-4313-8af3-b7511551b33b";
            credentials.NoteName = "name";//NoteNameEntry.Text;
            credentials.NoteText = "text";//NoteTextEntry.Text;

            var response = NotesService.GetService().Edit(credentials);

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
