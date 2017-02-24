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
            NoteNameEntry.Text = item.NoteName;
            NoteTextEntry.Text = item.NoteText;
            InitializeComponent();
        }

        public async void OnCreateNote(object sender, EventArgs e)
        {
            NoteModel note = new NoteModel();
            note.NoteName = NoteNameEntry.Text;
            note.NoteText = NoteTextEntry.Text;

            var result = App.NotesItemManager.NoteService.CreateNote(note);

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
    }
}
