﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Providers.Entities;
using WebMatrix.WebData;
using WebService.Models;

namespace WebService.Filters
{
    public class BasicAuthenticationAttribute : System.Web.Http.Filters.ActionFilterAttribute
    {
        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            if (actionContext.Request.Headers.Authorization == null)
            {
                actionContext.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
            }
            else
            {
                string authToken = actionContext.Request.Headers.Authorization.Parameter;
                string decodedToken = Encoding.UTF8.GetString(Convert.FromBase64String(authToken));

                string userName = decodedToken.Substring(0, decodedToken.IndexOf(":"));
                string password = decodedToken.Substring(decodedToken.IndexOf(":") + 1);

                if (WebSecurity.Login(userName, password, false))
                {
                    AccountModels.UserProfile user = new AccountModels.UserProfile() { UserName = userName };
                    HttpContext.Current.User = new GenericPrincipal(new ApiIdentity(user), new string[] { });
                    Thread.CurrentPrincipal = new GenericPrincipal(new ApiIdentity(user), new string[] { });
                    base.OnActionExecuting(actionContext);
                }
                else
                {
                    actionContext.Response =
                        new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
                }
            }
        }

    }
    public class ApiIdentity : IIdentity
    {
        public AccountModels.UserProfile User { get; private set; }
        public ApiIdentity(AccountModels.UserProfile user)
        {
            if (user == null)
                throw new ArgumentNullException("User");

            this.User = user;
        }
        public string Name
        {
            get { return this.User.UserName; }
        }

        public string AuthenticationType
        {
            get { return "Basic"; }
        }

        public bool IsAuthenticated
        {
            get { return true; }
        }
    }
}