﻿using LinqKit;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using TripNPick.Models;

namespace TripNPick.Controllers
{
    public class MapController : Controller
    {
        ColdSpotDBEntities dbContext = new ColdSpotDBEntities();

        public ActionResult Index(string[] cMonths, string[] cInterests)
        {
            UserSelections us = new UserSelections();
            us.cMonths = cMonths;
            us.cInterests = cInterests;
            us.combinedString = "";
            if (cMonths != null)
            {
                for (int i = 0; i < cMonths.Length; i++)
                {
                    if (i == cMonths.Length - 1)
                    {
                        us.combinedString = us.combinedString + cMonths[i].ToLower() + "|";
                    }
                    else
                    {
                        us.combinedString = us.combinedString + cMonths[i].ToLower() + ",";
                    }
                }
            }
            else
            {
                us.combinedString = "null|";
            }
            if (cInterests != null)
            {
                for (int i = 0; i < cInterests.Length; i++)
                {
                    if (i == cInterests.Length - 1)
                    {
                        us.combinedString = us.combinedString + cInterests[i];
                    }
                    else
                    {
                        us.combinedString = us.combinedString + cInterests[i] + ",";
                    }
                }
            }
            else
            {
                us.combinedString = us.combinedString + "null";
            }
            return View(us);
        }

        // GET: RegionMap
        public ActionResult RegionIndex()
        {
            var suburbList = dbContext.suburb_table.ToList();
            var harvestList = dbContext.suburb_harvest.ToList();
            var suburbCountViewModel = from s in suburbList
                                       join hv in harvestList on s.suburb_id equals hv.suburb_id
                                       group new { s, hv } by new { s.state } into groupedTable
                                       select new StateSuburbCount
                                       {
                                           stateName = groupedTable.Key.state,
                                           suburbCount = groupedTable.Select(x => x.hv.suburb_id).Distinct().Count()
                                       };

            return View(suburbCountViewModel);
        }

        public ActionResult MultipleMarkersMap()
        {
            return View();
        }

        public ActionResult NumberOfFarms()
        {
            var farmList = dbContext.farms.ToList();
            var suburbList = dbContext.suburb_table.ToList();
            var farmCountViewModel = from f in farmList
                                     join sl in suburbList on f.suburb_id equals sl.suburb_id
                                     group new { f, sl } by new { sl.state } into groupedTable2
                                     select new StateFarmsCount
                                     {
                                         stateName = groupedTable2.Key.state,
                                         numberOfFarms = groupedTable2.Select(x => x.sl.state).Count()
                                     };
            return View(farmCountViewModel);
        }

        public JsonResult GetAllLocations()
        {
            var farmList = dbContext.farms;
            var jsonFarms = from oneFarm in farmList
                            select new FarmJson
                            {
                                farm_id = oneFarm.farm_id,
                                farm_name = oneFarm.farm_name,
                                farm_address = oneFarm.farm_address,
                                location_lat = oneFarm.location_lat,
                                location_lng = oneFarm.location_lng
                            };
            return Json(jsonFarms, JsonRequestBehavior.AllowGet);



        }


        public Expression<Func<suburb_harvest, bool>> FilterFarms(string combinedString)
        {
            var months = new List<string>();
            var interests = new List<string>();
            string[] p = combinedString.Split('|');
            if (p[0].Equals("null"))
            {
                months.Add("january");
                months.Add("february");
                months.Add("march");
                months.Add("april");
                months.Add("may");
                months.Add("june");
                months.Add("july");
                months.Add("august");
                months.Add("sepetember");
                months.Add("october");
                months.Add("november");
                months.Add("december");
            }
            else
            {
                months = p[0].Split(',').ToList();
            }
            if (p[1].Equals("null") == false)
            {
                interests = p[1].Split(',').ToList();
            }

            var predicate = PredicateBuilder.New<suburb_harvest>();
            foreach (string month in months)
            {
                switch (month)
                {
                    case "january":
                        predicate = predicate.And(s => s.january != null);
                        break;
                    case "february":
                        predicate = predicate.And(s => s.february != null);
                        break;
                    case "march":
                        predicate = predicate.And(s => s.march != null);
                        break;
                    case "april":
                        predicate = predicate.And(s => s.april != null);
                        break;
                    case "may":
                        predicate = predicate.And(s => s.may != null);
                        break;
                    case "june":
                        predicate = predicate.And(s => s.june != null);
                        break;
                    case "july":
                        predicate = predicate.And(s => s.july != null);
                        break;
                    case "august":
                        predicate = predicate.And(s => s.august != null);
                        break;
                    case "sepetember":
                        predicate = predicate.And(s => s.september != null);
                        break;
                    case "october":
                        predicate = predicate.And(s => s.october != null);
                        break;
                    case "november":
                        predicate = predicate.And(s => s.november != null);
                        break;
                    case "december":
                        predicate = predicate.And(s => s.december != null);
                        break;
                }

            }
            return predicate;
        }

