using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyASP.NET.Models
{
    public class UacGroup
    {
        public int UserGroupId { get; set; }
        public string ShowName { get; set; }
        public string Name { get; set; }
        public int CountMember { get; set; }
        public string Description { get; set; }
    }
}
