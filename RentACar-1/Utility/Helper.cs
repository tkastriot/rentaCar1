using Microsoft.AspNetCore.Mvc.Rendering;

namespace RentACar_1.Utility
{
    public class Helper
    {
        public static string Owner = "Owner";
        public static string Renter = "Renter";


        public static List<SelectListItem> GetRolesForDropDown()
        {
            return new List<SelectListItem>()
           {
               new SelectListItem(Owner, Owner.ToUpper()),
               new SelectListItem(Renter, Renter.ToUpper())
           };
        }

    }
}
