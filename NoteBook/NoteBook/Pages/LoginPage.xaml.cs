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
            InitializeComponent();
        }

        public LoginPage(IAccountService accountService, INotesService notesService) : this()
        {
            AccountService = accountService;
            NotesService = notesService;
        }

        public LoginPage(string login, string password, string status, IAccountService accountService, INotesService notesService) : this(accountService, notesService)
        {

            LoginEntry.Text = login;
            PasswordEntry.Text = password;
            StatusLabel.Text = status;
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

                #region diasblingViews
                PasswordEntry.IsEnabled = false;
                LoginEntry.IsEnabled = false;
                LoginBtn.IsEnabled = false;
                RegisterButton.IsEnabled = false;
                FacebookButton.IsEnabled = false;
                GoogleButton.IsEnabled = false;
                LinkedInButton.IsEnabled = false;
                RememberMe.IsVisible = false;
                RememberMeLabel.IsVisible = false;
                ActivityIndicatorLogin.IsRunning = true;
                ActivityIndicatorLogin.IsVisible = true;
                #endregion

                var response = await AccountService.Login(credentials);

                #region enablingViews
                ActivityIndicatorLogin.IsRunning = false;
                ActivityIndicatorLogin.IsVisible = false;
                FacebookButton.IsEnabled = true;
                GoogleButton.IsEnabled = true;
                LinkedInButton.IsEnabled = true;
                RegisterButton.IsEnabled = true;
                RememberMe.IsVisible = true;
                RememberMeLabel.IsVisible = true;
                LoginBtn.IsEnabled = true;
                PasswordEntry.IsEnabled = true;
                LoginEntry.IsEnabled = true;
                #endregion

                if (!response)
                {
                    StatusLabel.Text = "User name or password is not valid";
                    PasswordEntry.Text = string.Empty;
                }
                else
                {
                    Application.Current.MainPage = new NavigationPage(new NotePage(AccountService, NotesService));

                    //OnDisappearing();
                    //var page = new NotePage();
                    //await Navigation.PushAsync(page);
                    //page.SetService(NotesService);
                    //page.SetAuthService(new AccountService());
                }
            }
            else
            {
                StatusLabel.Text = "Fill in all fields";
            }

        }

        //protected override void OnDisappearing()
        //{
        //    Navigation.RemovePage(this);
        //}

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
