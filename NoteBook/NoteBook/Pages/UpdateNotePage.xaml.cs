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
            note.NoteId = NoteIdEntry.Text;
            note.NoteName = NoteNameEntry.Text;
            note.NoteText = NoteTextEntry.Text;

            var result = App.NotesItemManager.NoteService.UpdateNote(note);

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
