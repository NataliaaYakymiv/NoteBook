using System;
using NoteBook.Servises;
using Xamarin.Forms;

namespace NoteBook.Pages
{
    public partial class ExternalLoginPage : ContentPage
    {
        public ExternalLoginPage(string provider)
        {
            InitializeComponent();
            var url = AccountService.GetService().Url + AccountService.GetService().ExternalLoginPath + "?provider=" +
                      provider;
            WebView.Source = new UrlWebViewSource()
            {
                Url = url
            };
        }

        private async void OnBack(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        public void OnExternalLogin()
        {
            WebView.Navigating += async (s, e) =>
            {

                if (
                    e.Url.StartsWith(AccountService.GetService().Url +
                                     AccountService.GetService().ExternalLoginFailurePath))
                {
                    e.Cancel = true;
                    StateLabel.Text = "Login Failure";
                    WebView.IsVisible = false;
                    BackBtn.IsVisible = true;

                }
                if (
                    e.Url.StartsWith(AccountService.GetService().Url +
                                     AccountService.GetService().ExternalLoginFinalPath))
                {
                    WebView.IsVisible = false;
                    var result = await AccountService.GetService().ExternalLogin(e.Url);
                    if (result.IsSuccessStatusCode)
                    {
                        await Navigation.PushAsync(new NotePage());
                    }
                    else
                    {
                        StateLabel.Text = await result.Content.ReadAsStringAsync();
                        StateLabel.Text = "Login Failure";
                        BackBtn.IsVisible = true;
                    }
                }
            };
        }
    }
}
