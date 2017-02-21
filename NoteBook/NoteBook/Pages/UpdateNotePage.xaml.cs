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

        public UpdateNotePage(NoteModel item)
        {
            InitializeComponent();
            ForceLayout();

            NoteNameEntry.Text = item.NoteName;
            NoteTextEntry.Text = item.NoteText;
            NoteIdEntry.Text = item.NoteId.ToString();
        }

        public async void OnUpdateNote(object sender, EventArgs e)
        {
            NoteModel note = new NoteModel();
            note.NoteId =Int32.Parse(NoteIdEntry.Text);
            note.NoteName = NoteNameEntry.Text;
            note.NoteText = NoteTextEntry.Text;

            var response = NotesService.GetService().Edit(note);
            StateLabel.Text = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                NoteNameEntry.Text = string.Empty;
                NoteTextEntry.Text = string.Empty;
            }

            await Navigation.PushAsync(new NotePage());
        }
    }
}
