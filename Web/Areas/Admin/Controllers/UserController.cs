﻿using MArc.IService;
using MArc.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Common;

namespace Web.Areas.Admin.Controllers
{
    public class UserController : BaseController
    {
        private IServiceBase serviceBase;
        //private RepositoryIdentity repositoryIdentity;
        public UserController()
        {
            serviceBase = this.ServiceBase;
            //repositoryIdentity = new RepositoryIdentity();
        }
        //
        // GET: /Admin/User/
        public ActionResult Index()
        {
            //User user = serviceBase.FindObject<User>(m => m.Id == 1);
            SqlParameter parameter = new SqlParameter("@Id", 1);
            User user = serviceBase.FindAllByProc<User>("ProcTest",parameter).FirstOrDefault();
            //AppUser user = repositoryIdentity.FindUser().FirstOrDefault();

            return View(user);
        }
	}
}