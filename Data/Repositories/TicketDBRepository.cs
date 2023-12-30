using Data.DataContext;
using Domain.Interfaces;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class TicketDBRepository : ITicketRepository
    {
        private AirlineDbContext _airlineDbContext;

        public TicketDBRepository(AirlineDbContext airlineDbContext)
        {
            _airlineDbContext = airlineDbContext;
        }

        public void Book(Ticket newTicket)
        {
            var existingTicket = _airlineDbContext.Tickets.FirstOrDefault(t => t.FlightIdFK == newTicket.FlightIdFK
                                                                            && t.Row == newTicket.Row
                                                                            && t.Column == newTicket.Column
                                                                            && !t.Cancelled);

            if (existingTicket == null)
            {
                _airlineDbContext.Tickets.Add(newTicket);
                _airlineDbContext.SaveChanges();
            }
        }

        public void Cancel(Ticket ticket)
        {
            //checking that teh ticket exists and that it was not already cancalled
            var existingTicket = _airlineDbContext.Tickets.FirstOrDefault(t => t.Id == ticket.Id);

            if (existingTicket != null && !existingTicket.Cancelled)
            {
                //Ticket cancelled, set prop. to true and save
                existingTicket.Cancelled = true;
                _airlineDbContext.SaveChanges();
            }
        }

        public IQueryable<Ticket> GetTickets()
        {
            return _airlineDbContext.Tickets;
        }



    }
}
