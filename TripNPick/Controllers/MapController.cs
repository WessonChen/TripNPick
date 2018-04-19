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
                        us.combinedString = us.combinedString + cInterests[i].ToLower();
                    }
                    else
                    {
                        us.combinedString = us.combinedString + cInterests[i].ToLower() + ",";
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

        public ActionResult Map()
        {
            return View();
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
        public ActionResult FullMap()
        {
            var farmList = dbContext.farms.ToList();
            return View(farmList);
        }

        public ActionResult MapTest()
        {
            return View();
        }
        public ActionResult FarmsIndex()
        {
            this.GetAllLocations();
            var farmList = dbContext.farms.ToList();
            return View(farmList);
        }

        public Expression<Func<suburb_harvest, bool>> FilterFarms(string combinedString)
        {
            Debug.WriteLine(combinedString);
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
            if (p[1].Equals("null"))
            {

            }
            else
            {
                interests = p[1].Split(',').ToList();
            }

            foreach (var m in months)
            {
                Debug.WriteLine(m);
            }
            foreach (var i in interests)
            {
                Debug.WriteLine(i);
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
        public ActionResult MapSVG()
        {
            return View();
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
                                          suburbName = sl.suburb_name,
                                          stateName = sl.state,
                                          farm_lat = (double)f.location_lat,
                                          farm_lng = (double)f.location_lng,
                                          farm_address = f.farm_address,
                                          farm_rating = f.farm_rating
                                      }).ToList();
            var getDistinctFarms = farmCountViewModel.DistinctBy(x => x.farmName);
            return getDistinctFarms;
        }

        public JsonResult getFarmCountMonthlyFiltered(string combinedString)
        {
            var states = dbContext.states.ToList();
            var distinctFarms = getAllFilteredFarms(combinedString);
            var farmsGroupedByState = distinctFarms.GroupBy(x => x.stateName).Select(c => new StateFarmsCount { stateName = c.Key, numberOfFarms = c.Count() });
            var joinStateLocation = from st in states
                                    join fg in farmsGroupedByState on st.state_id equals fg.stateName
                                    select new NumberOfFarmPerState
                                    {
                                        stateName = fg.stateName,
                                        numberOfFarms = fg.numberOfFarms,
                                        location_lat = (double)st.state_lat,
                                        location_lng = (double)st.state_lng
                                    };
            return Json(joinStateLocation, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult displayFilteredFarms()
        //{
        //    var distinctFarms = getAllFilteredFarms();
        //    return View(distinctFarms);
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
        public ActionResult TestMarkers()
        {
            return View();
        }
    }
}