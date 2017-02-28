using System;
using System.Web.Http;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using WebService.Contracts;
using WebService.Models;
using WebService.OAuthClients;
using WebService.Repositories;

namespace WebService.Controllers
{
    [Authorize]
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {

        //public string Url { get; } = "http://5ed5859d.ngrok.io/";
        public static string Url { get; } = "http://192.168.88.116:81/";

        static readonly IAccountRepository AccountRepository = new AccountRepository();
        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        public IHttpActionResult Login(AccountModels.LoginModel model)
        {
            IHttpActionResult result;

            var isLoged = false;

            try
            {
                isLoged = WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe);
            }
            catch (Exception)
            {
                return BadRequest();
            }

            if (isLoged)
            {
                result = Ok();
            }
            else
            {
                result = BadRequest();
            }

            return result;
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        public IHttpActionResult Register([FromBody] AccountModels.RegisterModel model)
        {
            IHttpActionResult result;
            try
            {
                WebSecurity.CreateUserAndAccount(model.UserName, model.Password);
                result = Ok();
            }
            catch (MembershipCreateUserException e)
            {
                var error = ErrorCodeToString(e.StatusCode);
                result = BadRequest(error);
            }

            return result;
        }


        //
        // GET: /Account/ExternalLogin
        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult ExternalLogin([FromUri]string provider)
        {
            OAuthWebSecurity.RequestAuthentication(provider, Url + "api/account/ExternalLoginCallback");

            return Ok();
        }

        //
        // GET: /Account/ExternalLoginCallback
        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult ExternalLoginCallback()
        {
            GoogleOAuth2Client.RewriteRequest();
            IHttpActionResult actionResult;

            AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication(Url + "api/account/ExternalLoginCallback");
            if (!result.IsSuccessful)
            {
                actionResult = Redirect(Url  + "api/account/ExternalLoginFailure");
            }

            else if (OAuthWebSecurity.Login(result.Provider, result.ProviderUserId, createPersistentCookie: false))
            {
                actionResult =
                    Redirect(Url + "api/account/ExternalLoginFinal?provider=" + result.Provider + "&providerUserId=" +
                             result.ProviderUserId);
            }
            else
            {
                string loginData = OAuthWebSecurity.SerializeProviderUserId(result.Provider, result.ProviderUserId);
                var model = new AccountModels.RegisterExternalLoginModel
                {
                    UserName = result.UserName,
                    ExternalLoginData = loginData
                };

                switch (result.Provider)
                {
                    case "facebook":
                    case "google":
                    {
                        model.Email = result.UserName;
                        model.UserName = result.UserName;
                        break;
                    }
                    case "twitter":
                    {
                        model.Email = result.UserName;
                        model.UserName = result.UserName;
                        break;
                    }
                    default:
                    {
                        model.Email = result.UserName;
                        model.UserName = result.UserName;
                        break;
                    }
                }

                actionResult =
                    Redirect(Url + "api/account/ExternalLoginConfirmation?username=" + model.UserName + "&email=" +
                             model.Email + "&externallogindata=" + model.ExternalLoginData + "&provider=" +
                             result.Provider + "&providerUserId=" + result.ProviderUserId);
            }

            return actionResult;
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult ExternalLoginConfirmation(string username, string externalLoginData, string email, string provider, string providerUserId)
        {
            AccountModels.RegisterExternalLoginModel model = new AccountModels.RegisterExternalLoginModel() {UserName = username, ExternalLoginData = externalLoginData, Email = email};

            AccountRepository.CreateUser(model, provider, providerUserId);

            return Redirect(Url + "api/account/ExternalLoginFinal?provider=" + provider + "&providerUserId=" + providerUserId);
        }

        //
        // GET: /Account/ExternalLoginFinal
        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult ExternalLoginFinal(string provider, string providerUserId)
        {
            IHttpActionResult result;
            if (OAuthWebSecurity.Login(provider, providerUserId, createPersistentCookie: false))
            {
                var username = OAuthWebSecurity.GetUserName(provider, providerUserId);
                result = Ok(username);
            }
            else
            {
                result = BadRequest();
            }
            
            return result;
        }

        //
        // GET: /Account/ExternalLoginFailure
        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult ExternalLoginFailure()
        {
            return BadRequest();
        }


        //
        // GET: /Account/Logout
        [HttpGet]
        public IHttpActionResult Logout()
        {
            WebSecurity.Logout();

            return Ok();
        }

        #region Helpers

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}