using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using RegistryInformationSystem.Context;
using RegistryInformationSystem.Features;
using RegistryInformationSystem.Models.Login;
using RegistryInformationSystem.Models.Register;

namespace RegistryInformationSystem.Controllers
{
    public class LoginController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login([Bind(Include = "LoginEmail,Password")] Login login)
        {
			if (ModelState.IsValid)
			{
				using (DatabaseContext db = new DatabaseContext())
				{
                    int IsUserExist = db.Registers.Select(x => x.RegisterEmail == login.LoginEmail).Count();
                    if (IsUserExist == 1)
                    {
						Session["LoginEmail"] = login.LoginEmail.ToString();
						Session["Password"] = login.Password.ToString();
                        return View("~/Views/Login/Register.cshtml", new Register());
                    }
					else
					{
                        ViewBag.UserNotRegistered = "This email is not registered";
						return View("~/Views/Login/Index.cshtml");
					}
				}
			}
			else
			{
				return View("~/Views/Login/Index.cshtml");
			}
        }
        
        public ActionResult Register()
        {
            return View();
        }
        public ActionResult Contact()
        {
            return View();
        }
        public ActionResult About()
        {
            return View();
        }

        public ActionResult CreateNewUser([Bind(Include ="Name,Gender,RegisterEmail,Password")] Register register)
        {
			if (ModelState.IsValid)
			{
				using (DatabaseContext db = new DatabaseContext())
                {
                    int IsUserExist = db.Registers.Where(x => x.RegisterEmail == register.RegisterEmail).Select(x=>x.RegisterEmail).Count();
                    if(IsUserExist == 1)
                    {
                        ViewBag.UserRegistered = "This email is already taken, Please register with another email";
                        return View("~/Views/Login/Register.cshtml", new Register());
                    }
                    else
                    {
                        var userDetails = new Register
                        {
                            Name = register.Name,
                            Gender = register.Gender,
                            RegisterEmail = register.RegisterEmail,
                            Password = register.Password
                        };
                        db.Registers.Add(userDetails);
                        db.SaveChanges();
                        Session["LoginEmail"] = register.RegisterEmail.ToString();
                        Session["Password"] = register.Password.ToString();
                        return View("~/Views/User/Index.cshtml", new Register());
                    }
                }
            }
            else
            {
                return View("~/Views/Login/Register.cshtml", new Register());
            }
        }

        public ActionResult SessionClear()
        {            
            Session.Abandon();
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            return RedirectToAction("Index");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        } 
    }
}
