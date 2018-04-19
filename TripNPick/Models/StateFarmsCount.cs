using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TripNPick.Models
{
    public class StateFarmsCount
    {
        public string stateName { get; set; }
        public int numberOfFarms { get; set; }
    }

    public class StateInterestsCount {
       public string stateName { get; set; }
        public int numberOfInterests { get; set; }
    }

    public class NumberOfFarmPerState {
        public string stateName { get; set; }
        public int numberOfFarms { get; set; }
        public double location_lat { get; set; }
        public double location_lng { get; set; }
    }

    public class AllInterest {
        public string stateName { get; set; }
        public int interestId { get; set; }
        public string interestType { get; set; }
        public int attractionId { get; set; }
        public string attractionName { get; set; }
        public int suburbId { get; set; }
        public string suburbName { get; set; }
    }
}