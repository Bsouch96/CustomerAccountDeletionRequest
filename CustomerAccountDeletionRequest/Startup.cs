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
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace CustomerAccountDeletionRequest
{
    public class Startup
    {
        private readonly IWebHostEnvironment _environment;
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            _environment = environment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            if(_environment.IsDevelopment())
            {
                services.AddDbContext<Context.Context>(options => options.UseSqlServer("localhost"));
            }
            else if (_environment.IsStaging() || _environment.IsProduction())
            {
                services.AddDbContext<Context.Context>(options => options.UseSqlServer
                    (Configuration.GetConnectionString("ThamcoConnectionString"),
                        sqlServerOptionsAction: sqlOptions => sqlOptions.EnableRetryOnFailure
                        (
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(2),
                            errorNumbersToAdd: null
                        )
                    )
                );
            }

            SetupAuth(services);
            
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMemoryCacheAutomater memoryCacheAutomater, Context.Context context)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else if(env.IsStaging() || env.IsProduction())
            {
                context.Database.Migrate();
                app.ConfigureCustomExceptionMiddleware();
                memoryCacheAutomater.AutomateCache();
            }
            else
            {
                app.ConfigureCustomExceptionMiddleware();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void SetupAuth(IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.Authority = $"https://{Configuration["Auth0:Domain"]}/";
                options.Audience = Configuration["Auth0:Audience"];
            });

            services.AddAuthorization(o =>
            {
                o.AddPolicy("ReadAllCustomerAccountDeletionRequests", policy =>
                    policy.RequireClaim("permissions", "read:customer_account_deletion_requests"));
                o.AddPolicy("ReadCustomerAccountDeletionRequest", policy =>
                    policy.RequireClaim("permissions", "read:product_review"));
                o.AddPolicy("CreateCustomerAccountDeletionRequest", policy =>
                    policy.RequireClaim("permissions", "add:customer_account_deletion_request"));
                o.AddPolicy("UpdateCustomerAccountDeletionRequest", policy =>
                    policy.RequireClaim("permissions", "edit:customer_account_deletion_requests"));
            });
        }
    }
}
