using NoteBook.Models;
using System;
using System.Collections.Generic;
using NoteBook.Servises;
using Xamarin.Forms;

namespace NoteBook.Pages
{
    public partial class NotePage : ContentPage
    {
        protected async override void OnAppearing()
        {
            Temp = await App.NotesItemManager.GetTasksAsync().ConfigureAwait(true);
            //notesList.ItemsSource = await NoteBook.NotesItemManager.GetTasksAsync().ConfigureAwait(false);
            notesList.ItemsSource = Temp;
            //base.OnAppearing();

            //if (Constants.RestUrl.Contains("developer.xamarin.com"))
            //{
            //    if (!alertShown)
            //    {
            //        await DisplayAlert(
            //            "Hosted Back-End",
            //            "This app is running against Xamarin's read-only REST service. To create, edit, and delete data you must update the service endpoint to point to your own hosted REST service.",
            //            "OK");
            //        alertShown = true;
            //    }
            //}

            //notesList.ItemsSource = Notes;
            //notesList.ItemsSource = await App.NotesItemManager.GetTasksAsync().ConfigureAwait(false);
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

        public async void Fill()
        {
            //Notes = new List<NoteModel>();
            //for (int i = 0; i < 100; i++)
            //{
            //    Notes.Add(new NoteModel { NoteName = "first", NoteText = "firstNote" });
            //}

            //notesList.ItemsSource = Notes;
            //
            //
            //
            //
        }


        public NotePage()
        {
            InitializeComponent();
            //Fill();


            // //   new NoteModel { NoteId = "s", NoteName = "first", NoteText = "firstNote" },
            //   // new NoteModel { NoteId = "s", NoteName = "first", NoteText = "firstNote" },
            //    //new NoteModel { NoteId = "s", NoteName = "first", NoteText = "firstNote" },
            //    //new NoteModel { NoteId = "s", NoteName = "first", NoteText = "firstNote" },
            //};

            // Notes.Add(new NoteModel { NoteName = "first", NoteText = "firstNote" });
            //notesList.ItemsSource = Notes;
            //notesList.ItemsSource = await App.NotesItemManager.GetTasksAsync();

        }
        void NotesList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            selected.Text = ((NoteModel)e.SelectedItem).NoteName;
        }

        //private async void OnCreate(object sender, EventArgs e)
        //{
        //    await Navigation.PushAsync(new CreateNotePage());
        //}
        //private async void OnUpdate(object sender, EventArgs e)
        //{
        //    await Navigation.PushAsync(new UpdateNotePage());
        //}
        //private async void OnDelete(object sender, EventArgs e)
        //{
        //    throw new Exception();
        //}

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
            NoteModel credentials = new NoteModel();

            //credentials.NoteId = "b94afb54-a1cb-4313-8af3-b7511551b33b";
            //credentials.NoteName = "name";//NoteNameEntry.Text;
            //credentials.NoteText = "text";//NoteTextEntry.Text;

            var response = NotesService.GetService().Delete((NoteModel)notesList.SelectedItem);
            StateLabel.Text = await response.Content.ReadAsStringAsync();
        }

        private async void OnRefresh(object sender, EventArgs e)
        {
            notesList.ItemsSource = await App.NotesItemManager.GetTasksAsync().ConfigureAwait(false);
        }

    }
}
