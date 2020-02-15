using System;
using System.Text.Json.Serialization;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using PaymentGateway.API.Helpers;
using PaymentGateway.Domain.DomainServices.BankConnector;
using PaymentGateway.Persistence;
using PaymentGateway.Persistence.Repositories;

namespace PaymentGateway.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var persistenceConnectionString = Configuration["PersistenceConnectionString"];
            services.AddTransient(x => new DataContext(persistenceConnectionString));

            services.AddTransient<ModelMapper>();

            // repositories are scoped services based on the Unit-of-work pattern, thus only belong to the request
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IMerchantRepository, MerchantRepository>();

            services.AddSingleton<IBankConnectorDomainService, MockingTestBankConnectorDomainService>();

            // integrate Application Insights
            services.AddApplicationInsightsTelemetry();

            services.AddControllers().AddJsonOptions(jsonOptions =>
            {
                jsonOptions.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PaymentGateway API", Version = "v1" });
                c.EnableAnnotations();
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //NOTE: HTTPS redirection is not used on purpose, because a reverse proxy is assumed in 
            // front of this application which provides HTTPS termination
            //app.UseHttpsRedirection();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "PaymentGateway API V1");
            });

            app.UseRouting();

            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // update database to newest version via migrations
            // NOTE: in a production-ready system, this should not be executed from the application
            // and instead from a CI/CD or build script. 
            int maxAttempts = 3;
            int attempt = 0;
            while(attempt <= maxAttempts)
            {
                attempt++;

                try
                {
                    var dataContext = app.ApplicationServices.GetService<DataContext>();
                    dataContext.Database.Migrate();
                }
                catch (SqlException)
                {
                    // SQL server might not be available yet, let's retry in a moment

                    var waitTimeInSeconds = 5;
                    var logger = app.ApplicationServices.GetService<ILogger<Startup>>();
                    logger.LogWarning($"SQL server is not yet available, retrying in {waitTimeInSeconds} seconds");
                    Thread.Sleep(TimeSpan.FromSeconds(waitTimeInSeconds));
                }
            }
        }
    }
}