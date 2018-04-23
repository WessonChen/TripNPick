using LinqKit;
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
                Debug.WriteLine(month);
                switch (month)
                {
                    case "january":
                        predicate = predicate.And(s => s.january != "NULL");
                        break;
                    case "february":
                        predicate = predicate.And(s => s.february != "NULL");
                        break;
                    case "march":
                        predicate = predicate.And(s => s.march != "NULL");
                        break;
                    case "april":
                        predicate = predicate.And(s => s.april != "NULL");
                        break;
                    case "may":
                        predicate = predicate.And(s => s.may != "NULL");
                        break;
                    case "june":
                        predicate = predicate.And(s => s.june != "NULL");
                        break;
                    case "july":
                        predicate = predicate.And(s => s.july != "NULL");
                        break;
                    case "august":
                        predicate = predicate.And(s => s.august != "NULL");
                        break;
                    case "september":
                        predicate = predicate.And(s => s.september != "NULL");
                        break;
                    case "october":
                        predicate = predicate.And(s => s.october != "NULL");
                        break;
                    case "november":
                        predicate = predicate.And(s => s.november != "NULL");
                        break;
                    case "december":
                        predicate = predicate.And(s => s.december != "NULL");
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
                                          state_lat = (double)st.state_lat,
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
                                          suburb_lat = (double)sl.suburb_lat,
                                          suburb_lng = (double)sl.suburb_lng
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
                selectedInterests.Add("Beaches");
                selectedInterests.Add("Outdoor Activities and Tours");
                selectedInterests.Add("Nature & Wildlife Areas");
                selectedInterests.Add("Hiking Trails");
                selectedInterests.Add("Fun & Games & Sports");
                selectedInterests.Add("Zoos & Aquariums");
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
                        predicate = predicate.Or(s => s.interest_id == 4);
                        break;
                    case "Nature and Parks":
                        predicate = predicate.Or(s => s.interest_id == 3);
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
                                  interestLat = (double)ia.location_lat,
                                  interestLng = (double)ia.location_lng,
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

        private double Haversine(double lat1, double lat2, double lon1, double lon2)
        {
            const double r = 6371; // meters

            var sdlat = Math.Sin((lat2 - lat1) / 2);
            var sdlon = Math.Sin((lon2 - lon1) / 2);
            var q = sdlat * sdlat + Math.Cos(lat1) * Math.Cos(lat2) * sdlon * sdlon;
            var d = 2 * r * Math.Asin(Math.Sqrt(q));

            return d;
        }

        public ActionResult doTheDew()
        {
            string combinedString = "april,may|Hiking Trails";
            string stateId = "VIC";
            var filteredFarms = getAllFilteredFarms(combinedString);
            var farmsInAState = filteredFarms.Where(x => x.stateName.Equals(stateId)).ToList();
            var filteredInterests = getAllInterest2(combinedString);
            var interestsInAState = filteredInterests.Where(x => x.stateName.Equals(stateId)).ToList();
            Dictionary<FilteredFarmViewModel, List<InterestWithDistance>> dictionary = new Dictionary<FilteredFarmViewModel, List<InterestWithDistance>>();
            foreach (FilteredFarmViewModel farm in farmsInAState)
            {
                foreach (AllInterest interest in interestsInAState) {
                    double distance = Math.Round(this.Haversine(interest.interestLat, farm.farm_lat, interest.interestLng, farm.farm_lng),2);
                    if (distance < 6000) {
                        InterestWithDistance intDistance = new InterestWithDistance()
                        {
                            stateName = interest.stateName,
                            interestId = interest.interestId,
                            interestType = interest.interestType,
                            interestLat = interest.interestLat,
                            interestLng = interest.interestLng,
                            attractionId = interest.attractionId,
                            attractionName = interest.attractionName,
                            suburbId = interest.suburbId,
                            suburbName = interest.suburbName,
                            distance = distance
                        };
                        if (dictionary.ContainsKey(farm))
                        {
                            List<InterestWithDistance> existingList = dictionary[farm];
                            existingList.Add(intDistance);
                            dictionary[farm] = existingList;
                        }
                        else {
                            List<InterestWithDistance> newList = new List<InterestWithDistance>();
                            newList.Add(intDistance);
                            dictionary.Add(farm, newList);
                        }
                    }
                }
            }
            DictionaryView dict = new DictionaryView
            {
                farmDictionary = dictionary
            };
            return View(dict);
           
        }

        //public ActionResult getFarmCombinedInterest() {
        //    string combinedString = "april,may|Hiking Trails";
        //    var farms = dbContext.farms.ToList();
        //    var harvestList = dbContext.suburb_harvest.ToList();
        //    var newHarvestList = harvestList.AsQueryable().Where(this.FilterFarms(combinedString));
        //    var interestAttractions = dbContext.interest_attraction.ToList();
        //    var filteredInterestAttr = interestAttractions.AsQueryable().Where(this.buildPredForInterestType(combinedString));
        //    var interests = dbContext.interest_table.ToList();
        //    var suburbs = dbContext.suburb_table.ToList();
        //    var states = dbContext.states.ToList();
        //    var combinedWithSuburb = from f in farms
        //                             join sb in suburbs on f.suburb_id equals sb.suburb_id
        //                             join ia in interestAttractions on sb.suburb_id equals ia.suburb_id
        //                             join i in interests on ia.interest_id equals i.interest_id
        //                             join nh in newHarvestList on sb.suburb_id equals nh.suburb_id
        //                             select new FarmSuburbInterestCombined
        //                             {
        //                                 farmId = f.farm_id,
        //                                 farmName = f.farm_name,
        //                                 suburbId = sb.suburb_id,
        //                                 suburbName = sb.suburb_name,
        //                                 interestId = i.interest_id,
        //                                 attractionId = ia.attraction_id,
        //                                 stateId = sb.state
        //                             };
        //    var groupedFarms = combinedWithSuburb.GroupBy(x => x.stateId).Select(c => new StateFarmsCount { stateName = c.Key, numberOfFarms = c.Count() });
        //    var allInterests = getAllInterest2(combinedString);
            //var joinedLocation = from gf in groupedFarms
            //                     join gi in groupedInterests on gf.stateName equals gi.stateName
            //                     join st in states on gi.stateName equals st.state_id
            //                     select new CountPerState
            //                     {
            //                         stateName = st.state_id,
            //                         numberOfFarms = gf.numberOfFarms,
            //                         numberOfInterests = gi.numberOfInterests,
            //                         location_lat = (double)st.state_lat,
            //                         location_lng = (double)st.state_lng
            //                     };
            //return View(groupedInterests);
        //}

        public JsonResult getFarmCombinedInterest(string combinedString)
        {
            //string combinedString = "april,may|Hiking Trails";
            var allFarms = getAllFilteredFarms(combinedString);
            var allInterests = getAllInterest2(combinedString);
            var combinedWithSuburb = from f in allFarms
                                     join interest in allInterests on f.suburbId equals interest.suburbId
                                     select new FarmSuburbInterestCombined
                                     {
                                         farmId = f.farmId,
                                         farmName = f.farmName,
                                         suburbId = f.suburbId,
                                         suburbName = f.suburbName,
                                         interestId = interest.interestId,
                                         attractionId = interest.attractionId,
                                         stateId = f.stateName
                                     };
            var groupedFarms = combinedWithSuburb.GroupBy(x => x.stateId).Select(c => new StateFarmsCount { stateName = c.Key, numberOfFarms = c.Count() });
            var groupedInterests = groupInterestByState(combinedString);
            var states = dbContext.states.ToList();
            var joinedLocation = from gf in groupedFarms
                                 join gi in groupedInterests on gf.stateName equals gi.stateName
                                 join st in states on gi.stateName equals st.state_id
                                 select new CountPerState
                                 {
                                     stateName = st.state_id,
                                     numberOfFarms = gf.numberOfFarms,
                                     numberOfInterests = gi.numberOfInterests,
                                     location_lat = (double)st.state_lat,
                                     location_lng = (double)st.state_lng
                                 };

            return Json(joinedLocation, JsonRequestBehavior.AllowGet);
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