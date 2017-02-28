using NoteBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using NoteBook.Contracts;
using NoteBook.Services;
using Xamarin.Forms;

namespace NoteBook.Pages
{
    public partial class NotePage : ContentPage
    {
        public INotesService NotesService { get; set; }
        public IAccountService AccountService { get; set; }

        public CreateNotePage CreateNotePage { get; private set; }
        public UpdateNotePage UpdateNotePage { get; private set; }

        public List<NoteModel> Notes { get; set; }


        public NotePage()
        {
            InitializeComponent();
            Title = "Note page";
        }

        public void SetService(INotesService notesService)
        {
            NotesService = notesService;

            CreateNotePage = new CreateNotePage(NotesService);
            UpdateNotePage = new UpdateNotePage(NotesService);

            OnAppearing();
        }

        public void SetAuthService(IAccountService accountService)
        {
            AccountService = accountService;
        }

        protected sealed override void OnAppearing()
        {
            if (NotesService != null)
            {
               // Notes = NotesService.GetAllNotes().Result.ToList();
                Notes = NotesService.GetSyncNotes(Convert.ToDateTime(UserSettings.SyncDate)).Result.ToList();
                NotesList.ItemsSource = Notes;

                UpdateButton.IsEnabled = DeleteButton.IsEnabled = false;
            }
            base.OnAppearing();
        }

        private void NotesList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {

            UpdateButton.IsEnabled = DeleteButton.IsEnabled = true;
        }

        private async void OnCreate(object sender, EventArgs e)
        {
            await Navigation.PushAsync(CreateNotePage);
        }

        private async void OnUpdate(object sender, EventArgs e)
        {
            UpdateNotePage.SetNoteModel((NoteModel)NotesList.SelectedItem);
            await Navigation.PushAsync(UpdateNotePage);
        }

        private async void OnDelete(object sender, EventArgs e)
        {
            await NotesService.DeleteNote((NoteModel)NotesList.SelectedItem);
            OnAppearing();
        }

        private void OnSync(object sender, EventArgs e)
        {
            OnAppearing();
        }

        private void Ontoggled(object sender, ToggledEventArgs e)
        {
            if (e.Value)
            {
                SetService(new RemoteNotesService(new AccountService(), new NoteService(Settings.DatabaseName)));
            }
            else
            {
                SetService(new LocalNotesService(Settings.DatabaseName));
            }
            
        }

        private async void OnLogout(object sender, EventArgs e)
        {
            await AccountService.Logout();
            await Navigation.PopToRootAsync();
        }

        private async void OnRefresh(object sender, EventArgs e)
        {
            Notes = (await NotesService.GetAllNotes()).ToList();
            base.OnAppearing();
        }
    }
}
