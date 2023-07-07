using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bileti.Domain.Models;

namespace Bileti.Domain.DTO
{
    public class CartDTO
    {
        public Ticket SelectedTicket { get; set; }
        public Guid SelectedTicketId { get; set; }
        public List<TicketInShoppingCart> Tickets { get; set; }
        public int Quantity { get; set; }
        public double TotalPrice { get; set; }
    }
}
