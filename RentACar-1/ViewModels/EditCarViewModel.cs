using System.ComponentModel.DataAnnotations;

namespace RentACar_1.ViewModels
{
    public class EditCarViewModel
    {
        public int CarId { get; set; }

        [Required(ErrorMessage = "Brand is required")]
        public string Brand { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public string Category { get; set; }

        [Required(ErrorMessage = "Year is required")]
        public int Year { get; set; }

        public string Description { get; set; }

        public bool IsAutomatic { get; set; }

        public string FuelType { get; set; }

        public int Power { get; set; }

        public string City { get; set; }

        [Required(ErrorMessage = "Registration number is required")]
        public string RegistrationNumber { get; set; }

        [Required(ErrorMessage = "Price per day is required")]
        public decimal PricePerDay { get; set; }
        public string? ImageUrl { get; set; }
    }
}
