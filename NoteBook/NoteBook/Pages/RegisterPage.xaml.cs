using System;
using NoteBook.Contracts;
using NoteBook.Models;
using NoteBook.Services;
using Xamarin.Forms;

namespace NoteBook.Pages
{
    public partial class RegisterPage : ContentPage
    {
        public IAccountService AccountService { get; private set; }

        public RegisterPage()
        {
            Title = "Registration Page";
            InitializeComponent();
        }

        public void SetService(IAccountService accountService)
        {
            AccountService = accountService;
        }

        private async void OnRegister(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(LoginEntry.Text) && !string.IsNullOrEmpty(PasswordEntry.Text) && !string.IsNullOrEmpty(RePasswordEntry.Text))
            {
                if (PasswordEntry.Text.Equals(RePasswordEntry.Text))
                {
                    var credentials = new AccountModels.RegisterModel
                    {
                        UserName = LoginEntry.Text,
                        Password = PasswordEntry.Text
                    };


                    ActivityIndicatorRegister.IsRunning = true;
                    ActivityIndicatorRegister.IsVisible = true;

                    var result = await AccountService.Register(credentials);

                    ActivityIndicatorRegister.IsRunning = false;
                    ActivityIndicatorRegister.IsVisible = false;

                    if (!result)
                    {
                        PasswordEntry.Text = string.Empty;
                        StateLabel.Text = "User name already exist";
                    }
                    else
                    {
                        var page = new LoginPage(credentials.UserName, credentials.Password, "You successfully registered. Now sing in", AccountService, new LocalNotesService(Settings.DatabaseName));

                        await Navigation.PushAsync(page);
                        
                    }
                }
                else
                {
                    StateLabel.Text = "Repeat password entered is not correct";
                }
            }
            else
            {
                StateLabel.Text = "Fill in all fields";
            }
            
        }
    }
}
