﻿using System;
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
            //ddList.states = state;
            
            return View(ddList);
        }

        public ActionResult Results(string yourInte)
        {
            InterestsJson interets = getInterests(yourInte, "a");
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

        public ActionResult Payments()
        {

            return View();
        }

        public ActionResult FarmDetails()
        {
            return View();
        }

        public ActionResult Taxs()
        {

            return View();
        }

        public ActionResult Rights()
        {

            return View();
        }

        public ExpList getLocalJson()
        {
            ExpList expList = new ExpList();

            using (StreamReader sr = new StreamReader(Server.MapPath("~/App_Data/farm.json")))
            {
                ExpFarmList farmList = JsonConvert.DeserializeObject<ExpFarmList>(sr.ReadToEnd());
                expList.farms = farmList;
            }
            using (StreamReader sr = new StreamReader(Server.MapPath("~/App_Data/harvest.json")))
            {
                ExpHarvestList harvestList = JsonConvert.DeserializeObject<ExpHarvestList>(sr.ReadToEnd());
                expList.harvests = harvestList;
            }
            using (StreamReader sr = new StreamReader(Server.MapPath("~/App_Data/interest.json")))
            {
                ExpInterestList interestList = JsonConvert.DeserializeObject<ExpInterestList>(sr.ReadToEnd());
                expList.intersts = interestList;
            }
            using (StreamReader sr = new StreamReader(Server.MapPath("~/App_Data/weather.json")))
            {
                ExpWeatherList weatherList = JsonConvert.DeserializeObject<ExpWeatherList>(sr.ReadToEnd());
                expList.weathers = weatherList;
            }

            return expList;
        }

        public ActionResult ExpDetails (int i)
        {
            ExpList theList = getLocalJson();
            return View(theList);
        }

        public ActionResult ExpResults (string yourInte)
        {
            ExpList theList = getLocalJson();
            List<int> inteID = new List<int>();
            foreach (var inte in theList.intersts.a)
            {
                if (inte.interest_type.ToUpper().Equals(yourInte.ToUpper()))
                {
                    inteID.Add(inte.Interest_place_ID);
                }
            }
            ExpFarmList newList = new ExpFarmList();
            List<ExpFarm> far = new List<ExpFarm>();
            Random rnd = new Random();
            foreach (var farm in theList.farms.a)
            {
                if (inteID.Contains(farm.Interest_place_ID))
                {
                    double rate = rnd.Next(30, 51) / 10;
                    double dist = rnd.Next(30, 101) / 10;
                    farm.rating = rate;
                    farm.distance = dist;
                    far.Add(farm);
                }
            }
            newList.a = far;
            theList.farms = newList;
            return View(theList);
        }

        public InterestsJson getInterests(string interest, string state)
        {
            var client = new WebClient();
            string myKey = "AIzaSyC1IPf50kkZ5oZy0uQmLHyobjd7MF5ugsA";
            string returnType = "json";
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
            radius = 10000;
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