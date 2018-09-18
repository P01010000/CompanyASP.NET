using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyASP.NET.Models
{
    public class ChaynsTokenPayload
    {
        public string jti { get; set; }
        public string sub { get; set; }
        public int type { get; set; }
        public DateTime iat { get; set; }
        public DateTime exp { get; set; }
        public int LocationId { get; set; }
        public string SiteId { get; set; }
        public bool IsAdmin { get; set; }
        public string PersonId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
