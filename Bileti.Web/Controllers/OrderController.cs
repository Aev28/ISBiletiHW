using Bileti.Domain;
using Bileti.Domain.Models;
using Bileti.Service;
using Bileti.Service.Impl;
using GemBox.Document;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Bileti.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
            ComponentInfo.SetLicense("FREE-LIMITED-KEY");
        }

        public IActionResult Index()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var orders = _orderService.GetAllOrders().Where(z => z.UserId == userId);
            var tempOrders = new List<Order>();

            foreach (var order in orders)
            {
                var orderDto = new Order
                {
                    Id = order.Id,
                    orderDate = order.orderDate,
                    total = _orderService.GetTotalPrice(order)
                };

                tempOrders.Add(orderDto);
            }
            return View(tempOrders);
        }
        public IActionResult CreateInvoice(Guid id)
        {
            var model = new BaseEntity
            {
                Id = id
            };

            Order order = this._orderService.GetOrderDetails(model);

            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "Invoice.docx");
            var document = DocumentModel.Load(templatePath);

            document.Content.Replace("{{OrderNumber}}", order.Id.ToString());
            document.Content.Replace("{{Email}}", order.User.Email);
            document.Content.Replace("{{Name}}", (order.User.Name + " " + order.User.Surname));
            document.Content.Replace("{{DateCreated}}", order.orderDate.ToString());

            StringBuilder sb = new StringBuilder();

            var total = 0.0;

            foreach (var item in order.TicketInOrder)
            {
                total += item.Quantity * item.Ticket.Price;
                sb.AppendLine("You have bought the ticket with the title: " + item.Ticket.Title + " and with a quantity of: " + item.Quantity + " and a ticket price of: $" + item.Ticket.Price);
            }

            document.Content.Replace("{{AllTickets}}", sb.ToString());
            document.Content.Replace("{{TotalPrice}}", "$" + total.ToString());
            var stream = new MemoryStream();

            document.Save(stream, new PdfSaveOptions());

            return File(stream.ToArray(), new PdfSaveOptions().ContentType, "ExportInvoice.pdf");
        }
    }
}