        public ActionResult FilteredNumberOfFarms()
        {
            var farmList = dbContext.farms.ToList();
            var suburbList = dbContext.suburb_table.ToList();
            var states = dbContext.states.ToList();
            var harvestList = dbContext.suburb_harvest.ToList();
            var newHarvestList = harvestList.AsQueryable().Where(this.FilterFarms(""));
            var farmCountViewModel = (from f in farmList
                                      join sl in suburbList on f.suburb_id equals sl.suburb_id
                                      join st in states on sl.state equals st.state_id
                                      join hv in newHarvestList on sl.suburb_id equals hv.suburb_id
                                      select new FilteredFarmViewModel
                                      {
                                          farmName = f.farm_name,
                                          farmId = f.farm_id,
                                          suburbName = sl.suburb_name,
                                          stateName = sl.state,
                                          state_lat =(double)st.state_lat,
                                          state_lng = (double)st.state_lng
                                      }).ToList();

            var getDistinctFarms = farmCountViewModel.DistinctBy(x => x.farmName);
            var groupedFarms = getDistinctFarms.GroupBy(x => x.stateName).Select(c => new StateFarmsCount { stateName = c.Key, numberOfFarms = c.Count() });
            return View(groupedFarms);
        }

        public IEnumerable<FilteredFarmViewModel> getAllFilteredFarms(string combinedString)
        {
            var farmList = dbContext.farms.ToList();
            var suburbList = dbContext.suburb_table.ToList();
            var harvestList = dbContext.suburb_harvest.ToList();
            var states = dbContext.states.ToList();
            var newHarvestList = harvestList.AsQueryable().Where(this.FilterFarms(combinedString));
            var farmCountViewModel = (from f in farmList
                                      join sl in suburbList on f.suburb_id equals sl.suburb_id
                                      join hv in newHarvestList on sl.suburb_id equals hv.suburb_id
                                      select new FilteredFarmViewModel
                                      {
                                          farmName = f.farm_name,
                                          farmId = f.farm_id,
                                          suburbId = sl.suburb_id,
                                          suburbName = sl.suburb_name,
                                          stateName = sl.state,
                                          farm_lat = (double)f.location_lat,
                                          farm_lng = (double)f.location_lng,
                                          farm_address = f.farm_address,
                                          farm_rating = f.farm_rating,
                                          suburb_lat =(double)sl.suburb_lat,
                                          suburb_lng=(double)sl.suburb_lng
                                      }).ToList();
            var getDistinctFarms = farmCountViewModel.DistinctBy(x => x.farmName);
            return getDistinctFarms;
        }

        public JsonResult getFarmCountMonthlyFiltered(string combinedString)
        {
            //string combinedString = "april,may|Hiking Trails";
            var states = dbContext.states.ToList();
            var distinctFarms = getAllFilteredFarms(combinedString);
            var interestGroupedByState = groupInterestByState(combinedString);
            var farmsGroupedByState = distinctFarms.GroupBy(x => x.stateName).Select(c => new StateFarmsCount { stateName = c.Key, numberOfFarms = c.Count() });
            var joinStateLocation = from st in states
                                    join fg in farmsGroupedByState on st.state_id equals fg.stateName
                                    join po in interestGroupedByState on st.state_id equals po.stateName  
                                    select new CountPerState
                                    {
                                        stateName = fg.stateName,
                                        numberOfFarms = fg.numberOfFarms,
                                        numberOfInterests = po.numberOfInterests,
                                        location_lat = (double)st.state_lat,
                                        location_lng = (double)st.state_lng
                                    };
            return Json(joinStateLocation, JsonRequestBehavior.AllowGet);
        }

