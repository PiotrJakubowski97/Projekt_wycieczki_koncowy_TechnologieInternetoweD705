//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Projekt_wycieczki_koncowy.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class User
    {
        public int idUser { get; set; }


        [Required(ErrorMessage = "Nazwa u�ytkownika jest wymagana.")]
        [StringLength(49, ErrorMessage = "Zbyt d�uga nazwa u�ytkownika.")]
        [Display(Name = "Nazwa u�ytkownika")]
        public string UserName { get; set; }


        [Required(ErrorMessage = "Has�o jest wymagane.")]
        [StringLength(49, ErrorMessage = "Zbyt d�ugie has�o u�ytkownika.")]
        [DataType(DataType.Password)]
        [Display(Name = "Has�o uzytkownika")]
        public string Password { get; set; }


        [Compare("Password", ErrorMessage = "Potwierdzenie has�a jest wymagane.")]
        [StringLength(49, ErrorMessage = "Potwierdzenie has�a jest wymagane.")]
        [DataType(DataType.Password)]
        [Display(Name = "Potwierd� has�o u�ytkownika")]
        public string ConfirmPassword { get; set; }


        public string Rola { get; set; }
    }
}
