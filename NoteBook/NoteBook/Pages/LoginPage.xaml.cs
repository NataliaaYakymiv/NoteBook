using System;
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
                await Navigation.PushAsync(new NotePage());
            }
        }

        private async void OnGetNumber(object sender, EventArgs e)
        {
            StateLabel1.Text = await AccountService.GetService().GetInt().Result.Content.ReadAsStringAsync();
        }

        private void OnFacebookLogin(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
