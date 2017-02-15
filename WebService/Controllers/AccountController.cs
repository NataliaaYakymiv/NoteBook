using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;
using DotNetOpenAuth.AspNet;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using WebService.Filters;
using WebService.Models;

namespace WebService.Controllers
{
    //[InitializeSimpleMembership]
    [System.Web.Http.RoutePrefix("api/Account")]
    //[System.Web.Http.Authorize]
    
    public class AccountController : ApiController
    {
        [System.Web.Http.HttpPost]
        [System.Web.Http.AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public IHttpActionResult Login(AccountModels.LoginModel model)
        {
            if (WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
            {
                //WebSecurity.
                var encodedToken = System.Convert.ToBase64String(Encoding.UTF8.GetBytes(model.UserName + ":" + model.Password));
                HttpContext.Current.Response.Headers.Add(HttpRequestHeader.Authorization.ToString(), "Basic " + encodedToken);
                //HttpRequestMessage requestMessage = new HttpRequestMessage();
                //requestMessage.Headers.Authorization = new AuthenticationHeaderValue();
                //IHttpActionResult result = StatusCode(HttpStatusCode.OK);
                return Ok("Successful login");
            }

            return BadRequest("The user name or password provided is incorrect.");
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public IHttpActionResult Register([FromBody] AccountModels.RegisterModel model)
        {
            string error;
            try
            {
                WebSecurity.CreateUserAndAccount(model.UserName, model.Password);
                //HttpRequestMessage response = new HttpRequestMessage();
                //response.Headers.Authorization = new AuthenticationHeaderValue();
                //IHttpActionResult result = new OkResult(response);
                return Ok("Registration successful");
            }
            catch (MembershipCreateUserException e)
            {
                error = ErrorCodeToString(e.StatusCode);
            }

            return BadRequest(error);
        }

        [System.Web.Http.HttpGet]
        [BasicAuthentication]
        //[ValidateAntiForgeryToken]
        //[System.Web.Http.AllowAnonymous]
        public string GetNumber()
        {
            return "kek";
        }

        //[System.Web.Http.HttpPost]
        //[System.Web.Http.AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public ActionResult ExternalLogin(string provider, string returnUrl)
        //{
        //    return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        //}

        //
        // GET: /Account/ExternalLoginCallback

        [System.Web.Http.AllowAnonymous]
        public IHttpActionResult ExternalLoginCallback(string returnUrl)
        {
            AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication(returnUrl);
            if (!result.IsSuccessful)
            {
                return Redirect("ExternalLoginFailure");
            }

            if (OAuthWebSecurity.Login(result.Provider, result.ProviderUserId, createPersistentCookie: false))
            {
                
                return Redirect(returnUrl);
            }

            if (User.Identity.IsAuthenticated)
            {
                // If the current user is logged in add the new account
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
                return Redirect(returnUrl);
            }
            else
            {
                string loginData = OAuthWebSecurity.SerializeProviderUserId(result.Provider, result.ProviderUserId);
                //ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(result.Provider).DisplayName;
                //ViewBag.ReturnUrl = returnUrl;
                var model = new AccountModels.RegisterExternalLoginModel { UserName = result.UserName, ExternalLoginData = loginData };
                switch (result.Provider)
                {
                    case "facebook":
                    case "google":
                        {
                            model.Email = result.UserName;
                            model.UserName = "";
                            break;
                        }
                    case "twitter":
                        {
                            model.Email = "";
                            model.UserName = result.UserName;
                            break;
                        }
                    default:
                        break;

                }
                return Ok("External login");
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation

        [System.Web.Http.HttpPost]
        [System.Web.Http.AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public IHttpActionResult ExternalLoginConfirmation(AccountModels.RegisterExternalLoginModel model, string returnUrl)
        {
            string provider = null;
            string providerUserId = null;

            if (User.Identity.IsAuthenticated || !OAuthWebSecurity.TryDeserializeProviderUserId(model.ExternalLoginData, out provider, out providerUserId))
            {
                return Redirect("/api/nodes/get");
            }

            if (ModelState.IsValid)
            {
                // Insert a new user into the database
                using (AccountModels.UsersContext db = new AccountModels.UsersContext())
                {
                    AccountModels.UserProfile user = db.UserProfiles.FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower());
                    // Check if user already exists
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

                        OAuthWebSecurity.Login(provider, providerUserId, createPersistentCookie: false);

                        return Redirect(returnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError("UserName", "User name already exists. Please enter a different user name.");
                    }
                }
            }

            //ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(provider).DisplayName;
            //ViewBag.ReturnUrl = returnUrl;
            return Ok("Confirm external login");
        }

        //
        // GET: /Account/ExternalLoginFailure

        [System.Web.Http.AllowAnonymous]
        public IHttpActionResult ExternalLoginFailure()
        {
            return BadRequest("Fail OAuth login");
        }

        [System.Web.Http.HttpGet]
        //[ValidateAntiForgeryToken]
        public IHttpActionResult Logout()
        {
            WebSecurity.Logout();
            return Ok("Logout");
        }
        #region Helpers
        //private ActionResult RedirectToLocal(string returnUrl)
        //{
        //    if (Url.IsLocalUrl(returnUrl))
        //    {
        //        return Redirect(returnUrl);
        //    }
        //    else
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }
        //}

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
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