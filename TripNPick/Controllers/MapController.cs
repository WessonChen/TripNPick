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
using Highsoft.Web.Mvc.Charts;

namespace TripNPick.Controllers
{
    public class MapController : Controller
    {
        ColdspotDBEntities dbContext = new ColdspotDBEntities();

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

        [HttpPost]
        public ActionResult helperMethod(Pairs aPair) {
            var farm2 = aPair.farm.farmId;
            Debug.WriteLine(farm2);
            TempData["pairModel"] = aPair;
            //ViewBag.reqModel = aPair;
            return RedirectToAction("createTable", "Map");
            //this.createTable(aPair);
            //return Json(aPair, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FarmDetailsTest1() {

            return View();
        }

        //public ActionResult createTable() {
        //    return View();
        //}

        public ActionResult createTable(string farmInfo)
        {
            //var pair = TempData["pairModel"] as Pairs;
            //var farm2 = pair.farm.farmId;
            //string farmId = pair.farm.farmId;
            Debug.WriteLine(farmInfo);
            var farmList = dbContext.farms.ToList();
            var reqSub = from f in farmList where f.farm_id.Equals(farmInfo) select f.suburb_id;
            var suburbId = Convert.ToInt32(reqSub.FirstOrDefault());
            Debug.WriteLine(Convert.ToInt32(reqSub.FirstOrDefault()));
            var suburbList = dbContext.suburb_table.ToList();
            var reqStation = from s in suburbList where s.suburb_id.Equals(suburbId) select s.station_id;
            Debug.WriteLine(Convert.ToInt32(reqStation.FirstOrDefault()));
            var stationId = Convert.ToInt32(reqStation.FirstOrDefault());
            var coldView = getColdDays(stationId);
            var hotView = getHotDays(stationId);
            var rainView = getRainyDays(stationId);
            var temp3View = getTemp3pm(stationId);
            var temp9View = getTemp9am(stationId);

            List<double> coldValues = this.getThatList(coldView.First()).getDays();
            List<double> hotValues = this.getThatList(hotView.First()).getDays();
            List<double> rainValues = this.getThatList(rainView.First()).getDays();
            List<double> temp3Values = this.getThatList(temp3View.First()).getDays();
            List<double> temp9Values = this.getThatList(temp9View.First()).getDays();

            List<LineSeriesData> coldData = new List<LineSeriesData>();
            List<LineSeriesData> hotData = new List<LineSeriesData>();
            List<LineSeriesData> rainData = new List<LineSeriesData>();
            List<LineSeriesData> temp3Data = new List<LineSeriesData>();
            List<LineSeriesData> temp9Data = new List<LineSeriesData>();

            coldValues.ForEach(p => coldData.Add(new LineSeriesData { Y = p }));
            hotValues.ForEach(p => hotData.Add(new LineSeriesData { Y = p }));
            rainValues.ForEach(p => rainData.Add(new LineSeriesData { Y = p }));
            temp3Values.ForEach(p => temp3Data.Add(new LineSeriesData { Y = p }));
            temp9Values.ForEach(p => temp9Data.Add(new LineSeriesData { Y = p }));

            FarmDetailsView twoModels = new FarmDetailsView();
            twoModels.weatherList = new List<WeatherView>();
            twoModels.weatherList.Add(coldView.First());
            twoModels.weatherList.Add(hotView.First());
            twoModels.weatherList.Add(rainView.First());
            twoModels.weatherList.Add(temp3View.First());
            twoModels.weatherList.Add(temp9View.First());

            var demandView = getFarmDemands(suburbId);
            twoModels.demandList = demandView;
            //twoModels.newPair = pair;

            ViewData["coldData"] = coldData;
            ViewData["hotData"] = hotData;
            ViewData["rainData"] = rainData;
            ViewData["temp3Data"] = temp3Data;
            ViewData["temp9Data"] = temp9Data;


            return View(twoModels);
        }

        public WeatherDays getThatList(WeatherView weather) {
            WeatherDays weatherStruct = new WeatherDays();
            List<double> newList = new List<double>();
            newList.Add(Convert.ToDouble(weather.january));
            newList.Add(Convert.ToDouble(weather.february));
            newList.Add(Convert.ToDouble(weather.march));
            newList.Add(Convert.ToDouble(weather.april));
            newList.Add(Convert.ToDouble(weather.may));
            newList.Add(Convert.ToDouble(weather.june));
            newList.Add(Convert.ToDouble(weather.july));
            newList.Add(Convert.ToDouble(weather.august));
            newList.Add(Convert.ToDouble(weather.september));
            newList.Add(Convert.ToDouble(weather.october));
            newList.Add(Convert.ToDouble(weather.november));
            newList.Add(Convert.ToDouble(weather.december));
            weatherStruct.setDays(newList);
            weatherStruct.setFeature(weather.feature);
            return weatherStruct;
     }

