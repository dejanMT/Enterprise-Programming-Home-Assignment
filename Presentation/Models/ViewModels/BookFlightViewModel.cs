using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Presentation.Models.ViewModels
{
    public class BookFlightViewModel
    {
        public Guid Id { get; set; }
        [Remote(action: "checkIfAvailable", controller: "TicketsConroller", AdditionalFields =nameof(Id) + "," + nameof(Row))]
        public int Row { get; set; }
        [Remote(action: "checkIfAvailable", controller: "TicketsConroller", AdditionalFields = nameof(Id) + "," + nameof(Column))]
        public int Column { get; set; }
        public Guid FlightIdFK { get; set; }

        [Required(ErrorMessage = "Passport is empty")]
        public string Passport { get; set; }
        public decimal PricePaid { get; set; }

        public IFormFile PassportImgFile { get; set; }
        public string SelectedSeat {  get; set; }


        // this is for the booking seating plan
        public List<(int Row, int Column)> TakenSeats { get; set; } = new List<(int Row, int Column)>();

        public bool IsSeatTaken(int row, int column)
        {
            return TakenSeats.Any(seat => seat.Row == row && seat.Column == column);
        }

    }
}
