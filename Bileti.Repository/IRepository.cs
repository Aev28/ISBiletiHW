using Bileti.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bileti.Repository
{
    public interface IRepository<T> where T : BaseEntity
    {
        IEnumerable<T> GetAll();
        T Get(Guid? id);
        T Add(T entity);
        T Edit(T entity);
        T Delete(T entity);
    }
}
