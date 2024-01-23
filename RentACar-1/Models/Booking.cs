namespace RentACar_1.Models
{
    public class Booking
    {
        public int BookingID { get; set; }
        public int RenterID { get; set; } // Foreign key to User table
        public int CarID { get; set; } // Foreign key to Car table
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
