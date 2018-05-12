using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Net;
using TripNPick.Models;
using System.IO;

namespace TripNPick.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Dashboard()
        {
            return View();
        }

        public ActionResult Accomodation() {
            return View();
        }

        public ActionResult LogInView()
        {
            return View();
        }

        public ActionResult FarmDetails()
        {
            return View();
        }

        public ActionResult Information()
        {
            return View();
        }

        public ActionResult AboutUS()
        {
            SuggestionForm newForm = new SuggestionForm();
            ViewData["info"] = "New";
            return View(newForm);
        }


        [HttpPost]
        public ActionResult AboutUS(SuggestionForm newForm)
        {
            if (ModelState.IsValid) {
                ViewData["info"] = "Success!";
                ModelState.Clear();
                SuggestionForm oneForm = new SuggestionForm() { userLastName = string.Empty, userFirstName=string.Empty, userEmail=string.Empty,
                    userSuggestion = string.Empty
                };
                return View("AboutUS", oneForm); 
            }
            ViewData["info"] = "Sorry!";
            return View(newForm);
        }
    }
}