using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TripNPick.Controllers
{
    public class ErrorPageController : Controller
    {
        //The view shows when user enter an invalid path
        public ActionResult ErrorMessage()
        {
            return View();
        }
    }
}