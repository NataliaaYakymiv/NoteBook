using System;
using System.Net.Http;
using NoteBook.Models;
using NoteBook.Services;
using Xamarin.Forms;

namespace NoteBook.Pages
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            Title = "Login Page";
            InitializeComponent();
        }

        private async void OnLogin(object sender, EventArgs e)
        {
            AccountModels.LoginModel credentials = new AccountModels.LoginModel();
            credentials.UserName = LoginEntry.Text;
            credentials.Password = PasswordEntry.Text;

            var response = AccountService.GetService().Login(credentials);
            LoginEntry.Text = string.Empty;
            StateLabel.Text = await response.Result.Content.ReadAsStringAsync();

            if (response.Result.IsSuccessStatusCode)
            {
                PasswordEntry.Text = string.Empty;
                await Navigation.PushAsync(new NotePage());
            }
        }

        private async void OnLinkedinLogin(object sender, EventArgs e)
        {
            var page = new ExternalLoginPage("linkedin");
            await Navigation.PushAsync(page);
            page.OnExternalLogin();
        }

        private async void OnFacebookLogin(object sender, EventArgs e)
        {
            var page = new ExternalLoginPage("facebook");
            await Navigation.PushAsync(page);
            page.OnExternalLogin();
        }

        private async void OnGoogleLogin(object sender, EventArgs e)
        {
            var page = new ExternalLoginPage("google");
            await Navigation.PushAsync(page);
            page.OnExternalLogin();
        }

        private async void OnVkLogin(object sender, EventArgs e)
        {
            var page = new ExternalLoginPage("vkontakte");
            await Navigation.PushAsync(page);
            page.OnExternalLogin();
        }
    }
}
