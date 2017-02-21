using System;
using Xamarin.Forms;

namespace NoteBook.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            Title = "Welcome!";
            var logo = new Image { Aspect = Aspect.AspectFit };
            logo.Source = "logo.jpg";
            
            
            InitializeComponent();
        }

        private async void OnRegister(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RegisterPage());
        }

        private async void OnLogin(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new LoginPage());
        }
    }
}
