using Chayns.Backend.Api.Models.Result;
using CompanyASP.NET.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyASP.NET.Interfaces
{
    public interface IUacHelper
    {
        IEnumerable<UacGroupResult> GetUacGroups(int LocationId, int userId);
    }
}
