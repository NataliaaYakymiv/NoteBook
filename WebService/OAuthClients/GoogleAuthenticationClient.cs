using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using DotNetOpenAuth.AspNet;
using DotNetOpenAuth.AspNet.Clients;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebService.OAuthClients
{
    //public class GoogleAuthenticationClient : OAuth2Client
    //{
    //    public string appId;
    //    public string appSecret;
    //    private string redirectUri;

    //    public GoogleAuthenticationClient(string appId, string appSecret)
    //    {
    //        this.appId = appId;
    //        this.appSecret = appSecret;
    //    }

    //    string IAuthenticationClient.ProviderName
    //    {
    //        get { return "googleplus"; }
    //    }

    //    void IAuthenticationClient.RequestAuthentication(HttpContextBase context, Uri returnUrl)
    //    {
    //        var APP_ID = this.appId;
    //        this.redirectUri = returnUrl.ToString();

    //        var address = String.Format(
    //                "https://accounts.google.com/o/oauth2/auth?client_id={0}&redirect_uri={1}&response_type=code&scope={2}",
    //                APP_ID, this.redirectUri, "https://www.googleapis.com/auth/plus.login"
    //            );

    //        HttpContext.Current.Response.Redirect(address, true);
    //    }

    //    class AccessToken
    //    {
    //        public string access_token = null;
    //        public string user_id = null;
    //    }

    //    class UserData
    //    {
    //        public string uid = null;
    //        public string first_name = null;
    //        public string last_name = null;
    //        public string photo_50 = null;
    //    }

    //    class UsersData
    //    {
    //        public UserData[] response = null;
    //    }

    //    AuthenticationResult IAuthenticationClient.VerifyAuthentication(HttpContextBase context)
    //    {
    //        try
    //        {
    //            string code = context.Request["code"];

    //            var address = String.Format(
    //                    "https://accounts.google.com/o/oauth2/token?client_id={0}&client_secret={1}&code={2}&redirect_uri={3}",
    //                    this.appId, this.appSecret, code, this.redirectUri);

    //            var response = GoogleAuthenticationClient.Load(address);
    //            var accessToken = GoogleAuthenticationClient.DeserializeJson<AccessToken>(response);

    //            address = String.Format(
    //                    "https://www.googleapis.com/plus/v1/people/{0}?access_token=1/fFBGRNJru1FQd44AzqT3Zg",
    //                    accessToken.user_id);

    //            response = GoogleAuthenticationClient.Load(address);
    //            var usersData = GoogleAuthenticationClient.DeserializeJson<UsersData>(response);
    //            var userData = usersData.response.First();

    //            return new AuthenticationResult(
    //                true, (this as IAuthenticationClient).ProviderName, accessToken.user_id,
    //                userData.first_name + " " + userData.last_name,
    //                new Dictionary<string, string>());
    //        }
    //        catch (Exception ex)
    //        {
    //            return new AuthenticationResult(ex);
    //        }
    //    }

    //    public static string Load(string address)
    //    {
    //        var request = WebRequest.Create(address) as HttpWebRequest;
    //        using (var response = request.GetResponse() as HttpWebResponse)
    //        {
    //            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
    //            {
    //                return reader.ReadToEnd();
    //            }
    //        }
    //    }

    //    public static T DeserializeJson<T>(string input)
    //    {
    //        var serializer = new JavaScriptSerializer();
    //        return serializer.Deserialize<T>(input);
    //    }
    //}

    public class GoogleOAuth2Client : OAuth2Client
    {
        #region Constants and Fields

        /// <summary>
        /// The authorization endpoint.
        /// </summary>
        private const string AuthorizationEndpoint = "https://accounts.google.com/o/oauth2/auth";

        /// <summary>
        /// The token endpoint.
        /// </summary>
        private const string TokenEndpoint = "https://accounts.google.com/o/oauth2/token";

        /// <summary>
        /// The user info endpoint.
        /// </summary>
        private const string UserInfoEndpoint = "https://www.googleapis.com/oauth2/v1/userinfo";

        /// <summary>
        /// The base uri for scopes.
        /// </summary>
        private const string ScopeBaseUri = "https://www.googleapis.com/auth/";

        /// <summary>
        /// The _app id.
        /// </summary>
        private readonly string _clientId;

        /// <summary>
        /// The _app secret.
        /// </summary>
        private readonly string _clientSecret;

        /// <summary>
        /// The requested scopes.
        /// </summary>
        private readonly string[] _requestedScopes;

        #endregion

        /// <summary>
        /// Creates a new Google OAuth2 Client, requesting the default "userinfo.profile" and "userinfo.email" scopes.
        /// </summary>
        /// <param name="clientId">The Google Client Id</param>
        /// <param name="clientSecret">The Google Client Secret</param>
        public GoogleOAuth2Client(string clientId, string clientSecret)
            : this(clientId, clientSecret, new[] { "userinfo.profile", "userinfo.email" }) { }

        /// <summary>
        /// Creates a new Google OAuth2 client.
        /// </summary>
        /// <param name="clientId">The Google Client Id</param>
        /// <param name="clientSecret">The Google Client Secret</param>
        /// <param name="requestedScopes">One or more requested scopes, passed without the base URI.</param>
        public GoogleOAuth2Client(string clientId, string clientSecret, params string[] requestedScopes)
            : base("google")
        {
            if (string.IsNullOrWhiteSpace(clientId))
                throw new ArgumentNullException("clientId");

            if (string.IsNullOrWhiteSpace(clientSecret))
                throw new ArgumentNullException("clientSecret");

            if (requestedScopes == null)
                throw new ArgumentNullException("requestedScopes");

            if (requestedScopes.Length == 0)
                throw new ArgumentException("One or more scopes must be requested.", "requestedScopes");

            _clientId = clientId;
            _clientSecret = clientSecret;
            _requestedScopes = requestedScopes;
        }

        protected override Uri GetServiceLoginUrl(Uri returnUrl)
        {
            var scopes = _requestedScopes.Select(x => !x.StartsWith("http", StringComparison.OrdinalIgnoreCase) ? ScopeBaseUri + x : x);
            var state = string.IsNullOrEmpty(returnUrl.Query) ? string.Empty : returnUrl.Query.Substring(1);

            return BuildUri(AuthorizationEndpoint, new NameValueCollection
                {
                    { "response_type", "code" },
                    { "client_id", _clientId },
                    { "scope", string.Join(" ", scopes) },
                    { "redirect_uri", returnUrl.GetLeftPart(UriPartial.Path) },
                    { "state", state },
                });
        }

        protected override IDictionary<string, string> GetUserData(string accessToken)
        {
            var uri = BuildUri(UserInfoEndpoint, new NameValueCollection { { "access_token", accessToken } });

            var webRequest = (HttpWebRequest)WebRequest.Create(uri);

            using (var webResponse = webRequest.GetResponse())
            using (var stream = webResponse.GetResponseStream())
            {
                if (stream == null)
                    return null;

                using (var textReader = new StreamReader(stream))
                {
                    var json = textReader.ReadToEnd();
                    var extraData = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                    return extraData;
                }
            }
        }

        protected override string QueryAccessToken(Uri returnUrl, string authorizationCode)
        {
            var postData = HttpUtility.ParseQueryString(string.Empty);
            postData.Add(new NameValueCollection
                {
                    { "grant_type", "authorization_code" },
                    { "code", authorizationCode },
                    { "client_id", _clientId },
                    { "client_secret", _clientSecret },
                    { "redirect_uri", returnUrl.GetLeftPart(UriPartial.Path) },
                });

            var webRequest = (HttpWebRequest)WebRequest.Create(TokenEndpoint);

            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";

            using (var s = webRequest.GetRequestStream())
            using (var sw = new StreamWriter(s))
                sw.Write(postData.ToString());

            using (var webResponse = webRequest.GetResponse())
            {
                var responseStream = webResponse.GetResponseStream();
                if (responseStream == null)
                    return null;

                using (var reader = new StreamReader(responseStream))
                {
                    var response = reader.ReadToEnd();
                    var json = JObject.Parse(response);
                    var accessToken = json.Value<string>("access_token");
                    return accessToken;
                }
            }
        }

        private static Uri BuildUri(string baseUri, NameValueCollection queryParameters)
        {
            var keyValuePairs = queryParameters.AllKeys.Select(k => HttpUtility.UrlEncode(k) + "=" + HttpUtility.UrlEncode(queryParameters[k]));
            var qs = String.Join("&", keyValuePairs);

            var builder = new UriBuilder(baseUri) { Query = qs };
            return builder.Uri;
        }

        /// <summary>
        /// Google requires that all return data be packed into a "state" parameter.
        /// This should be called before verifying the request, so that the url is rewritten to support this.
        /// </summary>
        public static void RewriteRequest()
        {
            var ctx = HttpContext.Current;

            var stateString = HttpUtility.UrlDecode(ctx.Request.QueryString["state"]);
            if (stateString == null || !stateString.Contains("__provider__=google"))
                return;

            var q = HttpUtility.ParseQueryString(stateString);
            q.Add(ctx.Request.QueryString);
            q.Remove("state");

            ctx.RewritePath(ctx.Request.Path + "?" + q);
        }
    }
}