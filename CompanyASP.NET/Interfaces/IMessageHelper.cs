using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyASP.NET.Interfaces
{
    public interface IMessageHelper
    {
        bool SendIntercom(string message);
    }
}
