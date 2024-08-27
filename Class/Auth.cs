using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostDnFhirR4Api.Class
{
    public class Auth
    {
        public string AuthorizationUrl { get; set; }

        public string ClientID { get; set; }
        public string ClientSecret { get; set; }
    }
}
