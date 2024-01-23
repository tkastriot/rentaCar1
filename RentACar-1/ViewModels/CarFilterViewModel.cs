namespace RentACar_1.ViewModels
{
    public class CarFilterViewModel
    {
        public bool? IsAutomatic { get; set; }
        public string Brand { get; set; }
        public string Category { get; set; }
        public string City { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
