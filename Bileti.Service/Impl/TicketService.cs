using Bileti.Domain.DTO;
using Bileti.Domain.Models;
using Bileti.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bileti.Service.Impl
{
    public class TicketService : ITicketService
    {
        private readonly IRepository<Ticket> _ticketRepository;
        private readonly IRepository<TicketInShoppingCart> _ticketInShoppingCartRepository;
        private readonly IUserRepository _userRepository;

        public TicketService(IRepository<Ticket> ticketRepository, IRepository<TicketInShoppingCart> ticketInShoppingCartRepository, IUserRepository userRepository)
        {
            _ticketRepository = ticketRepository;
            _userRepository = userRepository;
            _ticketInShoppingCartRepository = ticketInShoppingCartRepository;
        }


        public bool AddToShoppingCart(CartDTO item, string userID)
        {
            var user = this._userRepository.Get(userID);

            var userShoppingCart = user.UserCart;

            if (item.SelectedTicketId != null && userShoppingCart != null)
            {
                var ticket = this.GetDetailsTicket(item.SelectedTicketId);
                if (ticket != null)
                {
                    TicketInShoppingCart itemToAdd = new TicketInShoppingCart
                    {
                        Id = Guid.NewGuid(),
                        Ticket = ticket,
                        TicketId = ticket.Id,
                        Cart = userShoppingCart,
                        CartId = userShoppingCart.Id,
                        Quantity = item.Quantity
                    };

                    var existing = userShoppingCart.TicketInCart.Where(z => z.TicketId == userShoppingCart.Id && z.TicketId == itemToAdd.TicketId).FirstOrDefault();

                    if (existing != null)
                    {
                        existing.Quantity += itemToAdd.Quantity;
                        this._ticketInShoppingCartRepository.Edit(existing);

                    }
                    else
                    {
                        this._ticketInShoppingCartRepository.Add(itemToAdd);
                    }
                    return true;
                }
                return false;
            }
            return false;
        }

        public void AddTicket(Ticket p)
        {
            this._ticketRepository.Add(p);
        }

        public void DeleteTicket(Guid id)
        {
            var product = this.GetDetailsTicket(id);
            this._ticketRepository.Delete(product);
        }

        public List<Ticket> GetAllTickets()
        {
            return this._ticketRepository.GetAll().ToList();
        }

        public Ticket GetDetailsTicket(Guid? id)
        {
            return this._ticketRepository.Get(id);
        }

        public CartDTO GetCartInfo(Guid? id)
        {
            var product = this.GetDetailsTicket(id);
            CartDTO model = new CartDTO
            {
                SelectedTicket = product,
                SelectedTicketId = product.Id,
                Quantity = 1
            };

            return model;
        }

        public void EditTicket(Ticket p)
        {
            this._ticketRepository.Edit(p);
        }
    }
}
