using Domain.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Presentation.Models.ViewModels
{
    public class ListTicketsHistoryViewModel
    {
        public Guid Id { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public string Flight { get; set; }
        public string Passport { get; set; }
        public decimal PricePaid { get; set; }
    }
}
