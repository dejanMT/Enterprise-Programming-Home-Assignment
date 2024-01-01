using Data.DataContext;
using Data.Repositories;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models.ViewModels;
using System.Collections.Generic;

namespace Presentation.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {

        private FlightDbRepository _flightDbRepository;
        private ITicketRepository _ticketDBRepository;
        private AirlineDbContext _airlineDbContext;

        public AdminController(FlightDbRepository flightDbRepository, ITicketRepository ticketDBRepository, AirlineDbContext airlineDbContext)
        {
            _flightDbRepository = flightDbRepository;
            _ticketDBRepository = ticketDBRepository;
            _airlineDbContext = airlineDbContext;
        }

        public IActionResult Index()
        {
            var currentDate = DateTime.Now;
            IQueryable<Flight> list = _flightDbRepository.GetFlights();


            if (!list.Any())
            {
                TempData["error"] = "No Tickets found!";
                return View();
            }

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
        public IActionResult FlightTicket(Guid Id)
        {
            var tickets = _ticketDBRepository.GetTickets().Where(t => t.FlightIdFK == Id).ToList();

            if (tickets == null)
            {
                TempData["error"] = "No Tickets were booked for this flight!";
                return View();
            }
            else
            {
                var flight = _airlineDbContext.Flights.FirstOrDefault(f => f.Id == Id);

                if (flight == null)
                {
                    TempData["error"] = "Flight not found!";
                    return View();
                }

                var viewModel = tickets.Select(t => new ListTicketsViewModel
                {
                    Id = t.Id,
                    Passport = t.Passport,
                    Row = t.Row,
                    Column = t.Column,
                    Cancelled = t.Cancelled,
                    Flight = flight.CountryFrom + " to " + flight.CountryTo
                }).ToList();

                return View(viewModel);
            }
        }


        public IActionResult TicketDetails(Guid id)
        {
            var ticketList = _ticketDBRepository.GetTickets().ToList();
            var ticket = ticketList.FirstOrDefault(t => t.Id == id);

            if (ticket == null)
            {
                TempData["error"] = "Ticket not found!";
                return View();
            }

            var flight = _airlineDbContext.Flights.FirstOrDefault(f => f.Id == ticket.FlightIdFK);
            if (flight == null)
            {
                TempData["error"] = "Flight for this ticket not found!";
                return View();
            }

            var output = new ListTicketsViewModel
            {
                Id = ticket.Id,
                Passport = ticket.Passport,
                Flight = flight.CountryFrom + " to " + flight.CountryTo,
                Row = ticket.Row,
                Column = ticket.Column,
                PricePaid = ticket.PricePaid,
                PassportImg = ticket.PassportImg,
                Cancelled = ticket.Cancelled
            };

            return View(output);
        }
    }
}
