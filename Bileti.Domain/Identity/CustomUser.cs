using Bileti.Domain.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bileti.Domain.Identity
{
    public class CustomUser : IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public virtual ShoppingCart UserCart { get; set; }
        public string SelectedRole { get; set; }
        public string Password { get; set; }
    }
}
