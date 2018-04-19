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

    public class NumberOfFarmPerState {
        public string stateName { get; set; }
        public int numberOfFarms { get; set; }
        public double location_lat { get; set; }
        public double location_lng { get; set; }
    }
}