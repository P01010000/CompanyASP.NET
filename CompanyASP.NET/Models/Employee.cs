using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyASP.NET.Models
{
    public class Employee : IModel
    {
        public int? Id { get; set; }
        public int? PersonId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public DateTime? Birthday { get; set; }
        public string Phone { get; set; }
        public string Gender { get; set; }
        public DateTime? EmployeeSince { get; set; }
        public bool Identity(int[] ids)
        {
            return Id == ids[0];
        }
    }
}
