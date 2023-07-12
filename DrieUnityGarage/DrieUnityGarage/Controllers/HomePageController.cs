using DrieUnityGrage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DrieUnityGrage.Controllers
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