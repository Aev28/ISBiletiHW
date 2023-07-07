using Bileti.Domain.Models;
using Bileti.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bileti.Service
{
    public interface IOrderService
    {
        public List<Order> GetAllOrders();
        public Order GetOrderDetails(BaseEntity model);
        public double GetTotalPrice(Order order);
        public DateTime PurchaseDate(Order order);
    }
}
