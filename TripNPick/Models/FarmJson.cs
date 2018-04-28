using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TripNPick.Models
{
    public class FarmJson
    {
        public string farm_id { get; set; }
        public string farm_name { get; set; }
        public Nullable<double> location_lat { get; set; }
        public Nullable<double> location_lng { get; set; }
        public string farm_address { get; set; }
        public string farm_rating { get; set; }
        public Nullable<int> suburb_id { get; set; }

        //public virtual suburb_table suburb_table { get; set; }
    }
}