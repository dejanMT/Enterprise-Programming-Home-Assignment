using Domain.Models;
using System.ComponentModel.DataAnnotations;
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

        [Display(Name = "Price")]
        public decimal PricePaid { get; set; }

        [Display(Name = "Flight From")]
        public string CountryFrom { get; set; }

        [Display(Name = "Flight To")]
        public string CountryTo { get; set; }
        public string PassportImg { get; set; }
        public bool Cancelled { get; set; }
    }
}
