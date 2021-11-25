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
using CustomerAccountDeletionRequest.Extensions;
using CustomerAccountDeletionRequest.Models;

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
            (Configuration.GetConnectionString("ThamcoConnectionString")));

            services.AddControllers().AddNewtonsoftJson(j =>
            {
                j.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddMemoryCache();

            if (_environment.IsDevelopment())
            {
                services.AddSingleton<ICustomerAccountDeletionRequestRepository, FakeCustomerAccountDeletionRequestRepository>();
            }
            else
            {
                services.AddScoped<ICustomerAccountDeletionRequestRepository, SqlCustomerAccountDeletionRequestRepository>();
            }

            services.AddSingleton<IMemoryCacheAutomater, MemoryCacheAutomater>();
            services.Configure<MemoryCacheModel>(Configuration.GetSection("MemoryCache"));
            services.Configure<DatabaseAttributesModel>(Configuration.GetSection("DatabaseAttributes"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMemoryCacheAutomater memoryCacheAutomater, Context.Context context)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                context.Database.Migrate();
            }

            app.ConfigureCustomExceptionMiddleware();

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
