using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyASP.NET.Models
{
    public class LocationIdFilter : IAuthorizationRequirement
    {
        public int[] LocationId { get; set; }
        public LocationIdFilter(params int[] locationId)
        {
            LocationId = locationId;
        }
    }
}
