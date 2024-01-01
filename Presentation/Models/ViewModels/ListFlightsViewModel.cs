using System.ComponentModel.DataAnnotations;

namespace Presentation.Models.ViewModels
{
    public class ListFlightsViewModel
    {
        public Guid Id { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }

        [Display(Name = "Departure Date")]
        public DateTime DepartureDate { get; set; }

        [Display(Name = "ArrivalDate")]
        public DateTime ArrivalDate { get; set; }

        [Display(Name = "Flight From")]
        public string CountryFrom { get; set; }

        [Display(Name = "Flight To")]
        public string CountryTo { get; set; }

        [Display(Name = "Price")]
        public decimal RetailPrice { get; set; }

        [Display(Name = "Fully Booked")]
        public bool IsFullyBooked { get; set; }
    }
}
