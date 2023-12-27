using Domain.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Presentation.Models.ViewModels
{
    public class BookFlightViewModel
    {
        public Guid Id { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public Guid FlightIdFK { get; set; }

        //[Required(ErrorMessage = "Passport is empty")]
        public string Passport { get; set; }
        public decimal PricePaid { get; set; }
       // public Flight Flight { get; internal set; }
    }
}
