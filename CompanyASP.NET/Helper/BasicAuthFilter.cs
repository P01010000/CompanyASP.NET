using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Builder.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyASP.NET.Helper
{
    public class BasicAuthFilter
    {
        public void Configure(IApplicationBuilder appBuilder)
        {
            appBuilder.UseMiddleware<BasicAuthMiddleware>();
        }
    }
}
