// OwnerCarsViewModel.cs

using System.Collections.Generic;
using RentACar_1.Models;

namespace RentACar_1.ViewModels
{
    public class OwnerCarsViewModel
    {
        public int CarId { get; set; }
        public string Brand { get; set; }
        public string Category { get; set; }
        public int Year { get; set; }
        public string? Description { get; set; }
        public bool IsAutomatic { get; set; }
        public string FuelType { get; set; }
        public int Power { get; set; }
        public string City { get; set; }
        public string RegistrationNumber { get; set; }
        public decimal PricePerDay { get; set; }
        public string? ImageUrl { get; set; }
        // public string OwnerID { get; set; }
        public List<BookingSchedule> BookingSchedule { get; set; }
    }
}
