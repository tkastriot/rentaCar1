using RentACar_1.Data;

namespace RentACar_1.Models
{
	public class Review
	{
		public int ReviewID { get; set; }
		public int CarID { get; set; } // Reference to the car
		public string RentalID { get; set; } // Reference to the Rental -Application User
		public int Rating { get; set; }
		public string Comment { get; set; }
		public DateOnly CreatedDate { get; set; }
		public Car Car { get; set; }
		public ApplicationUser Rental { get; set; }
	}

}
