using Data.Repositories;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models.ViewModels;

namespace Presentation.Controllers
{
    public class TicketsController : Controller
    {
        private FlightDbRepository _flightDbRepository;
        private TicketDBRepository _ticketDBRepository;

        public TicketsController(FlightDbRepository flightDbRepository, TicketDBRepository ticketDBRepository) 
        {
            _flightDbRepository = flightDbRepository; 
            _ticketDBRepository = ticketDBRepository;
        }

        public IActionResult Index()
        {
            var currentDate = DateTime.Now;
            IQueryable<Flight> list = _flightDbRepository.GetFlights();

            var output = from f in list
                         where f.DepartureDate > currentDate
                         select new ListFlightsViewModel()
                         {
                             Id = f.Id,
                             DepartureDate = f.DepartureDate,
                             ArrivalDate = f.ArrivalDate,
                             CountryFrom = f.CountryFrom,
                             CountryTo = f.CountryTo,
                             RetailPrice = f.WholesalePrice * (1 + (decimal)f.CommissionRate / 100), // Calculating the price
                             IsFullyBooked = _flightDbRepository.FlightAvailablity(f.Id) //Checking if the flight is fully booked
                         };

            return View(output);
        }

        [HttpGet]
        public IActionResult Book(Guid Id)
        {
            BookFlightViewModel viewModel = new BookFlightViewModel{};
            
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Book(BookFlightViewModel t)
        {
            try
            {
                if (ModelState.IsValid == false)
                {
                    return View(t); //return the model if invalid
                }


                var flight = _flightDbRepository.GetFlight(t.Id);
                if (flight == null || _flightDbRepository.FlightAvailablity(flight.Id) || flight.DepartureDate <= DateTime.Now)
                {
                    return View(t);
                }

                _ticketDBRepository.Book(
                    new Ticket()
                    {
                        Id = t.Id,
                        Row = t.Row,
                        Column = t.Column,
                        FlightIdFK = flight.Id,
                        Passport = t.Passport,
                        PricePaid = flight.WholesalePrice * (1 + (decimal)flight.CommissionRate / 100),
                        Cancelled = false
                    }
                );
                TempData["message"] = "Ticket boked successfully!";

                return View(t);
            } catch (Exception ex)
            {
                
                TempData["error"] = "Ticket not booked! Check your inputs!";
                return View(t);
            }
        }


    }
}