        public Expression<Func<interest_attraction, bool>> buildPredForInterestType(string combinedString)
        {
            List<string> selectedInterests = new List<string>();
            var months = new List<string>();
            string[] p = combinedString.Split('|');
            if (p[1].Equals("null"))
            {
                selectedInterests.Add("Museums");
                selectedInterests.Add("Sights & Landmarks");
                selectedInterests.Add("Nature and Parks");
                selectedInterests.Add("Points of Interest & Landmarks");
                selectedInterests.Add("Beaches");
                selectedInterests.Add("Outdoor Activities and Tours");
                selectedInterests.Add("Nature & Wildlife Areas");
                selectedInterests.Add("Hiking Trails");
                selectedInterests.Add("Fun & Games & Sports");
                selectedInterests.Add("Zoos & Aquariums");
                selectedInterests.Add("Bodies of Water");
            }
            else
            {
                selectedInterests = p[1].Split(',').ToList();
            }
            if (p[0].Equals("null") == false)
            {
                months = p[0].Split(',').ToList();
            }
            
            var predicate = PredicateBuilder.New<interest_attraction>();
            foreach (var interest in selectedInterests)
            {
                switch (interest)
                {
                    case "Museums":
                        predicate = predicate.Or(s => s.interest_id == 1);
                        break;
                    case "Sights & Landmarks":
                        predicate = predicate.Or(s => s.interest_id == 2);
                        break;
                    case "Nature and Parks":
                        predicate = predicate.Or(s => s.interest_id == 3);
                        break;
                    case "Points of Interest & Landmarks":
                        predicate = predicate.Or(s => s.interest_id == 4);
                        break;
                    case "Beaches":
                        predicate = predicate.Or(s => s.interest_id == 5);
                        break;
                    case "Outdoor Activities and Tours":
                        predicate = predicate.Or(s => s.interest_id == 6);
                        break;
                    case "Nature & Wildlife Areas":
                        predicate = predicate.Or(s => s.interest_id == 7);
                        break;
                    case "Hiking Trails":
                        predicate = predicate.Or(s => s.interest_id == 8);
                        break;
                    case "Fun & Games & Sports":
                        predicate = predicate.Or(s => s.interest_id == 9);
                        break;
                    case "Zoos & Aquariums":
                        predicate = predicate.Or(s => s.interest_id == 10);
                        break;
                    case "Bodies of Water":
                        predicate = predicate.Or(s => s.interest_id == 11);
                        break;
                }
            }
            return predicate;
        }
        public IEnumerable<AllInterest> getAllInterest2(string combinedString)
        {
            var interestTypes = dbContext.interest_table.ToList();
            var interestAttractions = dbContext.interest_attraction.ToList();
            var filteredInterestAttr = interestAttractions.AsQueryable().Where(this.buildPredForInterestType(combinedString));
            var suburbs = dbContext.suburb_table.ToList();
            var states = dbContext.states.ToList();
            var allInterest = from it in interestTypes
                              join ia in filteredInterestAttr on it.interest_id equals ia.interest_id
                              join sb in suburbs on ia.suburb_id equals sb.suburb_id
                              join st in states on sb.state equals st.state_id
                              select new AllInterest
                              {
                                  stateName = st.state_id,
                                  interestId = it.interest_id,
                                  interestType = it.types,
                                  attractionId = ia.attraction_id,
                                  attractionName = ia.attraction_name,
                                  suburbId = sb.suburb_id,
                                  suburbName = sb.suburb_name
                              };
            return allInterest;
        }
        public IEnumerable<AllInterest> getAllInterestInAState(string combinedString)
        {
            var interestTypes = dbContext.interest_table.ToList();
            var interestAttractions = dbContext.interest_attraction.ToList();
            var filteredInterestAttr = interestAttractions.AsQueryable().Where(this.buildPredForInterestType(combinedString));
            var suburbs = dbContext.suburb_table.ToList();
            var states = dbContext.states.ToList();
            var allInterest = from it in interestTypes
                              join ia in filteredInterestAttr on it.interest_id equals ia.interest_id
                              join sb in suburbs on ia.suburb_id equals sb.suburb_id
                              join st in states on sb.state equals st.state_id
                              select new AllInterest
                              {
                                  stateName = st.state_id,
                                  interestId = it.interest_id,
                                  interestType = it.types,
                                  attractionId = ia.attraction_id,
                                  attractionName = ia.attraction_name,
                                  suburbId = sb.suburb_id,
                                  suburbName = sb.suburb_name
                              };
            return allInterest;
        }

