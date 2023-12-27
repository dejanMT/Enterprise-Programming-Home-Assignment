using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class OrderDetail
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Order")]
        public int OrderFK { get; set; }
        public virtual Order Order { get; set; }

        [ForeignKey("Ticket")]
        public Guid TicketFK { get; set; }
        public virtual Ticket Ticket { get; set; }

        public double Quantity { get; set; }

        public double Price { get; set; }
    }
}
