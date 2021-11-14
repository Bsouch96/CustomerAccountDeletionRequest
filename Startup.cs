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
using Newtonsoft.Json.Serialization;
using Invoices.Helpers.Interface;
using Invoices.Helpers.Concrete;

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
            services.AddDbContext<Context.Context>(options => options.UseSqlServer
            (Configuration.GetConnectionString("CustomerAccountDeletionRequestConnection")));

            services.AddControllers().AddNewtonsoftJson(j =>
            {
                j.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddMemoryCache();

            /*if(_environment.IsDevelopment())
            {
                services.AddTransient<ICustomerAccountDeletionRequestRepository, SqlCustomerAccountDeletionRequestRepository>();
            }
            else
            {
                services.AddScoped<ICustomerAccountDeletionRequestRepository, FakeCustomerAccountDeletionRequestRepository>();
            }*/
            services.AddSingleton<ICustomerAccountDeletionRequestRepository, FakeCustomerAccountDeletionRequestRepository>();
            services.AddSingleton<IMemoryCacheAutomater, MemoryCacheAutomater>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMemoryCacheAutomater memoryCacheAutomater)
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

            memoryCacheAutomater.AutomateCache();
        }
    }
}
