using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StartupCentral.Models
{
    public class RegisterUserDetails
    {
        [Display(Name = "Virksomhedsnavn")]
        public string UserName { get; set; }
        [Display(Name = "Valgfrit")]
        public string UserNameLabel { get; set; }
        [Display(Name = "Dette vil være dit brugernavn.")]
        [Remote("ValidateEmail", "RegisterUserDetails", HttpMethod = "POST")]
        [Required(ErrorMessage = "Dette felt er påkrævet.")]
        public string Email { get; set; }
        [Display(Name = "Et stærkt kodeord består af 12 eller flere tegn, bruges kun et sted og deles ikke med andre.")]
        [Required(ErrorMessage = "Dette felt er påkrævet.")]
        public string Password { get; set; }
        [Display(Name = "Kodeord*")]
        public string PasswordLabel { get; set; }
        [Required(ErrorMessage = "Dette felt er påkrævet.")]
        [Display(Name = "Telefonnummer*")]
        public string PhoneNumber { get; set; }
        public string Cvr { get; set; }
        [Display(Name = "Vil du gerne modtage nyheder, tilbud mv. fra StartupCentral?")]
        public bool Newsletter { get; set; }

        [Display(Name = "Adresse")]
        [Required(ErrorMessage = "Dette felt er påkrævet.")]
        public string AddressLine { get; set; }

        [Display(Name = "By")]
        [Required(ErrorMessage = "Dette felt er påkrævet.")]
        public string City { get; set; }

        [Display(Name = "Postnummer")]
        [Required(ErrorMessage = "Dette felt er påkrævet.")]
        public string ZipCode { get; set; }

        public string ErrorMessage { get; set; }

    }
}