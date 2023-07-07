using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bileti.Domain.DTO
{
    public class UserDTO : IdentityUser
    {
        public List<string> Roles { get; set; }
        public string SelectedRole { get; set; }
    }
}
