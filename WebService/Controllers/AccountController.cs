using System;
using System.Linq;
using System.Web.Http;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using WebService.Models;

namespace WebService.Controllers
{
    [System.Web.Http.Authorize]
    [System.Web.Http.RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {

        public string Url { get; } = "http://192.168.1.127:81";

        //
        // POST: /Account/Login
        [System.Web.Http.HttpPost]
        [System.Web.Http.AllowAnonymous]
        public IHttpActionResult Login(AccountModels.LoginModel model)
        {
            if (WebSecurity.Login(model.UserName, model.Password, persistCookie: true))
            {
                return Ok("Successful login");
            }

            return BadRequest("The user name or password provided is incorrect.");
        }

        //
        // POST: /Account/Register
        [System.Web.Http.HttpPost]
        [System.Web.Http.AllowAnonymous]
        public IHttpActionResult Register([FromBody] AccountModels.RegisterModel model)
        {
            string error;
            try
            {
                WebSecurity.CreateUserAndAccount(model.UserName, model.Password);
                return Ok("Registration successful");
            }
            catch (MembershipCreateUserException e)
            {
                error = ErrorCodeToString(e.StatusCode);
            }

            return BadRequest(error);
        }


        //
        // GET: /Account/ExternalLogin
        [System.Web.Http.HttpGet]
        [System.Web.Http.AllowAnonymous]
        public IHttpActionResult ExternalLogin([FromUri]string provider)
        {
            OAuthWebSecurity.RequestAuthentication(provider, Url + "/api/account/ExternalLoginCallback");
            return Ok("ExternalLogin");
        }

        //
        // GET: /Account/ExternalLoginCallback
        [System.Web.Http.HttpGet]
        [System.Web.Http.AllowAnonymous]
        public IHttpActionResult ExternalLoginCallback()
        {
            AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication();
            if (!result.IsSuccessful)
            {
                return Redirect(Url + "/api/account/ExternalLoginFailure");
            }

            if (OAuthWebSecurity.Login(result.Provider, result.ProviderUserId, createPersistentCookie: false))
            {
                return Redirect(Url + "/api/account/ExternalLoginFinal?provider=" + result.Provider + "&providerUserId=" + result.ProviderUserId);
            }

            if (User.Identity.IsAuthenticated)
            {

                OAuthWebSecurity.CreateOrUpdateAccount(result.Provider, result.ProviderUserId, User.Identity.Name);
                if (result.Provider == "facebook" || result.Provider == "google")
                {
                    using (AccountModels.UsersContext db = new AccountModels.UsersContext())
                    {
                        AccountModels.UserProfile user = db.UserProfiles.FirstOrDefault(u => u.UserName.ToLower() == User.Identity.Name);
                        if (user != null)
                        {
                            var oauthItem = db.OAuthMemberships.FirstOrDefault(x => x.Provider == result.Provider && x.ProviderUserId == result.ProviderUserId && x.UserId == user.UserId);
                            if (oauthItem != null)
                            {
                                oauthItem.Email = result.UserName;
                                db.SaveChanges();
                            }
                        }
                    }
                }
                return Redirect(Url + "/api/account/ExternalLoginFinal?provider=" + result.Provider + "&providerUserId=" + result.ProviderUserId);
            }
            else
            {
                string loginData = OAuthWebSecurity.SerializeProviderUserId(result.Provider, result.ProviderUserId);
                var model = new AccountModels.RegisterExternalLoginModel { UserName = result.UserName, ExternalLoginData = loginData };
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

                ExternalLoginConfirmation(model, "");
                return Ok(model);
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [System.Web.Http.HttpPost]
        [System.Web.Http.AllowAnonymous]
        public IHttpActionResult ExternalLoginConfirmation(AccountModels.RegisterExternalLoginModel model, string returnUrl)
        {
            string provider = null;
            string providerUserId = null;

            if (User.Identity.IsAuthenticated || !OAuthWebSecurity.TryDeserializeProviderUserId(model.ExternalLoginData, out provider, out providerUserId))
            {
                return Redirect(Url + "/api/account/ExternalLoginFinal?provider=" + provider + "&providerUserId=" + providerUserId);
            }

            if (ModelState.IsValid)
            {
                using (AccountModels.UsersContext db = new AccountModels.UsersContext())
                {
                    AccountModels.UserProfile user = db.UserProfiles.FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower());

                    if (user == null)
                    {
                        user = new AccountModels.UserProfile { UserName = model.UserName };
                        db.UserProfiles.Add(user);
                        db.SaveChanges();

                        OAuthWebSecurity.CreateOrUpdateAccount(provider, providerUserId, model.UserName);

                        if (!String.IsNullOrEmpty(model.Email))
                        {
                            var oauthItem = db.OAuthMemberships.FirstOrDefault(x => x.Provider == provider && x.ProviderUserId == providerUserId && x.UserId == user.UserId);
                            if (oauthItem != null)
                            {
                                oauthItem.Email = model.Email;
                                db.SaveChanges();
                            }
                        }

                        return Redirect(Url + "/api/account/ExternalLoginFinal?provider=" + provider + "&providerUserId=" + providerUserId);
                    }
                   
                }
            }

            return Ok("Confirm external login");
        }

        //
        // GET: /Account/ExternalLoginFinal
        [System.Web.Http.HttpGet]
        [System.Web.Http.AllowAnonymous]
        public IHttpActionResult ExternalLoginFinal(string provider, string providerUserId)
        {
            if (OAuthWebSecurity.Login(provider, providerUserId, createPersistentCookie: false))
            {
                return Ok("Successful login");
            }
            return BadRequest("Fail OAuth login");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [System.Web.Http.HttpGet]
        [System.Web.Http.AllowAnonymous]
        public IHttpActionResult ExternalLoginFailure()
        {
            return BadRequest("Fail OAuth login");
        }


        //
        // GET: /Account/Logout
        [System.Web.Http.HttpGet]
        public IHttpActionResult Logout()
        {
            WebSecurity.Logout();
            return Ok("Logout");
        }

        #region Helpers

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

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