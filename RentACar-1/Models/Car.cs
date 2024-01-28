using System.ComponentModel.DataAnnotations.Schema;
using RentACar_1.Data;

namespace RentACar_1.Models
{
    public class Car
    {
        public int CarID { get; set; }
        [ForeignKey("Owner")]
        public string OwnerID { get; set; } // Foreign key to User table
        public int CarDetailID { get; set; } // Foreign key to CarDetails table
        public CarDetails CarDetail { get; set; }
        public string RegistrationNumber { get; set; }
        public decimal PricePerDay { get; set; }
        public bool IsAvailable { get; set; }
        public List<Booking> Bookings { get; set; }
        public ApplicationUser Owner { get; set; }
    }
}
