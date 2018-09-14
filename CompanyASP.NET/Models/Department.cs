using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyASP.NET.Models
{
    public class Department : IModel
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? Supervisor { get; set; }
        public int? SuperDepartment { get; set; }
        public int? CompanyId { get; set; }
        public bool Identity(int[] ids)
        {
            return Id == ids[0];
        }
    }
}
