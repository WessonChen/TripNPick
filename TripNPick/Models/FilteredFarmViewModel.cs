using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TripNPick.Models
{
    public class FilteredFarmViewModel
    {
        public string farmName { get; set; }
        public string farmId { get; set; }
        public string suburbName { get; set; }
        public string stateName { get; set; }
        public double farm_lat { get; set; }
        public double farm_lng { get; set; }
        public string farm_address { get; set; }
        public string farm_rating { get; set; }
        public double state_lat { get; set; }
        public double state_lng { get; set; }

    }
}