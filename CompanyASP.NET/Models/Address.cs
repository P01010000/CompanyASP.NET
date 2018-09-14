using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyASP.NET.Models
{
    public class Address : IModel
    {
        public int? Id { get; set; }
        public string Street { get; set; }
        public string Zip { get; set; }
        public string Place { get; set; }
        public string Country { get; set; }

        public bool Identity(int[] ids)
        {
            return Id == ids[0];
        }
    }
}
