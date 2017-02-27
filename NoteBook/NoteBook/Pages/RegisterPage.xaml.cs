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

                    #region diasblingViews
                    LoginEntry.IsEnabled = false;
                    PasswordEntry.IsEnabled = false;
                    RePasswordEntry.IsEnabled = false;
                    RegisterButton.IsVisible = false;
                    ActivityIndicatorRegister.IsRunning = true;
                    ActivityIndicatorRegister.IsVisible = true;
                    #endregion

                    var result = await AccountService.Register(credentials);
                    
                    #region enablingViews
                    ActivityIndicatorRegister.IsRunning = false;
                    ActivityIndicatorRegister.IsVisible = false;
                    RegisterButton.IsVisible = true;
                    RePasswordEntry.IsEnabled = true;
                    PasswordEntry.IsEnabled = true;
                    LoginEntry.IsEnabled = true;
                    #endregion


                    if (!result)
                    {
                        PasswordEntry.Text = string.Empty;
                        StatusLabel.Text = "User name already exist";
                    }
                    else
                    {
                        var page = new LoginPage(credentials.UserName, credentials.Password, "You successfully registered. Now sing in", AccountService, new LocalNotesService(Settings.DatabaseName));

                        await Navigation.PushAsync(page);

                    }
                }
                else
                {
                    StatusLabel.Text = "Repeat password entered is not correct";
                }
            }
            else
            {
                StatusLabel.Text = "Fill in all fields";
            }

        }
    }
}
