﻿using System;
using NoteBook.Contracts;
using NoteBook.Helpers;
using NoteBook.Services;
using NoteBook.Settings;
using Xamarin.Forms;

namespace NoteBook.Pages
{
    public partial class ExternalLoginPage : ContentPage
    {
        public IAccountService AccountService { get; private set; }
        public INotesService NotesService { get; private set; }

        public ExternalLoginPage(string provider, IAccountService accountService, INotesService notesService)
        {
            AccountService = accountService;
            NotesService = notesService;

            InitializeComponent();
            var url = Settings.Settings.Url + Settings.Settings.ExternalLoginPath + "?provider=" +
                      provider;
            WebView.Source = new UrlWebViewSource()
            {
                Url = url
            };
        }

        private async void OnBack(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        public void OnExternalLogin()
        {
            WebView.Navigating += async (s, e) =>
            {

                if (
                    e.Url.StartsWith(Settings.Settings.Url + Settings.Settings.ExternalLoginFailurePath))
                {
                    e.Cancel = true;
                    StateLabel.Text = "Login Failure";
                    WebView.IsVisible = false;
                    BackBtn.IsVisible = true;

                }
                if (
                    e.Url.StartsWith(Settings.Settings.Url + Settings.Settings.ExternalLoginFinalPath))
                {
                    WebView.IsVisible = false;
                    var result = await AccountService.ExternalLogin(e.Url);
                    if (result)
                    {
                        NotesHelper.ClearLocal(new NoteService(Settings.Settings.DatabaseName));
                        UserSettings.SyncDate = DateTime.MinValue.ToString("G");
                        var page = new NotePage(AccountService, NotesService);
                        Application.Current.MainPage = new NavigationPage(page);
                    }
                    else
                    {
                        StateLabel.Text = "Login Failure";
                        BackBtn.IsVisible = true;
                    }
                }
            };
        }
    }
}
