﻿using MArc.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Web.Common;

namespace Web
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //EntityModel表更自动更新数据表结构
            Database.SetInitializer<AppIdentityDbContext>(new MigrateDatabaseToLatestVersion<AppIdentityDbContext, ConfigurationIdentity>());
            Database.SetInitializer<DbContextBase>(new MigrateDatabaseToLatestVersion<DbContextBase, ConfigurationBase>());
            //AutoMapper配置
            ConfigurationAutoMapper.Configure();
        }
    }
}
