using Domain.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Policy;

namespace Presentation.Models.ViewModels
{
    public class ListTicketsViewModel
    {
        public Guid Id { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public string Flight { get; set; }
        public string Passport { get; set; }
        public decimal PricePaid { get; set; }


        public string CountryFrom { get; set; }
        public string CountryTo { get; set; }
        public string PassportImg { get; set; }
    }
}
