using Bileti.Domain.Models;
using Bileti.Domain;
using Bileti.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bileti.Service.Impl
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }
        public List<Order> GetAllOrders()
        {
            return this._orderRepository.getAllOrders();
        }

        public Order GetOrderDetails(BaseEntity model)
        {
            return this._orderRepository.getOrderDetails(model);
        }

        public double GetTotalPrice(Order order)
        {
            double sum = 0;
            foreach (var ticket in order.TicketInOrder)
            {
                sum = ticket.Quantity * ticket.Ticket.Price;
            }
            return sum;
        }

        public DateTime PurchaseDate(Order order)
        {
            return order.orderDate;
        }
    }
}
