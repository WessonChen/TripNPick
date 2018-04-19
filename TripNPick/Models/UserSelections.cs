using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TripNPick.Models
{
    public class UserSelections
    {
        public string[] cMonths { get; set; }
        public string[] cInterests { get; set; }
        public string combinedString { get; set; }
    }
}