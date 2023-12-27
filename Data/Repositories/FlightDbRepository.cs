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
    }
}
