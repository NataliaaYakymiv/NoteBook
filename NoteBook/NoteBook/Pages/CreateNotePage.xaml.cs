using NoteBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoteBook.Services;
using Xamarin.Forms;

namespace NoteBook.Pages
{
    public partial class CreateNotePage : ContentPage
    {
        public CreateNotePage()
        {
            InitializeComponent();
        }

        public CreateNotePage(NoteModel item)
        {
            //update behavior
            NoteNameEntry.Text = item.NoteName;
            NoteTextEntry.Text = item.NoteText;
            InitializeComponent();
        }

        public async void OnCreateNote(object sender, EventArgs e)
        {
            NoteModel credentials = new NoteModel();
            credentials.NoteName = NoteNameEntry.Text;
            credentials.NoteText = NoteTextEntry.Text;

            var response = NotesService.GetService().CreateNote(credentials);

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
