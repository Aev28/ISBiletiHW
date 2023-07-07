using Bileti.Domain.Models;
using Bileti.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bileti.Repository
{
    public interface IOrderRepository
    {
        public List<Order> getAllOrders();
        public Order getOrderDetails(BaseEntity model);
    }
}
