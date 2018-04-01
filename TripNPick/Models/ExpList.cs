using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TripNPick.Models
{
    public class ExpList
    {
        public ExpFarmList farms { get; set; }
        public ExpInterestList intersts { get; set; }
        public ExpWeatherList weathers { get; set; }
        public ExpHarvestList harvests { get; set; }
    }
}