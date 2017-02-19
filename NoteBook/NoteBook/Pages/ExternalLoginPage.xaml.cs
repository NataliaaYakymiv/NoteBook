using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
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
           
            WebView.Navigating += (s, e) =>
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
            };
            WebView.Navigated += (s, e) =>
            {
                if (
                    e.Url.StartsWith(AccountService.GetService().Url +
                                     AccountService.GetService().ExternalLoginConfirmationPath))
                {

                    StateLabel.Text = "ExternalLoginConfirmationPath";
                }
            };
        }

        private async void OnBack(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
