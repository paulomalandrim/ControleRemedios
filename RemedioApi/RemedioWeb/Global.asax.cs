using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Http;
using System.Web.Routing;
using RemedioWeb.App_Start;
using System.Web.Configuration;
using System.Text;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

namespace RemedioWeb
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            byte[] fcmAdminBytes = Convert.FromBase64String(WebConfigurationManager.AppSettings["FcmAdminJson"]);

            string json = Encoding.UTF8.GetString(fcmAdminBytes);

            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromJson(json)
            });
        }
    }
}
