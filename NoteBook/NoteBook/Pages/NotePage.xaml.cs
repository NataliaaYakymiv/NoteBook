using NoteBook.Models;
using System;
using System.Collections.Generic;
using NoteBook.Servises;
using Xamarin.Forms;

namespace NoteBook.Pages
{
    public partial class NotePage : ContentPage
    {
        protected override async void OnAppearing()
        {
            Notes = await App.NotesItemManager.GetTasksAsync().ConfigureAwait(true);
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

        private async void OnDelete(object sender, EventArgs e)
        {
            var response = NotesService.GetService().Delete((NoteModel)notesList.SelectedItem);
            OnAppearing();
        }
    }
}
