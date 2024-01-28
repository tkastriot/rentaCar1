namespace RentACar_1.ViewModels
{
    public class AddCarViewModel
    {
        public string Brand { get; set; }
        public string Category { get; set; }
        public int Year { get; set; }
        public string Description { get; set; }
        public bool IsAutomatic { get; set; }
        public string FuelType { get; set; }
        public int Power { get; set; }
        public string City { get; set; }
        public string RegistrationNumber { get; set; }
        public decimal PricePerDay { get; set; }
       // public string OwnerID { get; set; } // Assuming you'll pass the OwnerID
    }
}
