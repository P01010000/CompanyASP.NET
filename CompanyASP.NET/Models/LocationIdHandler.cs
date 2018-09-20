using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyASP.NET.Models
{
    public class LocationIdHandler : AuthorizationHandler<LocationIdFilter>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, LocationIdFilter requirement)
        {
            if(context.User.HasClaim(c =>
            {
                var isLocation = c.Type.Equals("LocationId", StringComparison.InvariantCultureIgnoreCase);
                return isLocation && requirement.LocationId.Contains(Convert.ToInt32(c.Value));
            }))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
