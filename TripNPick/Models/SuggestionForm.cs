using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace TripNPick.Models
{
    public class SuggestionForm
    {
        [Required(ErrorMessage = "Your First Name is required")]
        public string userFirstName { get; set; }

        [Required(ErrorMessage = "Your Last Name is required")]
        public string userLastName { get; set; }

        [Required(ErrorMessage = "Your Email is required"), EmailAddress(ErrorMessage ="Please provide a valid email address.")]
        public string userEmail { get; set; }

        [Required(ErrorMessage = "Your Suggestion is required")]
        public string userSuggestion { get; set; }
    }
}