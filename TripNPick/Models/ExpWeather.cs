using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TripNPick.Models
{
    public class ExpWeather
    {
        public int stationID { get; set; }
        public string statistic_element { get; set; }
        public double January { get; set; }
        public double February { get; set; }
        public double March { get; set; }
        public double April { get; set; }
        public double May { get; set; }
        public double June { get; set; }
        public double July { get; set; }
        public double August { get; set; }
        public double September { get; set; }
        public double October { get; set; }
        public double November { get; set; }
        public double December { get; set; }
    }

    public class ExpWeatherList
    {
        public List<ExpWeather> a { get; set; }
    }
}