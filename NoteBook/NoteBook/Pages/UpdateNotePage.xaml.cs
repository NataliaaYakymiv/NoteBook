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
            NoteModel credentials = new NoteModel();
            credentials.NoteId =Int32.Parse(NoteIdEntry.Text);
            credentials.NoteName = NoteNameEntry.Text;
            credentials.NoteText = NoteTextEntry.Text;

            var response = NotesService.GetService().Edit(credentials);

            var v = credentials;
            NoteNameEntry.Text = string.Empty;
            StateLabel.Text = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                NoteTextEntry.Text = string.Empty;
            }

            await Navigation.PushAsync(new NotePage());
        }
    }
}
