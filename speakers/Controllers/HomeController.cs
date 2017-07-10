using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Data.Sql;
using System.Data.SqlClient;
using speakers.Models;

namespace speakers.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (Request.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public ActionResult Role_Selection()
        {
            if (Request.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
            
        }

        [HttpPost]
        public ActionResult Role_Selection(string role)
        {
            if (Request.IsAuthenticated)
            {
                Session["role"] = role;
                return RedirectToAction("Speakers_List", "Home");
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public ActionResult List()
        {
            return View("List", "_NoLayout");
        }

        public ActionResult Speakers_List()
        {
            if (Request.IsAuthenticated)
            {
                GlobalFuncs.get_speakers();
                ViewBag.isrequested = GlobalFuncs.is_current_requested();
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

    }
}