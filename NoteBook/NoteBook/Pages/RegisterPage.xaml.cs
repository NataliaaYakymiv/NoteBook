using System;
using NoteBook.Models;
using NoteBook.Servises;
using Xamarin.Forms;

namespace NoteBook.Pages
{
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage()
        {
            Title = "Registration Page";
            InitializeComponent();
        }

        private async void OnRegister(object sender, EventArgs e)
        {
            AccountModels.RegisterModel credentials = new AccountModels.RegisterModel();
            credentials.UserName = LoginEntry.Text;
            credentials.Password = PasswordEntry.Text;

            var response = AccountService.GetService().Register(credentials);

            LoginEntry.Text = string.Empty;
            StateLabel.Text = await response.Result.Content.ReadAsStringAsync();

            if (response.Result.IsSuccessStatusCode)
            {
                PasswordEntry.Text = string.Empty;
            }
            
        }
    }
}
