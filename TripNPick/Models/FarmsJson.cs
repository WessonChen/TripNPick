using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TripNPick.Models
{
    public class FarmsJson
    {
        public List<string> html_attributions { get; set; }
        public List<FarmResults> results { get; set; }
        public string status { get; set; }
    }

    public class FarmResults
    {
        public Geometry geometry { get; set; }
        public string icon { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public Opening_hours opening_hours { get; set; }
        public List<Photos> photos { get; set; }
        public string place_id { get; set; }
        public string rating { get; set; }
        public string reference { get; set; }
        public List<string> types { get; set; }
        public string vicinity { get; set; }
        public IntResults interest { get; set; }
    }
}