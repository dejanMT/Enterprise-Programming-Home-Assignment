using Data.DataContext;
using Data.Repositories;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models.ViewModels;

namespace Presentation.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {

        private FlightDbRepository _flightDbRepository;
        private TicketDBRepository _ticketDBRepository;
        private AirlineDbContext _airlineDbContext;

        public AdminController(FlightDbRepository flightDbRepository, TicketDBRepository ticketDBRepository, AirlineDbContext airlineDbContext)
        {
            _flightDbRepository = flightDbRepository;
            _ticketDBRepository = ticketDBRepository;
            _airlineDbContext = airlineDbContext;
        }

        public IActionResult Index()
        {
            var currentDate = DateTime.Now;
            IQueryable<Flight> list = _flightDbRepository.GetFlights();

            var output = from f in list
                         where f.DepartureDate > currentDate
                         select new AdminListFlightsViewModel()
                         {
                             Id = f.Id,
                             DepartureDate = f.DepartureDate,
                             ArrivalDate = f.ArrivalDate,
                             Columns = f.Columns,
                             Rows = f.Rows,
                             CountryFrom = f.CountryFrom,
                             CountryTo = f.CountryTo,
                             WholesalePrice = f.WholesalePrice,
                             CommissionRate = f.CommissionRate,
                             RetailPrice = f.WholesalePrice * (1 + (decimal)f.CommissionRate / 100), // Calculating the price
                             IsFullyBooked = _flightDbRepository.FlightAvailablity(f.Id) //Checking if the flight is fully booked
                         };

            return View(output);
        }

        [HttpGet]
        public IActionResult FlightTicket (Guid Id)
        {
            var list = _ticketDBRepository.GetTickets().Where(t => t.FlightIdFK == Id);
            if (list == null)
            {
                TempData["error"] = "No Tickets where booked for this flight!";
                return View();
            } else
            {
                var output = from t in list
                             join f in _airlineDbContext.Flights on t.FlightIdFK equals f.Id
                             select new ListTicketsViewModel()
                             {
                                 Id = t.Id,
                                 Passport = t.Passport,
                                 Row = t.Row,
                                 Column = t.Column,
                                 Cancelled = t.Cancelled
                             };

                return View(output.ToList());
            }
           
        }

        public IActionResult TicketDetails(Guid id)
        {
            var ticket = _ticketDBRepository.GetTickets().Where(t => t.Id == id);

            var output = (from t in ticket
                         join f in _airlineDbContext.Flights on t.FlightIdFK equals f.Id
                         select new ListTicketsViewModel()
                         {
                             Id = t.Id,
                             Passport = t.Passport,
                             Flight = f.CountryFrom + " to " + f.CountryTo,
                             Row = t.Row,
                             Column = t.Column,
                             PricePaid = t.PricePaid,
                             PassportImg = t.PassportImg,
                             Cancelled = t.Cancelled

                         }).FirstOrDefault();

            return View(output);
        }
    }
}
