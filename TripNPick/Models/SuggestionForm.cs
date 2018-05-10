using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace TripNPick.Models
{
    public class SuggestionForm
    {
        public string userFirstName { get; set; }

        public string userLastName { get; set; }

        [Required(ErrorMessage = "Your Email is required"), EmailAddress(ErrorMessage ="Please provide a valid email address.")]
        public string userEmail { get; set; }

        [Required(ErrorMessage = "Your Suggestion is required")]
        public string userSuggestion { get; set; }
    }
}