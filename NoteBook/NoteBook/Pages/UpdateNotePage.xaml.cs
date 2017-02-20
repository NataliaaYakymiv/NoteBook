using NoteBook.Models;
using NoteBook.Servises;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            credentials.NoteId = 1;
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
        }
    }
}
