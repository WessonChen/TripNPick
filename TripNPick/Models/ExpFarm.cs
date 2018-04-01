using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TripNPick.Models
{
    public class ExpFarm
    {
        public string farm_name { get; set; }
        public string address { get; set; }
        public int Interest_place_ID { get; set; }
        public int stationID { get; set; }
        public int suburbID { get; set; }
        public double rating { get; set; }
        public double distance { get; set; }
    }

    public class ExpFarmList
    {
        public List<ExpFarm> a { get; set; }
    }
}