        public IEnumerable<StateInterestsCount> groupInterestByState(string combinedString)
        {
            var allInterests = getAllInterest2(combinedString);
            var interestsGrouped = allInterests.GroupBy(x => x.stateName).Select(c => new StateInterestsCount { stateName = c.Key, numberOfInterests = c.Count() });
            return interestsGrouped;
        }

        public IEnumerable<SuburbInterestsCount> groupInterestsBySuburb(string combinedString, string stateName)
        {
            //var stateName = "NSW";
            //string combinedString = "april,may|Hiking Trails";
            var suburbs = dbContext.suburb_table.ToList();
            var suburbsInASate = suburbs.AsQueryable().Where(x => x.state.Equals(stateName));
            var interestTypes = dbContext.interest_table.ToList();
            var interestAttractions = dbContext.interest_attraction.ToList();
            var filteredInterestAttr = interestAttractions.AsQueryable().Where(this.buildPredForInterestType(combinedString));
            var allInterest = from it in interestTypes
                              join ia in filteredInterestAttr on it.interest_id equals ia.interest_id
                              join sb in suburbsInASate on ia.suburb_id equals sb.suburb_id
                              select new AllInterest
                              {
                                  stateName = sb.state,
                                  interestId = it.interest_id,
                                  interestType = it.types,
                                  attractionId = ia.attraction_id,
                                  attractionName = ia.attraction_name,
                                  suburbId = sb.suburb_id,
                                  suburbName = sb.suburb_name
                              };
            var interestsGrouped = allInterest.GroupBy(x => x.suburbId).Select(c => new SuburbInterestsCount { suburbId = c.Key, numberOfInterests = c.Count() });
            return interestsGrouped;
        }

        public IQueryable<SuburbFarmsCount> groupFarmsBySuburb(string combinedString, string stateName)
        {
           // string combinedString = "april,may|Hiking Trails";
            //var stateName = "NSW";
            var allFilteredFarms = getAllFilteredFarms(combinedString).ToList();
            var farmsInState = allFilteredFarms.AsQueryable().Where(x => x.stateName.Equals(stateName));
            var groupFarmsBySuburbs = farmsInState.GroupBy(x => x.suburbId).Select(c => new SuburbFarmsCount { suburbId = c.Key, numberOfFarms = c.Count() });
            return groupFarmsBySuburbs;
        }

        public JsonResult getSuburbWiseData(string userInput)
        {
            Debug.WriteLine(userInput);
            string[] p = userInput.Split(':');
            var combinedString = p[0];
            var selection = p[1];
            var groupedFarms = groupFarmsBySuburb(combinedString, selection);
            var groupedInterests = groupInterestsBySuburb(combinedString, selection);
            var suburbs = dbContext.suburb_table.ToList();
            var countSuburbWise = from sb in suburbs
                                  join gi in groupedInterests on sb.suburb_id equals gi.suburbId
                                  join gf in groupedFarms on sb.suburb_id equals gf.suburbId
                                  select new CountPerSuburb
                                  {
                                      suburbId = sb.suburb_id,
                                      suburbName = sb.suburb_name,
                                      numberOfFarms = gf.numberOfFarms,
                                      numberOfInterests = gi.numberOfInterests,
                                      suburbLat = (double)sb.suburb_lat,
                                      suburbLng = (double)sb.suburb_lng
                                  };
            return Json(countSuburbWise, JsonRequestBehavior.AllowGet);
        }


        //public ActionResult displayFilteredFarms()
        //{
        //    var allFarmsAllStates = this.getAllFilteredFarms(us.combinedString);
        //    return View(allFarmsAllStates);
        //}
        //public ActionResult displayFarmCountMonthlyFiltered()
        //{
        //    var farmsGroupedByState = getFarmCountMonthlyFiltered();
        //    return View(farmsGroupedByState);
        //}
        //public ActionResult stupidMarkers()
        //{
        //    var farmList = getAllFilteredFarms();
        //    return View();
        //}
    }
}