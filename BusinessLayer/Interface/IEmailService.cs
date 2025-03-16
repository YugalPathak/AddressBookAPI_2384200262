using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interface
{
    /// <summary>
    /// Interface for email service to handle email-related operations.
    /// </summary>
    public interface IEmailService
    {
        void SendPasswordResetEmail(string email, string token);
    }
}