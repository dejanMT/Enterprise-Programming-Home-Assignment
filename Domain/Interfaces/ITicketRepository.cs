using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ITicketRepository
    {
        IQueryable<Ticket> GetTickets();
        void Book(Ticket ticket);
        void Cancel(Ticket ticket);
        bool IsSeatAvailable(int row, int column, Guid id);
    }
}
