using NoteBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace NoteBook.Pages
{
    public partial class NotePage : ContentPage
    {
        public List<NoteModel> Notes
        {
            get;
            set;
        }
        public NotePage()
        {
            InitializeComponent();
            Notes = new List<NoteModel>();
            //{
            //    new NoteModel {NoteName = "first", NoteText = "firstNote"},
            //    new NoteModel {NoteName = "first", NoteText = "firstNote"},
            //    new NoteModel {NoteName = "first", NoteText = "firstNote"},
            //    new NoteModel {NoteName = "first", NoteText = "firstNote"}
            //};
            for (int i = 0; i < 100; i++)
            {
                Notes.Add(new NoteModel { NoteName = "first", NoteText = "firstNote" });
            }
            // Notes.Add(new NoteModel { NoteName = "first", NoteText = "firstNote" });
            notesList.ItemsSource = Notes;
        }
        void NotesList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            selected.Text = ((NoteModel)e.SelectedItem).NoteName;
        }

        private async void OnCreate(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CreateNotePage());
        }
        private async void OnUpdate(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new UpdateNotePage());
        }
        private async void OnDelete(object sender, EventArgs e)
        {
            throw new Exception();
        }

    }
}
