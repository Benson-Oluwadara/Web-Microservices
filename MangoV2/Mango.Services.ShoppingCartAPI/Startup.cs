//using Mango.Services.ProductAPI.Database.DapperRepositorys;
//using Mango.Services.ProductAPI.Database.IDapperRepositorys;
//using Mango.Services.ProductAPI.Repository.IRepository;
//using Mango.Services.ProductAPI.Repository.Repository;
//using Mango.Services.ProductAPI.Service.IService;
//using Mango.Services.ProductAPI.Service.Service;
using mango.messagebus;
using Mango.Services.ShoppingCartAPI.Database.DapperRepositorys;
using Mango.Services.ShoppingCartAPI.Database.IDapperRepositorys;
using Mango.Services.ShoppingCartAPI.Repository.IRepository;
using Mango.Services.ShoppingCartAPI.Repository.Repository;
using Mango.Services.ShoppingCartAPI.Service.IService;
using Mango.Services.ShoppingCartAPI.Service.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Data;
using System.Data.SqlClient;

namespace Mango.Services.ShoppingCartAPI
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
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                })
                .AddXmlDataContractSerializerFormatters();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ShoppingCart API", Version = "v1" });
            });

            
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<ICartRepository, CartRepository>();

            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICouponService, CouponService>();
            services.AddScoped<IMessageBus, MessageBus>();
            

            //services.AddScoped<IProductService, ProductService>();
            //services.AddScoped<ICouponService, CouponService>();
            services.AddTransient<IDbConnection>(_ => new SqlConnection(Configuration.GetConnectionString("SqlConnection")));
            //services.AddHttpClient();
            services.AddScoped<IDapperRepository, DapperRepository>();
            
            services.AddAutoMapper(typeof(Startup));
            services.AddHttpClient("ProductAPI", u => u.BaseAddress =
            new Uri(Configuration["ServiceUrls:ProductAPI"]));//.AddHttpMessageHandler<BackendApiAuthenticationHttpClientHandler>();
            services.AddHttpClient("CouponAPI", u => u.BaseAddress =
            new Uri(Configuration["ServiceUrls:CouponAPI"]));//.AddHttpMessageHandler<BackendApiAuthenticationHttpClientHandler>();
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
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
