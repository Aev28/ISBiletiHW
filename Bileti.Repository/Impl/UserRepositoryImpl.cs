using Bileti.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bileti.Repository.Impl
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext context;
        private DbSet<CustomUser> entities;
        string errorMessage = string.Empty;

        public UserRepository(ApplicationDbContext context)
        {
            this.context = context;
            entities = context.Set<CustomUser>();
        }
        public IEnumerable<CustomUser> GetAll()
        {
            return entities.AsEnumerable();
        }

        public CustomUser Get(string id)
        {
            return entities
               .Include(z => z.UserCart)
               .Include("UserCart.TicketInCart")
               .Include("UserCart.TicketInCart.Ticket")
               .SingleOrDefault(s => s.Id == id);
        }
        public void Add(CustomUser entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entities.Add(entity);
            context.SaveChanges();
        }

        public void Edit(CustomUser entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entities.Update(entity);
            context.SaveChanges();
        }

        public void Delete(CustomUser entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entities.Remove(entity);
            context.SaveChanges();
        }
    }
}
