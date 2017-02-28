using System;
using Xamarin.Forms;
using NoteBook.Contracts;
using NoteBook.Pages;
using NoteBook.Services;

namespace NoteBook
{
    public partial class App : Application
    {
        public static NotesItemManager NotesItemManager { get; private set; }

        public static INotesService database;
        public static INotesService Database

        {
            get
            {
                if (database == null)
                {
                    database = new NoteService(Settings.DatabaseName);
                }
                return database;
            }
        }

        public App()
        {
            InitializeComponent();

            if (string.IsNullOrEmpty(UserSettings.SyncDate))
            {
                UserSettings.SyncDate = DateTime.MinValue.ToString();
            }

            var accountService = new AccountService();
            var noteService = new NoteService(Settings.DatabaseName);
            var remoteService = new RemoteNotesService(accountService, noteService);

            NotesItemManager = new NotesItemManager(remoteService, noteService);

            var isLoggedIn = Services.AccountService.IsLoged();

            if (isLoggedIn)
            {
                MainPage = new NavigationPage(new NotePage(accountService, new LocalNotesService(Settings.DatabaseName)));
            }
            else
            {
                MainPage = new NavigationPage(new LoginPage(accountService, new LocalNotesService(Settings.DatabaseName)));
            }
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
