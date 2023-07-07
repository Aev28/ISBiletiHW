using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Stripe;
using System.Security.Claims;
using System;
using Bileti.Service;
using Bileti.Domain.Models;
using System.Reflection;
using Bileti.Domain.DTO;
using System.Collections.Generic;
using System.Linq;
using ClosedXML.Excel;
using System.IO;
using System.Net.Http;
using Bileti.Repository.Migrations;
using Microsoft.AspNetCore.Authorization;

namespace Bileti.Web.Controllers
{
    public class TicketsController : Controller
    {
        private readonly ITicketService _ticketService;
        private readonly ILogger<TicketsController> _logger;

        public TicketsController(ILogger<TicketsController> logger, ITicketService ticketService)
        {
            _logger = logger;
            _ticketService = ticketService;
        }

        // GET: Tickets
        public IActionResult Index()
        {
            this.genresInit();
            _logger.LogInformation("User Request -> Get All Tickets!");
            return View(this._ticketService.GetAllTickets());
        }

        [HttpPost]
        public IActionResult Search(DateTime date)
        {
            this.genresInit();
            _logger.LogInformation("Filter");

            IEnumerable<Ticket> tickets = _ticketService.GetAllTickets();

            if (date != null)
            {
                tickets = from ticket in _ticketService.GetAllTickets()
                          where DateTime.Compare(ticket.DateValid.Date, date.Date) < 0
                          select ticket;
             }

            return View(tickets.ToList());
        }

        // GET: Tickets/Details/5
        public IActionResult Details(Guid? id)
        {
            _logger.LogInformation("User Request -> Get Details For Tickets");
            if (id == null)
            {
                return NotFound();
            }

            var product = this._ticketService.GetDetailsTicket(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Tickets/Create
        public IActionResult Create()
        {
            _logger.LogInformation("User Request -> Get create form for Product!");
            Ticket ticket = new Ticket();
            ticket.genres = new List<String>() {
                "Action",
                "Adventure",
                "Comedy",
                "Drama",
                "Fantasy",
                "Horror",
                "Musical",
                "Mystery",
                "Romance",
                "Sci-Fi",
                "Western",
                "Thriller"
            };
            return View(ticket);
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,Title,Price,DateValid,selectedGenre")] Ticket ticket)
        {
            _logger.LogInformation("User Request -> Insert Product in DataBase!");
            if (ModelState.IsValid)
            {
                ticket.Id = Guid.NewGuid();
                this._ticketService.AddTicket(ticket);
                return RedirectToAction(nameof(Index));
            }
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public IActionResult Edit(Guid? id)
        {
            _logger.LogInformation("User Request -> Get edit form for Ticket!");
            if (id == null)
            {
                return NotFound();
            }

            var ticket = this._ticketService.GetDetailsTicket(id);
            if (ticket == null)
            {
                return NotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, [Bind("Id, Title, Price, DateValid, selectedGenre")] Ticket ticket)
        {
            _logger.LogInformation("User Request -> Update Ticket in DataBase!");

            if (id != ticket.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    this._ticketService.EditTicket(ticket);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketExists(ticket.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public IActionResult Delete(Guid? id)
        {
            _logger.LogInformation("User Request -> Get delete form for Ticket!");

            if (id == null)
            {
                return NotFound();
            }

            var ticket = this._ticketService.GetDetailsTicket(id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            _logger.LogInformation("User Request -> Delete Product in DataBase!");

            this._ticketService.DeleteTicket(id);
            return RedirectToAction(nameof(Index));
        }


        public IActionResult AddToCart(Guid id)
        {
            var result = this._ticketService.GetCartInfo(id);

            return View(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddToCart(CartDTO model)
        {

            _logger.LogInformation("User Request -> Add Ticket in ShoppingCart and save changes in database!");


            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = this._ticketService.AddToShoppingCart(model, userId);

            if (result)
            {
                return RedirectToAction("Index", "Tickets");
            }
            return View(model);
        }
        private bool TicketExists(Guid id)
        {
            return this._ticketService.GetDetailsTicket(id) != null;
        }

/*        [Authorize(Roles = "ADMINISTRATOR")]
*/        public IActionResult ExportAllTickets(String genre)
        {
            string fileName = "TicketsExport.xlsx";
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            IEnumerable<Ticket> tickets = new List<Ticket>();

            if (genre != null)
            {
                tickets = from ticket in _ticketService.GetAllTickets()
                          where ticket.selectedGenre.Equals(genre)
                          select ticket;
            } else
            {
                tickets = _ticketService.GetAllTickets();
            }

            using (var workBook = new XLWorkbook())
            {
                IXLWorksheet worksheet = workBook.Worksheets.Add("All Tickets");

                worksheet.Cell(1, 1).Value = "Ticket Id";
                worksheet.Cell(1, 2).Value = "Title";
                worksheet.Cell(1, 3).Value = "Price";
                worksheet.Cell(1, 4).Value = "Valid until";
                worksheet.Cell(1, 5).Value = "Genre";

                for (int i = 1; i <= tickets.Count(); i++)
                {
                    var item = tickets.ElementAt(i - 1);

                    worksheet.Cell(i + 1, 1).Value = item.Id.ToString();
                    worksheet.Cell(i + 1, 2).Value = item.Title;
                    worksheet.Cell(i + 1, 3).Value = item.Price;
                    worksheet.Cell(i + 1, 4).Value = item.DateValid;
                    worksheet.Cell(i + 1, 5).Value = item.selectedGenre;
                }

                using (var stream = new MemoryStream())
                {
                    workBook.SaveAs(stream);

                    var content = stream.ToArray();

                    return File(content, contentType, fileName);
                }
            }

        }
        public void genresInit()
        {
            List<string> genres = new List<string>
            {
                "Action",
                "Adventure",
                "Comedy",
                "Drama",
                "Fantasy",
                "Horror",
                "Musical",
                "Mystery",
                "Romance",
                "Sci-Fi",
                "Western",
                "Thriller"
            };
            ViewBag.Genres = genres;
        }
    }

}
