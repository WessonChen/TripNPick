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
        //The view of the home page
        public ActionResult Index()
        {
            return View();
        }

        //The view of the accomodation dashboard
        public ActionResult Accomodation()
        {
            return View();
        }

        //The view of the farm details page
        public ActionResult FarmDetails()
        {
            return View();
        }

        //The view of the information page
        public ActionResult Information()
        {
            return View();
        }

        //The default view of the about us page, with identifier 'new'
        public ActionResult AboutUS()
        {
            SuggestionForm newForm = new SuggestionForm();
            ViewData["info"] = "New";
            return View(newForm);
        }

        //The view of the about us page after user posted their suggestion
        //With identifier 'Success!' if it is a valid input, or 'Sorry!' if it is an invalid input.
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