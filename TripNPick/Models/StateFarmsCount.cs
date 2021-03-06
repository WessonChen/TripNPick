﻿using System;
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


    public class CountPerState {
        public string stateName { get; set; }
        public int numberOfFarms { get; set; }
        public int numberOfInterests { get; set; }
        public double location_lat { get; set; }
        public double location_lng { get; set; }
    }


    public class AllInterest {
        public string stateName { get; set; }
        public int interestId { get; set; }
        public string interestType { get; set; }
        public int attractionId { get; set; }
        public string attractionName { get; set; }
        public string attractionAddress { get; set; }
        public double interestLat { get; set; }
        public double interestLng { get; set; }
        public double interestRating { get; set; }
        public int numberOfReviews { get; set; }
        public int suburbId { get; set; }
        public string suburbName { get; set; }
    }

    public class InterestWithDistance
    {
        public string stateName { get; set; }
        public int interestId { get; set; }
        public string interestType { get; set; }
        public int attractionId { get; set; }
        public string attractionName { get; set; }
        public string attractionAddress { get; set; }
        public double interestLat { get; set; }
        public double interestLng { get; set; }
        public int suburbId { get; set; }
        public string suburbName { get; set; }
        public double distance { get; set; }
        public double attractionRating { get; set; }
        public int attractionNumberOfReviews { get; set; }
    }
}