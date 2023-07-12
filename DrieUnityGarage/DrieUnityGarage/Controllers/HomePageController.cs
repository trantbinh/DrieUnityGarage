using DrieUnityGarage.Models;
using DrieUnityGarage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DrieUnityGarage.Controllers
{
    public class HomePageController : Controller
    {
        DrieUnityGarageEntities db = new DrieUnityGarageEntities();
        // GET: HomePage
        public ActionResult View_HomePage()
        {
            return View();
        }
    }
}