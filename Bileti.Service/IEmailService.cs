using Bileti.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bileti.Service
{
    public interface IEmailService
    {
        Task SendEmailAsync(List<EmailMessage> allMails);
    }
}
