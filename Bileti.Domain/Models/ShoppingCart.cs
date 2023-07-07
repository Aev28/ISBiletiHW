using Bileti.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bileti.Domain.Models
{
    public class ShoppingCart : BaseEntity
    {
        public string OwnerId { get; set; }
        public virtual CustomUser Owner { get; set; }
        public virtual ICollection<TicketInShoppingCart> TicketInCart { get; set; }
    }
}
