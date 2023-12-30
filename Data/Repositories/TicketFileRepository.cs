using Domain.Interfaces;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class TicketFileRepository : ITicketRepository
    {
        string filePath;
        public TicketFileRepository(string pathToTicketsFile) { 
            filePath = pathToTicketsFile;

            if (System.IO.File.Exists(filePath) == false)
            {
                using (var myTicketsFile = System.IO.File.Create(filePath))
                {
                    myTicketsFile.Close();
                }
            }

        }

        public IQueryable<Ticket> GetTickets()
        {
            string allTicketText = System.IO.File.ReadAllText(filePath);

            if (allTicketText == "")
            {
                return new List<Ticket>().AsQueryable();
            } else
            {
                try
                {
                    List<Ticket> tickets = JsonSerializer.Deserialize<List<Ticket>>(allTicketText);
                    return tickets.AsQueryable();
                }
                catch
                {
                    return new List<Ticket>().AsQueryable();
                }
            }
        }

        public Ticket? GetTicket (Guid id)
        {
            return GetTickets().SingleOrDefault(t => t.Id == id);
        }

        public void Book(Ticket ticket)
        {
            // Flight flight
            //if (flight.DepartureDate <= DateTime.Now) return;


            ticket.Id = Guid.NewGuid();
            var ticketsList = GetTickets().ToList();
            ticketsList.Add(ticket);

            string jsonString = JsonSerializer.Serialize(ticketsList);
            System.IO.File.WriteAllText(filePath, jsonString);

        }

        public void Cancel(Ticket ticket)
        {
            var ticketsList = GetTickets().ToList();
            var existingTicket = ticketsList.FirstOrDefault(t => t.Id == ticket.Id);

            if (existingTicket != null && !existingTicket.Cancelled)
            {
                existingTicket.Cancelled = true;

                string jsonString = JsonSerializer.Serialize(ticketsList);
                System.IO.File.WriteAllText(filePath, jsonString);
            }
        }



    }
}
