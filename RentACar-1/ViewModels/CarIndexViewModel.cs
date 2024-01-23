using Microsoft.AspNetCore.Mvc.Rendering;
using RentACar_1.Models;

namespace RentACar_1.ViewModels
{
    public class CarIndexViewModel
    {
        public CarFilterViewModel Filters { get; set; }
        public List<Car> Cars { get; set; }
        public SelectList BrandList { get; set; }
        public SelectList CategoryList { get; set; }
        public SelectList CityList { get; set; }
    }
}
