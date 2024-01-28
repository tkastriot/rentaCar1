using System.ComponentModel.DataAnnotations.Schema;
using RentACar_1.Data;

namespace RentACar_1.Models
{
    public class Booking
    {
        public int BookingID { get; set; }
        [ForeignKey("Renter")]
        public string RenterID { get; set; } // Foreign key to User table
        [ForeignKey("Car")]
        public int CarID { get; set; } // Foreign key to Car table
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public ApplicationUser Renter { get; set; }
        public Car Car { get; set; }
    }
}