        public IEnumerable<DemandView> getFarmDemands(int suburbId) {
            var harverstList = dbContext.suburb_harvest.ToList();
            var cropList = dbContext.crops.ToList();
            var reqHarvest = from h in harverstList
                             join c in cropList on h.crop_id equals c.crop_id
                             where h.suburb_id == suburbId
                             select new DemandView
                             {
                                 cropName = c.crop_name,
                                 january = formatDemandString(h.january),
                                 february = formatDemandString(h.february),
                                 march = formatDemandString(h.march),
                                 april = formatDemandString(h.april),
                                 may = formatDemandString(h.may),
                                 june = formatDemandString(h.june),
                                 july = formatDemandString(h.july),
                                 august = formatDemandString(h.august),
                                 september = formatDemandString(h.september),
                                 october = formatDemandString(h.october),
                                 november = formatDemandString(h.november),
                                 december = formatDemandString(h.december)
                             };
            return reqHarvest;
        }

        public string formatDemandString(string demandLevel) {
            if (demandLevel.Equals("NULL")) {
                return "";
            }
            else
            {
                return demandLevel;
            }
            
        }

        public IEnumerable<WeatherView> getColdDays(int stationId)
        {
            var coldList = dbContext.weather_cold_days.ToList();
            var coldView = from f in coldList
                           where f.station_id == stationId
                           select new WeatherView
                           {
                               feature = "Number of cold days",
                               january = formatString(f.january),
                               february = formatString(f.february),
                               march = formatString(f.march),
                               april = formatString(f.april),
                               may = formatString(f.may),
                               june = formatString(f.june),
                               july = formatString(f.july),
                               august = formatString(f.august),
                               september = formatString(f.september),
                               october = formatString(f.october),
                               november = formatString(f.november),
                               december = formatString(f.december)

                           };
            return coldView;
        }

        public IEnumerable<WeatherView> getTemp9am(int stationId)
        {
            var temp9List = dbContext.weather_temp9am_days.ToList();
            var temp9View = from f in temp9List
                            where f.station_id == stationId
                            select new WeatherView
                            {
                                feature = "Temparature at 9 am (C)",
                                january = formatString(f.january),
                                february = formatString(f.february),
                                march = formatString(f.march),
                                april = formatString(f.april),
                                may = formatString(f.may),
                                june = formatString(f.june),
                                july = formatString(f.july),
                                august = formatString(f.august),
                                september = formatString(f.september),
                                october = formatString(f.october),
                                november = formatString(f.november),
                                december = formatString(f.december)

                            };
            return temp9View;
        }

        public IEnumerable<WeatherView> getTemp3pm(int stationId)
        {
            var temp3List = dbContext.weather_temp3pm_days.ToList();
            var temp3View = from f in temp3List
                           where f.station_id == stationId
                           select new WeatherView
                           {
                               feature = "Temparature at 3 pm (C)",
                               january = formatString(f.january),
                               february = formatString(f.february),
                               march = formatString(f.march),
                               april = formatString(f.april),
                               may = formatString(f.may),
                               june = formatString(f.june),
                               july = formatString(f.july),
                               august = formatString(f.august),
                               september = formatString(f.september),
                               october = formatString(f.october),
                               november = formatString(f.november),
                               december = formatString(f.december)

                           };
            return temp3View;
        }

        public IEnumerable<WeatherView> getRainyDays(int stationId)
        {
            var rainyList = dbContext.weather_rainy_days.ToList();
            var rainView = from f in rainyList
                           where f.station_id == stationId
                          select new WeatherView
                          {
                              feature = "Number of rainy days",
                              january = formatString(f.january),
                              february = formatString(f.february),
                              march = formatString(f.march),
                              april = formatString(f.april),
                              may = formatString(f.may),
                              june = formatString(f.june),
                              july = formatString(f.july),
                              august = formatString(f.august),
                              september = formatString(f.september),
                              october = formatString(f.october),
                              november = formatString(f.november),
                              december = formatString(f.december)

                          };
            return rainView;
        }

        public IEnumerable<WeatherView> getHotDays(int stationId)
        {
            var hotList = dbContext.weather_hot_days.ToList();
            var hotView = from f in hotList
                          where f.station_id == stationId
                          select new WeatherView
                          {
                              feature = "Number of hot days",
                              january = formatString(f.january),
                              february = formatString(f.february),
                              march = formatString(f.march),
                              april = formatString(f.april),
                              may = formatString(f.may),
                              june = formatString(f.june),
                              july = formatString(f.july),
                              august = formatString(f.august),
                              september = formatString(f.september),
                              october = formatString(f.october),
                              november = formatString(f.november),
                              december = formatString(f.december)

                          };
            return hotView;
        }

