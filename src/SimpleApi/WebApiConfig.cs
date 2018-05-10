using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace EmptyProject2
{
    public static class WebApiConfig
    {
        public const string ApiRoot = "api";

        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}",
                defaults: new { controller="Home" }
            );
        }
    }
    public static class HttpVerbs
    {
        public const string GET = nameof(GET);
        public const string POST = nameof(POST);
        public const string PUT = nameof(PUT);
    }
}
