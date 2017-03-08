using System;
using Xamarin.Forms;
using NoteBook.Contracts;
using NoteBook.Helpers;
using NoteBook.Pages;
using NoteBook.Services;
using NoteBook.Settings;

namespace NoteBook
{
    public partial class App : Application
    {

        private static INotesService _database;
        public static INotesService Database => _database ?? (_database = new NoteService(Settings.Settings.DatabaseName));

        public App()
        {
            InitializeComponent();
            //AuthHelper.ClearAll();
            //NotesHelper.ClearLocal(new NoteService(Settings.DatabaseName));
            if (string.IsNullOrEmpty(UserSettings.SyncDate))
            {
                UserSettings.SyncDate = DateTime.MinValue.ToString("G");
            }

            var accountService = new AccountService();
            var isLoggedIn = AuthHelper.IsLoged();

            if (isLoggedIn)
            {
                MainPage = new NavigationPage(new NotePage(accountService, new LocalNotesService(Settings.Settings.DatabaseName)));
            }
            else
            {
                MainPage = new NavigationPage(new LoginPage(accountService, new LocalNotesService(Settings.Settings.DatabaseName)));
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
