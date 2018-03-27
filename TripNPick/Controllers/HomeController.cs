using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Net;
using TripNPick.Models;

namespace TripNPick.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            List<Interests> inte = new List<Interests>();
            inte.Add(new Interests() { interestsID = 1, interestsName = "Camping", ischecked = false });
            inte.Add(new Interests() { interestsID = 2, interestsName = "Hiking", ischecked = false });
            inte.Add(new Interests() { interestsID = 3, interestsName = "Skiing", ischecked = false });
            inte.Add(new Interests() { interestsID = 4, interestsName = "Surfing", ischecked = false });

            List<States> state = new List<States>();
            state.Add(new States() { statesID = 1, statesShortName = "ACT", statesName = "Australian Capital Territory", ischecked = false });
            state.Add(new States() { statesID = 2, statesShortName = "NSW", statesName = "New South Wales", ischecked = false });
            state.Add(new States() { statesID = 3, statesShortName = "NT", statesName = "Northern Territory", ischecked = false });
            state.Add(new States() { statesID = 4, statesShortName = "QLD", statesName = "Queensland", ischecked = false });
            state.Add(new States() { statesID = 5, statesShortName = "SA", statesName = "South Australia", ischecked = false });
            state.Add(new States() { statesID = 6, statesShortName = "TAS", statesName = "Tasmania", ischecked = false });
            state.Add(new States() { statesID = 7, statesShortName = "VIC", statesName = "Victoria", ischecked = false });
            state.Add(new States() { statesID = 8, statesShortName = "WA", statesName = "Western Australia", ischecked = false });

            DropdownList ddList = new DropdownList();
            ddList.interests = inte;
            ddList.states = state;

            return View(ddList);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public InterestsJson getInterests(string interest, string state)
        {
            var client = new WebClient();
            string myKey = "AIzaSyC1IPf50kkZ5oZy0uQmLHyobjd7MF5ugsA";
            string returnType = "json";
            interest = "skiing";
            state = "Victoria";
            string searchUrl = string.Format("https://maps.googleapis.com/maps/api/place/textsearch/" + returnType + "?query=" + interest + "+in+" + state + "Australia&key=" + myKey);
            var json_data = string.Empty;
            json_data = client.DownloadString(searchUrl);
            return JsonConvert.DeserializeObject<InterestsJson>(json_data);
        }
    }
}