using CompanyASP.NET.Helper;
using CompanyASP.NET.Models;
using CompanyASP.NET.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            // has to be removed
            Startup.ConnectionString = Configuration.GetValue<string>("ConnectionString");
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddScoped<IRepository<Department>, DepartmentRepository>();
            services.AddScoped<IRepository<Company>, CompanyRepository>();
            services.AddScoped<IRepository<Employee>, EmployeeRepository>();
            services.AddScoped<IRepository<Address>, AddressRepository>();
            // services.AddSingleton<IRepository<Company>>(new CachingRepository<Company>(CompanyRepository.getInstance()));
            // services.AddSingleton<IRepository<Company>>(CompanyRepository.getInstance());
            // services.AddSingleton<IRepository<Department>>(DepartmentRepository.getInstance());
            // services.AddSingleton<IRepository<Employee>>(EmployeeRepository.getInstance());
            // services.AddSingleton<IRepository<Address>>(AddressRepository.getInstance());

            //services.AddDbContext<DbContext>(options => options.UseSqlServer(Configuration["ConnectionString"]));
            services.AddSingleton<IDbContext, SqlContext>();
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
