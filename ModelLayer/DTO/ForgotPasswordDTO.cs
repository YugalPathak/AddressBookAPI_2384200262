using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLayer.DTO
{
    /// <summary>
    /// Data Transfer Object (DTO) for sending the email.
    /// </summary>
    public class ForgotPasswordDTO
    {
        public string Email { get; set; }
    }
}