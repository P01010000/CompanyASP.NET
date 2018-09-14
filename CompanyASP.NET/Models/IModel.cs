using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyASP.NET.Models
{
    public interface IModel
    {
        bool Identity(int[] ids);
    }
}
