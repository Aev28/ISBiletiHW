using Bileti.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bileti.Domain.Models
{
    public class Order : BaseEntity
    {
        public string UserId { get; set; }
        public CustomUser User { get; set; }
        public virtual ICollection<TicketInOrder> TicketInOrder { get; set; }
        public DateTime orderDate { get; set; }
        public double total { get; set; }

        public Order()
        {
            orderDate = DateTime.Now;
        }
    }
}
