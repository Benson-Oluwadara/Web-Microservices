using Mango.Services.EmailAPI.Database.DapperRepositorys;
using Mango.Services.EmailAPI.Database.IDapperRepositorys;
using Mango.Services.EmailAPI.Repository.IRepository;
using Mango.Services.EmailAPI.Repository.Repository;
using Mango.Services.EmailAPI.Services.IServices;
using Mango.Services.EmailAPI.Services.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Mango.Services.EmailAPI.Messaging;
using Mango.Services.EmailAPI.Extension;

namespace Mango.Services.EmailAPI
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
        .AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        })
        .AddXmlDataContractSerializerFormatters();


            // Add any other necessary services here
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Email API", Version = "v1" });



                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {

                });

                // Additional configurations for Swagger if needed
            });
        
           
           var key = Configuration.GetValue<string>("ApiSettings:Secret");
            services.AddAuthentication();
            services.AddTransient<IDbConnection>(_ => new SqlConnection(Configuration.GetConnectionString("SqlConnection")));
            services.AddScoped<IDapperRepository, DapperRepository>();
            services.AddScoped<IEmailRepository, EmailRepository>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IAzureServiceBusConsumer, AzureServiceBusConsumer>(); // Change to scoped if necessary
            services.AddControllers();

        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {


        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthentication(); // This is for Authentication
        app.UseAuthorization();
        app.UseAzureServiceBusConsumer();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
}

