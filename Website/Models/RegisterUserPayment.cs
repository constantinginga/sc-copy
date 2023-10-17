using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StartupCentral.Models
{
    public class RegisterUserPayment
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Email { get; set; }

        [Required]
        public string PhoneNumber { get; set; }
        public string Cvr { get; set; }
        [Required(ErrorMessage = "Please accept the terms")]
        [Display(Name = "Accepterer du vores brugerbetingelser?")]
        public bool AcceptTerms { get; set; }
        [Display(Name = "Indsæt rabatkode")]
        public string PromocodeLabel { get; set; }
    }
}