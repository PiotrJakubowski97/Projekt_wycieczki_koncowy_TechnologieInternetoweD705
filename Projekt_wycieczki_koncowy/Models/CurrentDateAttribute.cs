using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Projekt_wycieczki_koncowy.Models
{
    public class CurrentDateAttribute : ValidationAttribute
    {
        public CurrentDateAttribute()
        {

        }
        public override bool IsValid(object value)
        {
            
            if(value !=null)
            {
                var dt = (DateTime)value;
                if (dt <= DateTime.Now)
                {
                    return true;
                }
                return false;
            }
            return false;
        }
    }
}