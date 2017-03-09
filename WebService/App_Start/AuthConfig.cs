using Microsoft.Web.WebPages.OAuth;
using WebService.OAuthClients;
namespace WebService
{
    public static class AuthConfig
    {
        public static void RegisterAuth()
        {
            OAuthWebSecurity.RegisterFacebookClient(
                appId: "710642582432970",
                appSecret: "f8676402fa85d8cfdaceb7ab6af86f6e");
            OAuthWebSecurity.RegisterLinkedInClient(
                consumerKey: "86t482hcaas1vl",
                consumerSecret: "FoaMtH2nj1MQFsim"
                );
            OAuthWebSecurity.RegisterClient(
                client: new GoogleOAuth2Client(
                    "1029764351833-n4mqi2u02v1439mb2f8v0df19qkl91ag.apps.googleusercontent.com", "StjvjB0OIiC6cVgk6VGZDatd"),
                displayName: "google",
                extraData: null);
        }
    }
}