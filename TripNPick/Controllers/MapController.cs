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
        /**
         * Create a context which connects to the database
         */
        ColdSpotDBEntities dbContext = new ColdSpotDBEntities();
        /*
         * Return the Farm Details Page along the model "FarmDetailsView"
         */
        public ActionResult FarmDetails(string farmInfo)
        {
            var farmList = dbContext.farms.ToList();
            if (farmInfo == null || farmInfo.Equals(""))
            {
                return RedirectToAction("ErrorMessage", "ErrorPage");
            }
            string[] infos = farmInfo.Split(':');
            string reqFarmId = infos[0];

            List<string> attractionIdList = new List<string>();
            List<string> distanceList = new List<string>();
            for (int i = 1; i < infos.Count(); i++)
            {
                string[] pair = infos[i].Split(',');
                attractionIdList.Add(pair[0]);
                distanceList.Add(pair[1]);
            }

            var combinedList = new List<tempInterest>();
            for (int i = 0; i < attractionIdList.Count(); i++) {
                var newList = new tempInterest();
                newList.attractionId = Convert.ToInt32(attractionIdList[i]);
                newList.distance = distanceList[i];
                combinedList.Add(newList);
            }

            var allAttractionList = dbContext.interest_attraction.ToList();
            var interestTypes = dbContext.interest_table.ToList();
            List<interest_attraction> nearbyInterests = new List<interest_attraction>();
            foreach (string attrId in attractionIdList) {
                var newId = Convert.ToInt32(attrId);
                var oneAttraction = allAttractionList.Where(x => x.attraction_id == newId).FirstOrDefault();
                nearbyInterests.Add(oneAttraction);
            }

            Dictionary<string, string> interestDict = new Dictionary<string, string>();
            interestDict.Add("1", "Museum");
            interestDict.Add("2", "Sights");
            interestDict.Add("3", "Parks");
            interestDict.Add("4", "Sights");
            interestDict.Add("5", "Beach");
            interestDict.Add("6", "Outdoors");
            interestDict.Add("7", "Wildlife");
            interestDict.Add("8", "Hiking");
            interestDict.Add("9", "Sports");
            interestDict.Add("10", "Zoo");

            var distantInfo = from c in combinedList
                              join n in nearbyInterests on c.attractionId equals n.attraction_id
                              join i in interestTypes on n.interest_id equals i.interest_id
                              select new distancePassView
                              {
                                  attraction_name = n.attraction_name,
                                  attraction_address = n.address_x,
                                  attraction_distance = c.distance,
                                  attraction_rating = Convert.ToDouble(n.review_grade),
                                  interest_type = "~/img/markers/" + interestDict[Convert.ToString(i.interest_id)] + ".png",
                                  number_of_reviews = Convert.ToDouble(n.number_of_reviews),
                                  toolTip = interestDict[Convert.ToString(i.interest_id)]
                              };
            var currentFarm = farmList.Where(x => x.farm_id == reqFarmId).FirstOrDefault();
            var reqSub = from f in farmList where f.farm_id.Equals(infos[0]) select f.suburb_id;
            var suburbId = Convert.ToInt32(reqSub.FirstOrDefault());
            var suburbList = dbContext.suburb_table.ToList();
            var reqStation = from s in suburbList where s.suburb_id.Equals(suburbId) select s.station_id;
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
            twoModels.theFarm = currentFarm;
            twoModels.nearbyAttractions = distantInfo;
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

        /*
         * return the model object "WeatherDays"
         */
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

        /*
         * returns an ienumerable object of "DemandView" which provides the demand details of the farm.
         */
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

        /*
         * Checks if the data in the demand details of the farm has "NULL" in the database. 
         */
        public string formatDemandString(string demandLevel) {
            if (demandLevel.Equals("NULL"))
            {
                return "";
            }
            else {
                return demandLevel;
            }
        }

        /*
         * returns an ienumerable object of "WeatherView which states the number of cold days in a particular farm throught the year"
         */
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

        /*
        * returns an ienumerable object of "WeatherView which states the temperature at 9 am in a particular farm throught the year"
        */
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

        /*
         * returns an ienumerable object of "WeatherView which states the temperature at 3 pm in a particular farm throught the year"
        */
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

        /*
        * returns an ienumerable object of "WeatherView which states the number of rainy days in a particular farm throught the year"
        */
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

        /*
        * returns an ienumerable object of "WeatherView which states the number of hot days in a particular farm throught the year"
         */
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
        /*
         * format the provided string to a desired way according to the nature of the input
         */
        public string formatString(string number) {

            if (String.IsNullOrEmpty(number))
            {
                return "0";
            }
            else
            {
                double some =  Convert.ToDouble(number);
                double precised = Math.Round(some, 2);
                string formatted = Convert.ToString(precised);
                return formatted;
            }
        }
        /*
         * returns the MapPage depending upon the user selection
         */
        public ActionResult MapPage(string[] cMonths, string[] cInterests, string[] cDistance)
        {
            UserSelections us = new UserSelections();
            us.cMonths = cMonths;
            us.cInterests = cInterests;
            us.cDistance = cDistance;
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
                DateTime now = DateTime.Now;
                string currentM = now.ToString("MMMM");
                List<string> months = new List<string>();
                months.Add(currentM);
                us.cMonths = months.ToArray();
                us.combinedString = currentM + "|";
            }
            if (cInterests != null)
            {
                for (int i = 0; i < cInterests.Length; i++)
                {
                    if (i == cInterests.Length - 1)
                    {
                        us.combinedString = us.combinedString + cInterests[i] + "|";
                    }
                    else
                    {
                        us.combinedString = us.combinedString + cInterests[i] + ",";
                    }
                }
            }
            else
            {
                List<string> ints = new List<string>();
                ints.Add("Museums");
                ints.Add("Sights and Landmarks");
                ints.Add("Nature and Parks");
                ints.Add("Beaches");
                ints.Add("Outdoor Activities and Tours");
                ints.Add("Nature and Wildlife Areas");
                ints.Add("Hiking Trails");
                ints.Add("Fun and Games and Sports");
                ints.Add("Zoos and Aquariums");
                us.cInterests = ints.ToArray();
                us.combinedString = us.combinedString + "null|";
            }
            if (cDistance != null && !cDistance.Equals(""))
            {
                us.combinedString = us.combinedString + cDistance[0];
            }
            else
            {
                List<string> dis = new List<string>();
                dis.Add("100");
                us.cDistance = dis.ToArray();
                us.combinedString = us.combinedString + 100;
            }
            return View(us);
        }

        /*
         * returns a linq query expression depending upon the user selection
         */
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

        /*
         * returns an ienumerable object of FilteredFarmViewModel which contents all the farms having medium or high demand during the months user has selected 
         */
        public IEnumerable<FilteredFarmViewModel> getAllFilteredFarms(string combinedString)
        {
            //var some = dbContext.farms.ToList();
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
                                          farm_rating = Convert.ToDouble(f.farm_rating),
                                          suburb_lat = (double)sl.suburb_lat,
                                          suburb_lng = (double)sl.suburb_lng
                                      }).ToList();
            var getDistinctFarms = farmCountViewModel.DistinctBy(x => x.farmName);
            return getDistinctFarms;
        }
        /*
         * returns an expression of interest_attraction which is build to query on the basis of interest type.
         */
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

        /*
         * returns an ienumerable object of "Allinterest" depending on the basis of user selection
         */
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
                                  attractionAddress = ia.address_x,
                                  interestLat = (double)ia.location_lat,
                                  interestLng = (double)ia.location_lng,
                                  interestRating = (double)ia.review_grade,
                                  numberOfReviews = Convert.ToInt32(ia.number_of_reviews),
                                  suburbId = sb.suburb_id,
                                  suburbName = sb.suburb_name
                              };
            return allInterest;
        }

        /*
         * returns an ienumerable object of SuburbInterestsCount which is a group of interests in a state.
         */
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
                                  suburbName = sb.suburb_name,
                                  attractionAddress = ia.address_x,
                                  interestRating = (double)ia.review_grade,
                                  numberOfReviews = Convert.ToInt32(ia.number_of_reviews)
                              };
            var interestsGrouped = allInterest.GroupBy(x => x.suburbId).Select(c => new SuburbInterestsCount { suburbId = c.Key, numberOfInterests = c.Count() });
            return interestsGrouped;
        }

        /*
         * returns an iqueryable object of SuburbFarmsCount which is a model that provides the number of farms in a suburb
         */
        public IQueryable<SuburbFarmsCount> groupFarmsBySuburb(string combinedString, string stateName)
        {
            var allFilteredFarms = getAllFilteredFarms(combinedString).ToList();
            var farmsInState = allFilteredFarms.AsQueryable().Where(x => x.stateName.Equals(stateName));
            var groupFarmsBySuburbs = farmsInState.GroupBy(x => x.suburbId).Select(c => new SuburbFarmsCount { suburbId = c.Key, numberOfFarms = c.Count() });
            return groupFarmsBySuburbs;
        }

        /*
         * returns a distance between two points depending on their latitude and longitude
         */
        private double Haversine(double lat1, double lat2, double lon1, double lon2)
        {
            const double r = 6371;

            var sdlat = Math.Sin((lat2 - lat1) / 2);
            var sdlon = Math.Sin((lon2 - lon1) / 2);
            var q = sdlat * sdlat + Math.Cos(lat1) * Math.Cos(lat2) * sdlon * sdlon;
            var d = 2 * r * Math.Asin(Math.Sqrt(q));

            return d;
        }

        /*
         * returns the final desired farms, nearby interests and hostels information in a state depending upon user selection
         */
        public JsonResult doTheDew(string userInput)
        {
            string[] p = userInput.Split(':');
            var combinedString = p[0];
            var stateId = p[1];
            string[] o = combinedString.Split('|');
            string distancex = o[2];
            combinedString = o[0].ToLower() + "|" + o[1];
            var filteredFarms = getAllFilteredFarms(combinedString);
            var farmsInAState = filteredFarms.Where(x => x.stateName.Equals(stateId)).ToList();
            var filteredInterests = getAllInterest2(combinedString);
            var interestsInAState = filteredInterests.Where(x => x.stateName.Equals(stateId)).ToList();
            List<Pairs> thelist = new List<Pairs>();
            foreach (FilteredFarmViewModel farm in farmsInAState)
            {
                foreach (AllInterest interest in interestsInAState) {
                    double distance = Math.Round(this.Haversine(interest.interestLat, farm.farm_lat, interest.interestLng, farm.farm_lng),2);
                    if (distance < Convert.ToInt32(distancex)) {
                        InterestWithDistance intDistance = new InterestWithDistance()
                        {
                            stateName = interest.stateName,
                            interestId = interest.interestId,
                            interestType = interest.interestType,
                            interestLat = interest.interestLat,
                            interestLng = interest.interestLng,
                            attractionId = interest.attractionId,
                            attractionName = interest.attractionName,
                            attractionAddress = interest.attractionAddress,
                            suburbId = interest.suburbId,
                            suburbName = interest.suburbName,
                            attractionRating = interest.interestRating,
                            attractionNumberOfReviews = interest.numberOfReviews,
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
            var mapInfo = new MapInformationView();
            mapInfo.farmsAndInterests = thelist;
            var selectedfarms = thelist.Select(x => x.farm).ToList();
            var hostellist = dbContext.hostels.ToList();
            var suburblist = dbContext.suburb_table.ToList();
            var nearbyHostels = from f in selectedfarms
                                join s in suburblist on f.suburbId equals s.suburb_id
                                join h in hostellist on s.suburb_id equals h.suburb_id
                                select new HostelView
                                {
                                    hostelId = h.hostel_id,
                                    hostelName = h.hostel_name,
                                    hostelAddress = h.hostel_address,
                                    hostel_lat = Convert.ToDouble(h.hostel_lat),
                                    hostel_lng = Convert.ToDouble(h.hostel_long),
                                    suburbId = Convert.ToInt32( h.suburb_id),
                                    hostelRating = Convert.ToDouble(h.hostel_rating)
                                };
            var reqHostels = nearbyHostels.ToList();
            mapInfo.hostels = reqHostels;

            return Json(mapInfo, JsonRequestBehavior.AllowGet);
        }
    }
}