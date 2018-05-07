using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TripNPick.Models
{
    public class StateSuburbCount
    {
        public string stateName { get; set; }
        public int suburbCount { get; set; }
    }


    public class SuburbFarmsCount
    {
        public int suburbId { get; set; }
        public int numberOfFarms { get; set; }
    }

    public class SuburbInterestsCount
    {
        public int suburbId { get; set; }
        public int numberOfInterests { get; set; }
    }

    public class CountPerSuburb
    {
        public int suburbId { get; set; }
        public string suburbName { get; set; }
        public int numberOfInterests { get; set; }
        public int numberOfFarms { get; set; }
        public double suburbLat { get; set; }
        public double suburbLng { get; set; }
    }

    public class NumberOfFarmsPerSuburb
    {
        public int suburbId { get; set; }
        public string suburbName { get; set; }
        public double suburbLat { get; set; }
        public double suburbLng { get; set; }
    }

    public class DictionaryView
    {
        public Dictionary<FilteredFarmViewModel, List<InterestWithDistance>> farmDictionary { get; set; }
    }

    public class FarmSuburbInterestCombined
    {
        public string farmId { get; set; }
        public string farmName { get; set; }
        public int suburbId { get; set; }
        public string suburbName { get; set; }
        public int interestId { get; set; }
        public int attractionId { get; set; }
        public int stateId { get; set; }
    }

    public class Pairs
    {
        public FilteredFarmViewModel farm { get; set; }
        public List<InterestWithDistance> interests { get; set; }
    }

    public class MapInformationView
    {
        public List<Pairs> farmsAndInterests { get; set; }
        public List<HostelView> hostels { get; set; }
    }

    public class HostelView
    {
        public int hostedId { get; set; }
        public double hostel_lat { get; set; }
        public double hostel_lng { get; set; }
        public int suburbId { get; set; }
    }

    public class StateCount
    {
        public string stateName { get; set; }
        public int farmCounts { get; set; }
        public int interestCounts { get; set; }
        public double state_lat { get; set; }
        public double state_lng { get; set; }
    }

    public class WeatherView
    {
        public string feature { get; set; }
        public string january { get; set; }
        public string february { get; set; }
        public string march { get; set; }
        public string april { get; set; }
        public string may { get; set; }
        public string june { get; set; }
        public string july { get; set; }
        public string august { get; set; }
        public string september { get; set; }
        public string october { get; set; }
        public string november { get; set; }
        public string december { get; set; }
    }

    public class DemandView
    {
        public string cropName { get; set; }
        public string january { get; set; }
        public string february { get; set; }
        public string march { get; set; }
        public string april { get; set; }
        public string may { get; set; }
        public string june { get; set; }
        public string july { get; set; }
        public string august { get; set; }
        public string september { get; set; }
        public string october { get; set; }
        public string november { get; set; }
        public string december { get; set; }
    }

    public class FarmDetailsView
    {
        public List<WeatherView> weatherList { get; set; }
        public IEnumerable<DemandView> demandList { get; set; }
        public List<WeatherDays> chartListOne { get; set; }
        public farm theFarm { get; set; }
        public IEnumerable<distancePassView> nearbyAttractions { get; set; }
    }

    public class tempInterest
    {
        public int attractionId { get; set; }
        public string distance { get; set; }
    }

    public class distancePassView {
        public string attraction_name { get; set; }
        public string attraction_distance { get; set; }
        public double attraction_rating { get; set; }
        public string number_of_reviews { get; set; }
        public string attraction_address { get; set; }
        public string interest_type { get; set; }
    }

        public class WeatherDays
        {
            private string featureName;
            private List<double> numberOfDays;

            public WeatherDays()
            {
            }

            public WeatherDays(string feature, List<double> days)
            {
                this.featureName = feature;
                this.numberOfDays = days;
            }

            public string getFeature()
            {
                return this.featureName;
            }
            public List<double> getDays()
            {
                return this.numberOfDays;
            }
            public void setFeature(string feature)
            {
                this.featureName = feature;
            }
            public void setDays(List<double> days)
            {
                this.numberOfDays = days;
            }
        }
    }
