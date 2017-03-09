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

        private bool _canLogout = false;
        private bool _settingsShowing = false;
        private bool _canDelete = false;

        private NoteModel _previouslySelectedItem = new NoteModel();



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
            _settingsShowing = true;
            OnSettings(this, EventArgs.Empty);
            if (NotesService != null)
            {
                Notes = NotesService.GetSyncNotes().Result.ToList();
                NotesList.ItemsSource = Notes;
                UpdateButton.IsEnabled = DeleteButton.IsEnabled = false;
            }
            base.OnAppearing();
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

        private void OnSettings(object sender, EventArgs e)
        {
            switch (_settingsShowing)
            {
                case false:
                    SettingsRow.Height = GridLength.Auto;
                    _settingsShowing = true;
                    break;
                case true:
                    SettingsRow.Height = 0;
                    _settingsShowing = false;
                    break;
                default:
                    SettingsRow.Height = 0;
                    _settingsShowing = false;
                    break;
            }
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

        private async void ShowLoguotDialog(object sender, EventArgs e)
        {
            var answer = await DisplayAlert("Logout", "Do you want to logout?", "Yes", "No");
            if (answer)
            {
                _canLogout = false;
                OnLogout(this, EventArgs.Empty);
            }
        }

        private async void ShowDeleteDialog(object sender, EventArgs e)
        {
            var answer = await DisplayAlert("Delete", "Do you want to delete this note?", "Yes", "No");
            if (answer)
            {
                _canDelete = false;
                OnDelete(this, EventArgs.Empty);
            }
        }

        private void NotesList_OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            switch (_previouslySelectedItem.Equals(e.Item))
            {
                case true:
                    _previouslySelectedItem = (NoteModel)NotesList.SelectedItem;
                    NotesList.SelectedItem = null;
                    UpdateButton.IsEnabled = DeleteButton.IsEnabled = false;
                    OnAppearing();
                    break;
                case false:
                    _previouslySelectedItem = (NoteModel)NotesList.SelectedItem;
                    UpdateButton.IsEnabled = DeleteButton.IsEnabled = true;
                    break;
            }
        }
    }
}