        public string formatString(string number) {

            if (String.IsNullOrEmpty(number))
            {
                return "0";
            }
            else
            {
                double some =  Convert.ToDouble(number);
                double precised = System.Math.Round(some, 2);
                string formatted = System.Convert.ToString(precised);
                return formatted;
            }
        }
        
        public ActionResult TestIndex(string[] cMonths, string[] cInterests)
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
            Debug.WriteLine(predicate);
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
            //string combinedString = "april,june|Hiking Trails";
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

        public ActionResult testSomething() {
            string farm2 = "00f3d3fea7ea216489383b68e66779100b87d91b";
            var farmList = dbContext.farms.ToList();
            var reqSub = from f in farmList where f.farm_id.Equals(farm2) select f.suburb_id;
            var suburbId = Convert.ToInt16(reqSub.FirstOrDefault());
            Debug.WriteLine(Convert.ToInt16(reqSub.FirstOrDefault()));
            var suburbList = dbContext.suburb_table.ToList();
            var reqStation = from s in suburbList where s.suburb_id.Equals(suburbId) select s.station_id;
            var stationId = Convert.ToInt16(reqStation.FirstOrDefault());
            var coldView = getColdDays(stationId);
            var hotView = getHotDays(stationId);
            var rainView = getRainyDays(stationId);
            var temp3View = getTemp3pm(stationId);
            var temp9View = getTemp9am(stationId);

            List<double> coldValues = this.getThatList(coldView.First()).getDays();
            List<double> hotValues = this.getThatList(hotView.First()).getDays();
            List<double> rainValues = this.getThatList(rainView.First()).getDays();
            List<double> temp3Values = this.getThatList(temp3View.First()).getDays();
            List<double> temp9Values = this.getThatList(temp9View.First()).getDays();

            List<LineSeriesData> coldData = new List<LineSeriesData>();
            List<LineSeriesData> hotData = new List<LineSeriesData>();
            List<LineSeriesData> rainData = new List<LineSeriesData>();
            List<LineSeriesData> temp3Data = new List<LineSeriesData>();
            List<LineSeriesData> temp9Data = new List<LineSeriesData>();

            coldValues.ForEach(p => coldData.Add(new LineSeriesData { Y = p }));
            hotValues.ForEach(p => hotData.Add(new LineSeriesData { Y = p }));
            rainValues.ForEach(p => rainData.Add(new LineSeriesData { Y = p }));
            temp3Values.ForEach(p => temp3Data.Add(new LineSeriesData { Y = p }));
            temp9Values.ForEach(p => temp9Data.Add(new LineSeriesData { Y = p }));

            FarmDetailsView twoModels = new FarmDetailsView();
            twoModels.weatherList = new List<WeatherView>();
            twoModels.weatherList.Add(coldView.First());
            twoModels.weatherList.Add(hotView.First());
            twoModels.weatherList.Add(rainView.First());
            twoModels.weatherList.Add(temp3View.First());
            twoModels.weatherList.Add(temp9View.First());

            var demandView = getFarmDemands(suburbId);
            twoModels.demandList = demandView;

            ViewData["coldData"] = coldData;
            ViewData["hotData"] = hotData;
            ViewData["rainData"] = rainData;
            ViewData["temp3Data"] = temp3Data;
            ViewData["temp9Data"] = temp9Data;


            return View(twoModels);
        }

