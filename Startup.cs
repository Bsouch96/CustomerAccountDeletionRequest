using CustomerAccountDeletionRequest.Context;
using CustomerAccountDeletionRequest.Repositories.Concrete;
using CustomerAccountDeletionRequest.Repositories.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace CustomerAccountDeletionRequest
{
    public class Startup
    {
        private readonly IWebHostEnvironment _environment;
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            _environment = environment;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<CustomerAccountDeletionRequestContext>(options => options.UseSqlServer
            (Configuration.GetConnectionString("CustomerAccountDeletionRequestConnection")));

            services.AddControllers();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            /*if(_environment.IsDevelopment())
            {
                services.AddTransient<ICustomerAccountDeletionRequestRepository, SqlCustomerAccountDeletionRequestRepository>();
            }
            else
            {
                services.AddScoped<ICustomerAccountDeletionRequestRepository, FakeCustomerAccountDeletionRequestRepository>();
            }*/
            services.AddScoped<ICustomerAccountDeletionRequestRepository, FakeCustomerAccountDeletionRequestRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
