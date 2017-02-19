using System;
using System.Net.Http;
using NoteBook.Models;
using NoteBook.Servises;
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
            }
        }

        private async void OnLinkedinLogin(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ExternalLoginPage("linkedin"));
        }

        private async void OnFacebookLogin(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ExternalLoginPage("facebook"));
        }

        private async void OnString(object sender, EventArgs e)
        {
            StateLabel1.Text = await AccountService.GetService().GetSTask().Result.Content.ReadAsStringAsync();
        }
    }
}
