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

    public class SuburbInterestsCount {
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

    public class DictionaryView {
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

    public class StateCount
    {
        public string stateName { get; set; }
        public int farmCounts { get; set; }
        public int interestCounts { get; set; }
        public double state_lat { get; set; }
        public double state_lng { get; set; }
    }
}