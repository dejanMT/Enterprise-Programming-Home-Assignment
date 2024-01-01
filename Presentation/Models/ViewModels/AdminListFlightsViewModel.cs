using System.ComponentModel.DataAnnotations;

namespace Presentation.Models.ViewModels
{
    public class AdminListFlightsViewModel
    {
        public Guid Id { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }

        [Display(Name = "Departure Date")]
        public DateTime DepartureDate { get; set; }

        [Display(Name = "Arrival Date")]
        public DateTime ArrivalDate { get; set; }

        [Display(Name = "Country From")]
        public string CountryFrom { get; set; }

        [Display(Name = "Country To")]
        public string CountryTo { get; set; }

        [Display(Name = "Wholesale Price")]
        public decimal WholesalePrice { get; set; }

        [Display(Name = "Commission Rate")]
        public double CommissionRate { get; set; }

        [Display(Name = "Retail Price")]
        public decimal RetailPrice { get; set; }

        [Display(Name = "Fully Booked")]
        public bool IsFullyBooked { get; set; }
    }
}
