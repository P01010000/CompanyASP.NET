using CompanyASP.NET.Models;
using CompanyASP.NET.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CompanyASP.NET
{
    public class Startup
    {
        public static string ConnectionString;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Startup.ConnectionString = Configuration.GetValue<string>("ConnectionString");
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddTransient<IRepository<Department>, DepartmentRepository>();
            services.AddTransient<IRepository<Company>, CompanyRepository>();
            services.AddTransient<IRepository<Employee>, EmployeeRepository>();
            services.AddTransient<IRepository<Address>, AddressRepository>();
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
            app.UseMvc();
        }
    }
}
