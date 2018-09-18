using CompanyASP.NET.Helper;
using CompanyASP.NET.Models;
using CompanyASP.NET.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddChaynsToken();

            services.Configure<DbSettings>(Configuration.GetSection("ConnectionStrings"));
            services.AddSingleton<IDbContext, SqlContext>();

            services.AddScoped<IRepository<Department>, DepartmentRepository>();
            services.AddScoped<IRepository<Company>, CompanyRepository>();
            services.AddScoped<IRepository<Employee>, EmployeeRepository>();
            services.AddScoped<IRepository<Address>, AddressRepository>();
            
            // Use the cachingRepository for Employee
            // services.AddSingleton<IRepository<Employee>>(sp => new CachingRepository<Employee>(new EmployeeRepository(sp.GetService<IDbContext>())));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
