using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TripNPick.Models
{
    public class ExpInterest
    {
        public int Interest_place_ID { get; set; }
        public string place_of_interest { get; set; }
        public double rating { get; set; }
        public string interest_type { get; set; }
    }

    public class ExpInterestList
    {
        public List<ExpInterest> a { get; set; }
    }
}
