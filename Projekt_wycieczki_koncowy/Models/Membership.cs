using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Projekt_wycieczki_koncowy.Models
{
    public class Membership
    {
        public int Id { get; set; }
        [Display(Name = "Nazwa uzytkownika")]
        public string UserName { get; set; }
        [Display(Name = "Hasło uzytkownika")]
        public string Password { get; set; }
    }
}