        public Expression<Func<interest_attraction, bool>> buildPredForInterestType(string combinedString)
        {
            List<string> selectedInterests = new List<string>();
            var months = new List<string>();
            string[] p = combinedString.Split('|');
            if (p[1].Equals("null"))
            {
                selectedInterests.Add("Museums");
                selectedInterests.Add("Sights and Landmarks");
                selectedInterests.Add("Nature and Parks");
                selectedInterests.Add("Beaches");
                selectedInterests.Add("Outdoor Activities and Tours");
                selectedInterests.Add("Nature and Wildlife Areas");
                selectedInterests.Add("Hiking Trails");
                selectedInterests.Add("Fun and Games and Sports");
                selectedInterests.Add("Zoos and Aquariums");
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
                    case "Sights and Landmarks":
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
                    case "Nature and Wildlife Areas":
                        predicate = predicate.Or(s => s.interest_id == 7);
                        break;
                    case "Hiking Trails":
                        predicate = predicate.Or(s => s.interest_id == 8);
                        break;
                    case "Fun and Games and Sports":
                        predicate = predicate.Or(s => s.interest_id == 9);
                        break;
                    case "Zoos and Aquariums":
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
            const double r = 6371;

            var sdlat = Math.Sin((lat2 - lat1) / 2);
            var sdlon = Math.Sin((lon2 - lon1) / 2);
            var q = sdlat * sdlat + Math.Cos(lat1) * Math.Cos(lat2) * sdlon * sdlon;
            var d = 2 * r * Math.Asin(Math.Sqrt(q));

            return d;
        }

        public JsonResult doTheDew(string userInput)
        {
            Debug.WriteLine(userInput);
            string[] p = userInput.Split(':');
            var combinedString = p[0];
            var stateId = p[1];
            var filteredFarms = getAllFilteredFarms(combinedString);
            var farmsInAState = filteredFarms.Where(x => x.stateName.Equals(stateId)).ToList();
            var filteredInterests = getAllInterest2(combinedString);
            var interestsInAState = filteredInterests.Where(x => x.stateName.Equals(stateId)).ToList();
            List<Pairs> thelist = new List<Pairs>();
            foreach (FilteredFarmViewModel farm in farmsInAState)
            {
                foreach (AllInterest interest in interestsInAState) {
                    double distance = Math.Round(this.Haversine(interest.interestLat, farm.farm_lat, interest.interestLng, farm.farm_lng),2);
                    if (distance < 100) {
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
                        bool isContained = false;
                        int c = 0;
                        while (!isContained && c < thelist.Count())
                        {
                            if (thelist[c].farm == farm)
                            {
                                thelist[c].interests.Add(intDistance);
                                isContained = true;
                            }
                            c++;
                        }
                        if (!isContained)
                        {
                            Pairs newPair = new Pairs();
                            List<InterestWithDistance> newInterests = new List<InterestWithDistance>();
                            newPair.farm = farm;
                            newInterests.Add(intDistance);
                            newPair.interests = newInterests;
                            thelist.Add(newPair);
                        }
                    }
                }
            }
            return Json(thelist, JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult countForStates(string userInput)
        {
            var combinedString = userInput;
            List<string> stateList = new List<string>();
            stateList.Add("NT");
            stateList.Add("WA");
            stateList.Add("SA");
            stateList.Add("VIC");
            stateList.Add("NSW");
            stateList.Add("TAS");
            stateList.Add("QLD");
            List<StateCount> scList = new List<StateCount>();
            foreach (var stateId in stateList)
            {
                StateCount sc = new StateCount();
                var aState = dbContext.states.Where(x => x.state_id.Equals(stateId)).ToList();
                foreach (var s in aState)
                {
                    sc.state_lat = (double) s.state_lat;
                    sc.state_lng = (double) s.state_lng;
                }
                int sCount = 0;
                var filteredFarms = getAllFilteredFarms(combinedString);
                var farmsInAState = filteredFarms.Where(x => x.stateName.Equals(stateId)).ToList();
                var filteredInterests = getAllInterest2(combinedString);
                var interestsInAState = filteredInterests.Where(x => x.stateName.Equals(stateId)).ToList();
                List<Pairs> thelist = new List<Pairs>();
                foreach (FilteredFarmViewModel farm in farmsInAState)
                {
                    foreach (AllInterest interest in interestsInAState)
                    {
                        double distance = Math.Round(this.Haversine(interest.interestLat, farm.farm_lat, interest.interestLng, farm.farm_lng), 2);
                        if (distance < 100)
                        {
                            bool isContained = false;
                            int c = 0;
                            while (!isContained && c < thelist.Count())
                            {
                                if (thelist[c].farm == farm)
                                {
                                    isContained = true;
                                }
                                c++;
                            }
                            if (!isContained)
                            {
                                Pairs newPair = new Pairs();
                                List<InterestWithDistance> newInterests = new List<InterestWithDistance>();
                                newPair.farm = farm;
                                newPair.interests = newInterests;
                                thelist.Add(newPair);
                                sCount++;
                            }
                        }
                    }
                }
                var interestGroupedByState = groupInterestByState(combinedString);
                sc.stateName = stateId;
                sc.farmCounts = sCount;
                List<StateInterestsCount> iList = interestGroupedByState.ToList();
                foreach (var item in iList)
                {
                    if (item.stateName.Equals(stateId))
                    {
                        sc.interestCounts = item.numberOfInterests;
                    }
                }
                scList.Add(sc);
            }
            
            return Json(scList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult dropdownTest()
        {
            return View();
        }
    }
}