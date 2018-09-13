using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyASP.NET.Models
{
    public class Address
    {
        public int? Id { get; set; }
        public string Street { get; set; }
        public string Zip { get; set; }
        public string Place { get; set; }
        public string Country { get; set; }
    }
}
