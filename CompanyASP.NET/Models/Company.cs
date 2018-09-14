using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyASP.NET.Models
{
    public class Company : IModel
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? FoundedAt { get; set; }
        public string Branch { get; set; }

        public bool Identity(int[] ids)
        {
            return Id == ids[0];
        }
    }
}
