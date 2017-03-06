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
        }

        public NotePage(IAccountService accountService, INotesService notesService) : this()
        {
            Title = "Your notes";
            SetService(notesService);
            SetAuthService(accountService);
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
                //Notes = NotesService.GetAllNotes().Result.ToList();
                Notes = NotesService.GetSyncNotes().Result.ToList();
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
            await Navigation.PushAsync(CreateNotePage, true);
        }

        private async void OnUpdate(object sender, EventArgs e)
        {
            UpdateNotePage.SetNoteModel((NoteModel)NotesList.SelectedItem);
            await Navigation.PushAsync(UpdateNotePage, true);
        }

        private async void OnDelete(object sender, EventArgs e)
        {
            await NotesService.DeleteNote((NoteModel)NotesList.SelectedItem);
            //NotesList.IsRefreshing = true;
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
                SetService(new RemoteNotesService(new NoteService(Settings.DatabaseName)));
                RemoteLocalSwitchLabel.Text = "REMOTE";
            }
            else
            {
                SetService(new LocalNotesService(Settings.DatabaseName));
                RemoteLocalSwitchLabel.Text = "LOCAL";
            }
            OnAppearing();
        }

        private async void OnLogout(object sender, EventArgs e)
        {
            SetService(new LocalNotesService(Settings.DatabaseName));
            await AccountService.Logout();
            Application.Current.MainPage = new NavigationPage(new LoginPage(AccountService, NotesService));
        }

        private void OnRefresh(object sender, EventArgs e)
        {
            Notes = NotesService.GetAllNotes().Result.ToList();
        }
    }
}
