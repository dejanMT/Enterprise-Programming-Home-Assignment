using Data.DataContext;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class FlightDbRepository
    {
        private AirlineDbContext _airlineDbContext;

        //Constructor Injection
        public FlightDbRepository(AirlineDbContext airlineDbContext)
        {
            _airlineDbContext = airlineDbContext;
        }

        public IQueryable<Flight> GetFlights()
        {
            return _airlineDbContext.Flights;
        }

        public Flight? GetFlight(Guid id) {
            return GetFlights().SingleOrDefault(x => x.Id == id);
        }


        public bool FlightAvailablity(Guid Id)
        {
            var flight = this.GetFlight(Id);

            if (flight == null)
            {
                throw new InvalidOperationException("Flight not found.");
            }

            int totalSeats = flight.Rows * flight.Columns;
            int bookedSeats = _airlineDbContext.Tickets.Count(t => t.FlightIdFK == Id && !t.Cancelled);

            return bookedSeats >= totalSeats;
        }
    }
}
