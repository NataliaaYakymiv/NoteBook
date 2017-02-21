using NoteBook.Models;
using System;
using System.Collections.Generic;
using NoteBook.Servises;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NoteBook.Pages
{
    public partial class NotePage : ContentPage
    {
        protected async override void OnAppearing()
        {
            Temp = await App.NotesItemManager.GetTasksAsync().ConfigureAwait(true);
            notesList.ItemsSource = Temp;
            base.OnAppearing();
        }

        public List<NoteModel> Notes
        {
            get;
            set;
        }

        public List<NoteModel> Temp
        {
            get;
            set;
        }

        public NotePage()
        {
            InitializeComponent();
            ForceLayout();
        }


        void NotesList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            UpdateButton.IsEnabled = true;

            //if (notesList.SelectedItem != null)
            //{
            //}
            //else
            //{
            //    notesList.
            //}
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
            NoteModel credentials = new NoteModel();
            var response = NotesService.GetService().Delete((NoteModel)notesList.SelectedItem);
            OnAppearing();
        }
    }
}
