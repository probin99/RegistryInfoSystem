using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using RegistryInformationSystem.Context;
using RegistryInformationSystem.Features;
using RegistryInformationSystem.Models.Software;

namespace RegistryInformationSystem.Controllers
{
    public class ComputerSoftwareController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        // GET: ComputerSoftware/Details
        public ActionResult Details()
        {
            List<ComputerSoftware> SoftwareFromDB = new List<ComputerSoftware>(db.ComputerSoftwares);
            if (SoftwareFromDB.Count() > 0)
            {
                return View(SoftwareFromDB);
            }
            else
            {
                return View();
            }
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
