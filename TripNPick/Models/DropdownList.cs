using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TripNPick.Models
{
    public class States
    {
        public int statesID { get; set; }
        public string statesShortName { get; set; }
        public string statesName { get; set; }
        public bool ischecked { get; set; }
    }

    public class Interests
    {
        public int interestsID { get; set; }
        public string interestsName { get; set; }
        public bool ischecked { get; set; }
    }

    public class DropdownList
    {
        //public List<States> states { get; set; }
        public List<Interests> interests { get; set; }
    }
}