using DrieUnityGarage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DrieUnityGarage.Controllers
{
    public class DrieUnityGarageController : Controller
    {
        DrieUnityGarageEntities db = new DrieUnityGarageEntities();
        // GET: HomePage
        public ActionResult HomePage()
        {
            return View();
        }
    }
}