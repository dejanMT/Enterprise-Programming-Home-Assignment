namespace Presentation.Models.ViewModels
{
    public class AdminListFlightsViewModel
    {
        public Guid Id { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime ArrivalDate { get; set; }
        public string CountryFrom { get; set; }
        public string CountryTo { get; set; }
        public decimal WholesalePrice { get; set; }
        public double CommissionRate { get; set; }
        public decimal RetailPrice { get; set; }
        public bool IsFullyBooked { get; set; }
    }
}
