using Bileti.Domain.DTO;
using Bileti.Domain.Models;
using System;
using System.Collections.Generic;

namespace Bileti.Service
{
    public interface ITicketService
    {
        List<Ticket> GetAllTickets();
        Ticket GetDetailsTicket(Guid? id);
        void AddTicket(Ticket p);
        void EditTicket(Ticket p);
        CartDTO GetCartInfo(Guid? id);
        void DeleteTicket(Guid id);
        bool AddToShoppingCart(CartDTO item, string userID);
    }
}