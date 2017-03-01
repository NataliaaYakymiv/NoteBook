using System;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading;
using System.Web.Http;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace WebService
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static SimpleMembershipInitializer _initializer;
        private static object _initializerLock = new object();
        private static bool _isInitialized;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            AuthConfig.RegisterAuth();
            LazyInitializer.EnsureInitialized(ref _initializer, ref _isInitialized, ref _initializerLock);
        }
    }
    public class SimpleMembershipInitializer
    {
        public SimpleMembershipInitializer()
        {
            if (!WebSecurity.Initialized)
                WebSecurity.InitializeDatabaseConnection("DefaultConnection", "UserProfile", "UserId", "UserName",
                    autoCreateTables: true);
        }
    }

    public class UploadMultipartMediaTypeFormatter : MediaTypeFormatter
    {
        UploadMultipartMediaTypeFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("multipart/form-data"));
        }

        public override bool CanReadType(Type type)
        {
            throw new System.NotImplementedException();
        }

        public override bool CanWriteType(Type type)
        {
            throw new System.NotImplementedException();
        }
    }
}



