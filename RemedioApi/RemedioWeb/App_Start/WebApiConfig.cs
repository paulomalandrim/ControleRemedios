using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace RemedioWeb.App_Start
{
    public class WebApiConfig
    {
		public static void Register(HttpConfiguration config)
		{
			// Web API configuration and services
			config.Formatters.Remove(config.Formatters.XmlFormatter); //Remove xml para deixar json padrão
																	  // Web API routes
			config.MapHttpAttributeRoutes();

			config.Routes.MapHttpRoute(
				name: "DefaultApi",
				routeTemplate: "api/{controller}/{action}/{id}",
				defaults: new { id = RouteParameter.Optional }
			);
		}

	}
}