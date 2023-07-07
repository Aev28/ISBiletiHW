using Bileti.Domain.DTO;
using Bileti.Domain.Models;
using Bileti.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Bileti.Service.Impl
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IRepository<ShoppingCart> _shoppingCartRepository;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<EmailMessage> _mailRepository;
        private readonly IRepository<TicketInOrder> _ticketInOrderRepository;
        private readonly IUserRepository _userRepository;

        public ShoppingCartService(IRepository<ShoppingCart> shoppingCartRepository, IUserRepository userRepository, IRepository<EmailMessage> mailRepository, IRepository<Order> orderRepository, IRepository<TicketInOrder> ticketInOrderRepository)
        {
            _shoppingCartRepository = shoppingCartRepository;
            _userRepository = userRepository;
            _orderRepository = orderRepository;
            _ticketInOrderRepository = ticketInOrderRepository;
            _mailRepository = mailRepository;
        }


        public bool DeleteFromCart(string userId, Guid productId)
        {
            if (!string.IsNullOrEmpty(userId) && productId != null)
            {
                var loggedInUser = this._userRepository.Get(userId);

                var userShoppingCart = loggedInUser.UserCart;

                var itemToDelete = userShoppingCart.TicketInCart.Where(z => z.TicketId.Equals(productId)).FirstOrDefault();

                userShoppingCart.TicketInCart.Remove(itemToDelete);

                this._shoppingCartRepository.Edit(userShoppingCart);

                return true;
            }
            return false;
        }

        public CartDTO GetCartInfo(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                var loggedInUser = this._userRepository.Get(userId);

                var userCard = loggedInUser.UserCart;

                var allProducts = userCard.TicketInCart.ToList();

                var allProductPrices = allProducts.Select(z => new
                {
                    ProductPrice = z.Ticket.Price,
                    Quantity = z.Quantity
                }).ToList();

                double totalPrice = 0.0;

                foreach (var item in allProductPrices)
                {
                    totalPrice += item.Quantity * item.ProductPrice;
                }

                var result = new CartDTO
                {
                    Tickets = allProducts,
                    TotalPrice = totalPrice
                };

                return result;
            }
            return new CartDTO();
        }

        public bool Order(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                var loggedInUser = this._userRepository.Get(userId);
                var userCard = loggedInUser.UserCart;

                EmailMessage mail = new EmailMessage();
                mail.MailTo = loggedInUser.Email;
                mail.Subject = "Sucessfuly created order!";
                mail.Status = false;


                Order order = new Order
                {
                    Id = Guid.NewGuid(),
                    User = loggedInUser,
                    UserId = userId
                };

                this._orderRepository.Add(order);

                List<TicketInOrder> productInOrders = new List<TicketInOrder>();

                var result = userCard.TicketInCart.Select(z => new TicketInOrder
                {
                    Id = Guid.NewGuid(),
                    TicketId = z.Ticket.Id,
                    Ticket = z.Ticket,
                    OrderId = order.Id,
                    Order = order,
                    Quantity = z.Quantity
                }).ToList();

                StringBuilder sb = new StringBuilder();

                var totalPrice = 0.0;

                sb.AppendLine("Your order is completed. The order conatins: ");

                for (int i = 1; i <= result.Count(); i++)
                {
                    var currentItem = result[i - 1];
                    totalPrice += currentItem.Quantity * currentItem.Ticket.Price;
                    sb.AppendLine(i.ToString() + ". " + currentItem.Ticket.Price + " with quantity of: " + currentItem.Quantity + " and price of: $" + currentItem.Ticket.Price);
                }

                sb.AppendLine("Total price for your order: " + totalPrice.ToString());

                mail.Content = sb.ToString();


                productInOrders.AddRange(result);

                foreach (var item in productInOrders)
                {
                    this._ticketInOrderRepository.Add(item);
                }

                loggedInUser.UserCart.TicketInCart.Clear();

                this._userRepository.Edit(loggedInUser);
                this._mailRepository.Add(mail);

                return true;
            }

            return false;
        }
    }
}
