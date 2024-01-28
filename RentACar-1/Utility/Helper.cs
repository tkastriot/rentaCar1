using Microsoft.AspNetCore.Mvc.Rendering;

namespace RentACar_1.Utility
{
    public class Helper
    {
        public static string Admin = "Admin";
        public static string User = "User";


        public static List<SelectListItem> GetRolesForDropDown()
        {
            return new List<SelectListItem>()
           {
               new SelectListItem(Admin, Admin),
               new SelectListItem(User, User)
           };
        }

    }
}
