using Data.DataContext;
using Data.Repositories;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Presentation.Models.ViewModels;

namespace Presentation.Controllers
{
    public class TicketsController : Controller
    {
        private FlightDbRepository _flightDbRepository;
        private TicketDBRepository _ticketDBRepository;
        private AirlineDbContext _airlineDbContext;

        public TicketsController(FlightDbRepository flightDbRepository, TicketDBRepository ticketDBRepository, AirlineDbContext airlineDbContext) 
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
                         select new ListFlightsViewModel()
                         {
                             Id = f.Id,
                             DepartureDate = f.DepartureDate,
                             ArrivalDate = f.ArrivalDate,
                             CountryFrom = f.CountryFrom,
                             CountryTo = f.CountryTo,
                             RetailPrice = f.WholesalePrice * (f.WholesalePrice + (decimal)f.CommissionRate), // Calculating the price
                             IsFullyBooked = _flightDbRepository.FlightAvailablity(f.Id) //Checking if the flight is fully booked
                         };

            return View(output);
        }

        [HttpGet]
        public IActionResult Book(Guid Id)
        {
            var flight = _flightDbRepository.GetFlight(Id);
            var tickets = _airlineDbContext.Tickets.Where(t => t.FlightIdFK == Id && !t.Cancelled).ToList();

            var viewModel = new BookFlightViewModel
            {
                Row = flight.Rows, 
                Column = flight.Columns,
                PricePaid = flight.WholesalePrice * (flight.WholesalePrice + (decimal)flight.CommissionRate),
                TakenSeats = tickets.Select(t => (t.Row, t.Column)).ToList()

            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Book(BookFlightViewModel t, [FromServices] IWebHostEnvironment host)
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


                string relativePath = "";
                if (t.PassportImgFile != null)
                {
                    string newFilename = Guid.NewGuid().ToString()+ Path.GetExtension(t.PassportImgFile.FileName);
                    relativePath = "/images/" + newFilename;
                    string absolutePath = host.WebRootPath + "\\images\\" + newFilename;
                    using (FileStream fs = new FileStream(absolutePath, FileMode.CreateNew))
                    {
                        t.PassportImgFile.CopyTo(fs);
                        fs.Flush();
                    }
                }

                //Seperate the string to rows and columns
                string[] seatRowAndColumn = (t.SelectedSeat).Split(',');
                int row = int.Parse(seatRowAndColumn[0]);
                int column = int.Parse(seatRowAndColumn[1]);

                _ticketDBRepository.Book(
                    new Ticket()
                    {
                        Row = row,
                        Column = column,
                        FlightIdFK = flight.Id,
                        Passport = t.Passport,
                        PricePaid = flight.WholesalePrice * (flight.WholesalePrice + (decimal)flight.CommissionRate),
                        Cancelled = false,
                        PassportImg = relativePath
                    }
                );
                TempData["message"] = "Ticket boked successfully!";

                //return View(t);
                return RedirectToAction("Index", "Tickets");
            } catch (Exception ex)
            {
                TempData["error"] = "Ticket not booked! Check your inputs!";
                return View(t);
            }
        }


        [HttpGet]
        [Authorize]
        public IActionResult TicketHistory(string passport)
        {
            var list = _ticketDBRepository.GetTickets().Where(t => t.Passport == passport && !t.Cancelled);

            var output = from t in list
                         join f in _airlineDbContext.Flights on t.FlightIdFK equals f.Id
                         select new ListTicketsViewModel()
                         {
                             Id = t.Id,
                             Row = t.Row,
                             Column = t.Column,
                             Flight = f.CountryFrom + " to " + f.CountryTo,
                             Passport =t.Passport,
                             PricePaid = t.PricePaid 
                         };

            return View(output.ToList());
        }

        public IActionResult Cancle(Guid id)
        {
            var ticket = (_ticketDBRepository.GetTickets().Where(t=> t.Id == id)).FirstOrDefault();
            if (ticket == null)
            {
                TempData["error"] = "No ticket to delete!";
            } else
            {
                _ticketDBRepository.Cancel(ticket);
                TempData["message"] = "Your flight from " + ticket.Flight + " has been canceled!";
            }

            return RedirectToAction("Index");
        }


    }
}
