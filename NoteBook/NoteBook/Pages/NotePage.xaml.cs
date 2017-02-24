using NoteBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using NoteBook.Services;
using Xamarin.Forms;

namespace NoteBook.Pages
{
    public partial class NotePage : ContentPage
    {
        protected override void OnAppearing()
        {
            Notes = App.NotesItemManager.NoteService.GetAllNotes().ToList();
            notesList.ItemsSource = Notes;

            UpdateButton.IsEnabled = DeleteButton.IsEnabled = false;

            base.OnAppearing();
        }

        public List<NoteModel> Notes
        {
            get;
            set;
        }

        public NotePage()
        {
            InitializeComponent();
            App.NotesItemManager.time = DateTime.Now;

        }


        private void NotesList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            UpdateButton.IsEnabled = DeleteButton.IsEnabled = true;
        }

        private async void OnCreate(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CreateNotePage());
        }

        private async void OnUpdate(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new UpdateNotePage((NoteModel)notesList.SelectedItem));
        }

        private void OnDelete(object sender, EventArgs e)
        {
            App.NotesItemManager.NoteService.DeleteNote((NoteModel)notesList.SelectedItem);
            OnAppearing();
        }

        private void OnSync(object sender, EventArgs e)
        {
            App.NotesItemManager.Sync();
            OnAppearing();
        }
    }
}
