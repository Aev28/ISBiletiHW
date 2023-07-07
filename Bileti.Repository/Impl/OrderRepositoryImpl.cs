using Bileti.Domain.Models;
using Bileti.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bileti.Repository.Impl
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext context;
        private DbSet<Order> entities;
        string errorMessage = string.Empty;

        public OrderRepository(ApplicationDbContext context)
        {
            this.context = context;
            entities = context.Set<Order>();
        }
        public List<Order> getAllOrders()
        {
            return entities
                .Include(z => z.User)
                .Include(z => z.TicketInOrder)
                .Include("TicketInOrder.Ticket")
                .ToListAsync().Result;
        }

        public Order getOrderDetails(BaseEntity model)
        {
            return entities
               .Include(z => z.User)
               .Include(z => z.TicketInOrder)
               .Include("TicketInOrder.Ticket")
               .SingleOrDefaultAsync(z => z.Id == model.Id).Result;
        }
    }
}
