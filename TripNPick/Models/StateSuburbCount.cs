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
}