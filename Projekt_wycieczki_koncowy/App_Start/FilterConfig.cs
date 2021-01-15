using System.Web;
using System.Web.Mvc;

namespace Projekt_wycieczki_koncowy
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
