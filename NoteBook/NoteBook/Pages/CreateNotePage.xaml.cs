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
    public partial class CreateNotePage : ContentPage
    {
        public CreateNotePage()
        {
            InitializeComponent();
        }
        public async void OnCreateNote(object sender, EventArgs e)
        {
            NoteModel credentials = new NoteModel();
           // credentials.NoteId = 2;
            credentials.NoteName = NoteNameEntry.Text;
            credentials.NoteText = NoteTextEntry.Text;

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
