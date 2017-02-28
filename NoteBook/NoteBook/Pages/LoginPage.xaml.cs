using System;
using NoteBook.Contracts;
using NoteBook.Models;
using NoteBook.Services;
using Xamarin.Forms;

namespace NoteBook.Pages
{
    public partial class LoginPage : ContentPage
    {
        public IAccountService AccountService { get; private set; }
        public INotesService NotesService { get; private set; }

        private LoginPage()
        {
            Title = "Sing in";
            InitializeComponent();
        }

        public LoginPage(IAccountService accountService, INotesService notesService)
        {
            InitializeComponent();

            AccountService = accountService;
            NotesService = notesService;

            if (AccountService.IsLoged())
            {
                var page = new NotePage();
                Navigation.PushAsync(page);
                page.SetService(NotesService);
                page.SetAuthService(new AccountService());
            }
        }

        public LoginPage(string login, string password, string state, IAccountService accountService, INotesService notesService) : this(accountService, notesService)
        {

            LoginEntry.Text = login;
            PasswordEntry.Text = password;
            StateLabel.Text = state;
        }

        private async void OnLogin(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(LoginEntry.Text) && !string.IsNullOrEmpty(PasswordEntry.Text))
            {
                var credentials = new AccountModels.LoginModel()
                {
                    UserName = LoginEntry.Text,
                    Password = PasswordEntry.Text,
                    RememberMe = !RememberMe.IsToggled
                };

                LoginBtn.IsEnabled = false;
                ActivityIndicatorLogin.IsRunning = true;
                ActivityIndicatorLogin.IsVisible = true;

                var response = await AccountService.Login(credentials);

                ActivityIndicatorLogin.IsRunning = false;
                ActivityIndicatorLogin.IsVisible = false;
                LoginBtn.IsEnabled = true;

                if (!response)
                {
                    StateLabel.Text = "User name or password is not valid";
                    PasswordEntry.Text = string.Empty;
                }
                else
                {
                    LoginBtn.IsEnabled = false;
                    ActivityIndicatorLogin.IsRunning = true;
                    ActivityIndicatorLogin.IsVisible = true;

                    App.NotesItemManager.ClearLocal();

                    ActivityIndicatorLogin.IsRunning = false;
                    ActivityIndicatorLogin.IsVisible = false;
                    LoginBtn.IsEnabled = true;

                    UserSettings.SyncDate = DateTime.MinValue.ToString();

                    var page = new NotePage();
                    await Navigation.PushAsync(page);
                    page.SetService(NotesService);
                    page.SetAuthService(new AccountService());
                }
            }
            else
            {
                StateLabel.Text = "Fill in all fields";
            }

        }

        private async void OnGoogleLogin(object sender, EventArgs e)
        {
            var page = new ExternalLoginPage("google", AccountService, NotesService);
            await Navigation.PushAsync(page);
            page.OnExternalLogin();
        }

        private async void OnFacebookLogin(object sender, EventArgs e)
        {
            var page = new ExternalLoginPage("facebook", AccountService, NotesService);
            await Navigation.PushAsync(page);
            page.OnExternalLogin();
        }

        private async void OnLinkedinLogin(object sender, EventArgs e)
        {
            var page = new ExternalLoginPage("linkedin", AccountService, NotesService);
            await Navigation.PushAsync(page);
            page.OnExternalLogin();
        }


        private async void OnRegister(object sender, EventArgs e)
        {
            var page = new RegisterPage();
            await Navigation.PushAsync(page);
            page.SetService(AccountService);
        }
    }
}
