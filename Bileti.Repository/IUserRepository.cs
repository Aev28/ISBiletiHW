using Bileti.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bileti.Repository
{
    public interface IUserRepository
    {
        IEnumerable<CustomUser> GetAll();
        CustomUser Get(string id);
        void Add(CustomUser entity);
        void Edit(CustomUser entity);
        void Delete(CustomUser entity);
    }
}
