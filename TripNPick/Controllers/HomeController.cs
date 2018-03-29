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
            inte.Add(new Interests() { interestsID = 0, interestsName = "Camping", ischecked = false });
            inte.Add(new Interests() { interestsID = 1, interestsName = "Hiking", ischecked = false });
            inte.Add(new Interests() { interestsID = 2, interestsName = "Skiing", ischecked = false });
            inte.Add(new Interests() { interestsID = 3, interestsName = "Surfing", ischecked = false });

            List<States> state = new List<States>();
            state.Add(new States() { statesID = 0, statesShortName = "ACT", statesName = "Australian Capital Territory", ischecked = false });
            state.Add(new States() { statesID = 1, statesShortName = "NSW", statesName = "New South Wales", ischecked = false });
            state.Add(new States() { statesID = 2, statesShortName = "NT", statesName = "Northern Territory", ischecked = false });
            state.Add(new States() { statesID = 3, statesShortName = "QLD", statesName = "Queensland", ischecked = false });
            state.Add(new States() { statesID = 4, statesShortName = "SA", statesName = "South Australia", ischecked = false });
            state.Add(new States() { statesID = 5, statesShortName = "TAS", statesName = "Tasmania", ischecked = false });
            state.Add(new States() { statesID = 6, statesShortName = "VIC", statesName = "Victoria", ischecked = false });
            state.Add(new States() { statesID = 7, statesShortName = "WA", statesName = "Western Australia", ischecked = false });

            DropdownList ddList = new DropdownList();
            ddList.interests = inte;
            ddList.states = state;
            
            return View(ddList);
        }

        public ActionResult About(string InteString, string StatString)
        {
            ViewBag.Message = InteString + StatString;
            System.Diagnostics.Debug.WriteLine(InteString + StatString);

            return View();
        }

        public ActionResult Results()
        {
            InterestsJson interets = getInterests("a", "a");
            FarmsJson farms = new FarmsJson();
            FarmsJson farmsList = new FarmsJson();
            List<string> s = new List<string>();
            s.Add("");
            farmsList.html_attributions = s;
            farmsList.status = "";
            List<FarmResults> re = new List<FarmResults>();
            farmsList.results = re;

            foreach (var interest in interets.results)
            {
                farms = getFarms(interest.geometry.location.lat, interest.geometry.location.lng, 25000, interest);
                foreach (var item in farms.results)
                {
                    farmsList.results.Add(item);
                }
            }
            return View(farmsList);
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
            interest = "Surfing";
            state = "Victoria";
            string searchUrl = string.Format("https://maps.googleapis.com/maps/api/place/textsearch/" + returnType + "?query=" + interest + "+in+" + state + "Australia&key=" + myKey);
            var json_data = string.Empty;
            json_data = client.DownloadString(searchUrl);
            InterestsJson interets = JsonConvert.DeserializeObject<InterestsJson>(json_data);
            foreach (var item in interets.results)
            {
                item.actualType = interest;
            }
            return interets;
        }

        public FarmsJson getFarms(double lat, double lng, int radius, IntResults interest)
        {
            var client = new WebClient();
            string myKey = "AIzaSyC1IPf50kkZ5oZy0uQmLHyobjd7MF5ugsA";
            string returnType = "json";
            radius = 20000;
            string searchUrl = string.Format("https://maps.googleapis.com/maps/api/place/nearbysearch/" + returnType + "?location=" + lat + "," + lng + "&radius=" + radius + "&type=&keyword=farm&key=" + myKey);
            var json_data = string.Empty;
            json_data = client.DownloadString(searchUrl);
            FarmsJson newFarmJson = JsonConvert.DeserializeObject<FarmsJson>(json_data);
            foreach (var farm in newFarmJson.results)
            {
                farm.interest = interest;
            }
            return newFarmJson;
        }
    }
}