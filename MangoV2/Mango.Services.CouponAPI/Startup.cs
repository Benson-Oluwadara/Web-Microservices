
using Mango.Services.CouponAPI.Database.DapperRepositorys;
using Mango.Services.CouponAPI.Database.IDapperRepositorys;
using Mango.Services.CouponAPI.Mapping;
using Mango.Services.CouponAPI.Repository.IRepository;
using Mango.Services.CouponAPI.Repository.Repository;
using Mango.Services.CouponAPI.Service.IService;
using Mango.Services.CouponAPI.Service.Service;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;
using AutoMapper;
//using AutoMapper.Extensions.Microsoft.DependencyInjection as AutoMapperExtensions;


namespace Mango.Services.CouponAPI
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
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Coupon API", Version = "v1" });



                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {

                });

                // Additional configurations for Swagger if needed
            });

            services.AddScoped<ICouponRepository, CouponRepository>();         
            services.AddScoped<ICouponService,CouponService>();
            services.AddAutoMapper(typeof(Startup)); // Replace with the correct assembly containing profiles

            
            var key = Configuration.GetValue<string>("ApiSettings:Secret");
            services.AddAuthentication();
            // Register IDbConnection (e.g., SqlConnection)
            services.AddTransient<IDbConnection>(_ => new SqlConnection(Configuration.GetConnectionString("SqlConnection")));

            // Register IDapperRepository
            services.AddScoped<IDapperRepository, DapperRepository>();
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {


            //if (env.IsDevelopment())
            //{
                //app.UseDeveloperExceptionPage();
                app.UseSwagger();
            //app.UseSwaggerUI();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "CouponAPI v1");
                c.RoutePrefix = string.Empty;
            });
            //}
            //else
            //{
            //    app.UseExceptionHandler("/Error");
            //    app.UseHsts();
            //}

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication(); // This is for Authentication
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
