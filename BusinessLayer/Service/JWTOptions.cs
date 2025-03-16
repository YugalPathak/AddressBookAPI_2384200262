using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Service
{
    /// <summary>
    /// Configuration options for JWT authentication.
    /// Stores the secret key, issuer, and audience required for token generation and validation.
    /// </summary>
    public class JWTOptions
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }
}