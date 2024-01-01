﻿using Data.DataContext;
using Data.Repositories;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Presentation.Models.ViewModels;

namespace Presentation.Controllers
{
    public class TicketsController : Controller
    {
        private FlightDbRepository _flightDbRepository;
        private ITicketRepository _ticketDBRepository;
        private AirlineDbContext _airlineDbContext;

        private readonly UserManager<CustomUser> _userManager;

        //constructor injection 
        public TicketsController(FlightDbRepository flightDbRepository, ITicketRepository ticketDBRepository, AirlineDbContext airlineDbContext, UserManager<CustomUser> userManager) 
        {
            _flightDbRepository = flightDbRepository; 
            _ticketDBRepository = ticketDBRepository;
            _airlineDbContext = airlineDbContext;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var currentDate = DateTime.Now;
            IQueryable<Flight> list = _flightDbRepository.GetFlights();

            if (list == null)
            {
                TempData["error"] = "No Flights found!";
                return RedirectToAction("Index", "Tickets");
            }

            var output = from f in list
                         where f.DepartureDate > currentDate
                         select new ListFlightsViewModel()
                         {
                             Id = f.Id,
                             DepartureDate = f.DepartureDate,
                             ArrivalDate = f.ArrivalDate,
                             CountryFrom = f.CountryFrom,
                             CountryTo = f.CountryTo,
                             RetailPrice = f.WholesalePrice + (f.WholesalePrice * (decimal)f.CommissionRate), // Calculating the price
                             IsFullyBooked = _flightDbRepository.FlightAvailablity(f.Id) //Checking if the flight is fully booked
                         };

            return View(output);
        }

        [HttpGet]
        public async Task<IActionResult> Book(Guid Id)
        {

            var flight = _flightDbRepository.GetFlight(Id);

            if (flight == null)
            {
                TempData["error"] = "Flight does not exist!";
                return RedirectToAction("Index", "Tickets");
            }

            var tickets = _airlineDbContext.Tickets.Where(t => t.FlightIdFK == Id && !t.Cancelled).ToList();

            var viewModel = new BookFlightViewModel
            {
                Row = flight.Rows,
                Column = flight.Columns,
                PricePaid = flight.WholesalePrice + (flight.WholesalePrice * (decimal)flight.CommissionRate),
                TakenSeats = tickets.Select(t => (t.Row, t.Column)).ToList(),
                
            };

            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null && user is CustomUser customUser) 
                {
                    viewModel.Passport = customUser.Passport;
                }
            }


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
                        PricePaid = flight.WholesalePrice + (flight.WholesalePrice * (decimal)flight.CommissionRate),
                        Cancelled = false,
                        PassportImg = relativePath,
                 
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
        public async Task<IActionResult> TicketHistory()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || !(currentUser is CustomUser castedUser))
            {
                TempData["error"] = "User not found or not logged in.";
                return RedirectToAction("Index", "Home");
            }

            var passport = castedUser.Passport;
            var ticketsList = _ticketDBRepository.GetTickets().Where(t => t.Passport == passport).ToList();
            var flightsList = _airlineDbContext.Flights.ToList();

            var output = ticketsList.Select(t => new ListTicketsViewModel
            {
                Id = t.Id,
                Row = t.Row,
                Column = t.Column,
                Flight = flightsList.FirstOrDefault(f => f.Id == t.FlightIdFK)?.CountryFrom + " to " + flightsList.FirstOrDefault(f => f.Id == t.FlightIdFK)?.CountryTo,
                Passport = t.Passport,
                PricePaid = t.PricePaid,
                Cancelled = t.Cancelled
            }).ToList();

            return View(output);
        }


        public IActionResult Cancel(Guid id)
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

        public IActionResult checkIfAvailable(int row, int column, Guid id)
        {
            bool available = _ticketDBRepository.IsSeatAvailable(row, column, id);
            if (available)
            {
                return Json(available);
            } else
            {
                return Json("This seat is take!");
            }
            
        }


    }
}
