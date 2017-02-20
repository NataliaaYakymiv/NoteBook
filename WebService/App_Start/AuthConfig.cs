using Microsoft.Web.WebPages.OAuth;
//using SocialAuthorization.Models;
namespace WebService
{
    public static class AuthConfig
    {
        public static void RegisterAuth()
        {
            // To let users of this site log in using their accounts from other sites such as Microsoft, Facebook, and Twitter,
            // you must update this site. For more information visit http://go.microsoft.com/fwlink/?LinkID=252166

            //OAuthWebSecurity.RegisterMicrosoftClient(
            //    clientId: "",
            //    clientSecret: "");

            //OAuthWebSecurity.RegisterTwitterClient(
            //    consumerKey: "",
            //    consumerSecret: "");
           // OAuthWebSecurity.
            OAuthWebSecurity.RegisterFacebookClient(
                appId: "710642582432970",
                appSecret: "f8676402fa85d8cfdaceb7ab6af86f6e");
            OAuthWebSecurity.RegisterLinkedInClient(
                consumerKey: "86t482hcaas1vl",
                consumerSecret: "FoaMtH2nj1MQFsim"
                );
            //OAuthWebSecurity.RegisterGoogleClient();
        }
    }
}