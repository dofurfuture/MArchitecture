﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Web.Attribute;

namespace Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            //config.Filters.Add(new ApiCustomAuthorizeAttribute());
            config.Filters.Add(new ApiExceptionFilterAttribute());
        }
    }
}
