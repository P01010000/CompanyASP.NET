using CompanyASP.NET.Helper;
using CompanyASP.NET.Interfaces;
using CompanyASP.NET.Models;
using CompanyASP.NET.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TobitLogger.Core;
using TobitLogger.Logstash;
using TobitLogger.Middleware;
using TobitWebApiExtensions.Authentication;
using TobitWebApiExtensions.Extensions;

namespace CompanyASP.NET
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // The HttpContextAccessor is required by RequestGuidContextProvider implementation
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Add the shipped implementation itself
            services.AddSingleton<ILogContextProvider, RequestGuidContextProvider>();

            services.AddChaynsToken();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("LocationId", policy =>
                policy.Requirements.Add(new LocationIdFilter(157669)));
            });
            services.AddSingleton<IAuthorizationHandler, LocationIdHandler>();

            services.Configure<ChaynsApiSettings>(Configuration.GetSection("ChaynsApiSettings"));
            services.AddScoped<IMessageHelper, MessageHelper>();

            services.AddScoped<IUacHelper, UacHelper>();

            services.Configure<DbSettings>(Configuration.GetSection("ConnectionStrings"));
            services.AddSingleton<IDbContext, SqlContext>();

            services.AddScoped<IRepository<Department>, DepartmentRepository>();
            services.AddScoped<IRepository<Company>, CompanyRepository>();
            services.AddScoped<IRepository<Employee>, EmployeeRepository>();
            services.AddScoped<IRepository<Address>, AddressRepository>();

            // Use the cachingRepository for Employee
            // services.AddSingleton<IRepository<Employee>>(sp => new CachingRepository<Employee>(new EmployeeRepository(sp.GetService<IDbContext>())));

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(o => o.SerializerSettings.NullValueHandling = NullValueHandling.Ignore);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, ILogContextProvider logContextProvider)
        {
            loggerFactory.AddLogstashLogger(Configuration.GetSection("Logger"), logContextProvider: logContextProvider);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            app.UseRequestLogging();
            app.UseHttpsRedirection();
            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
