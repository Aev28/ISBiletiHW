using Bileti.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bileti.Service
{
    public interface IShoppingCartService
    {
        CartDTO GetCartInfo(string userId);
        bool DeleteFromCart(string userId, Guid productId);
        bool Order(string userId);
    }
}
