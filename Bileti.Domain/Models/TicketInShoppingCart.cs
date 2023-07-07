using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bileti.Domain.Models
{
    public class TicketInShoppingCart : BaseEntity
    {
        public Guid TicketId { get; set; }
        public virtual Ticket Ticket { get; set; }
        public Guid CartId { get; set; }
        public virtual ShoppingCart Cart { get; set; }
        public int Quantity { get; set; }
    }
}
