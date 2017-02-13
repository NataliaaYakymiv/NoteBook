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
            FormCredentials credentials = new FormCredentials();
            credentials.Name = LoginEntry.Text;
            credentials.Password = PasswordEntry.Text;

            await AccountService.GetService().Register(credentials);
        }
    }
}
