//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.IdentityModel.Tokens;
//using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.OpenApi.Models;
//using AuthAPI.Data;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.AspNetCore.Builder;
//using AuthAPI.Models;
//using AuthAPI.Services.IService;
//using AuthAPI.Services.Service;
//using AuthAPI.Repository.Repository;
//using AuthAPI.Repository.IRepository;
//using Microsoft.Data.SqlClient;
//using System.Data;
//using System.Text;
//using mango.messagebus;
//using Serilog;

//var builder = WebApplication.CreateBuilder(args);

//builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
////Console.WriteLine("Connection String: " + builder.Configuration.GetConnectionString("SqlConnection"));
//// Logging with Serilog
//Log.Logger = new LoggerConfiguration()
//    .ReadFrom.Configuration(builder.Configuration) // Read Serilog configuration from appsettings.json
//    .CreateLogger();

//builder.Services.AddLogging(loggingBuilder =>
//{
//    loggingBuilder.ClearProviders(); // Clear default logging providers
//    loggingBuilder.AddSerilog(dispose: true); // Use Serilog for logging
//});

//// Add services to the container.
//builder.Services.AddControllers();
//builder.Services.AddSwaggerGen(c =>
//{
//    c.SwaggerDoc("v1", new OpenApiInfo { Title = "AuthAPI", Version = "v1" });
//});

//builder.Services.AddTransient<IDbConnection>(_ => new SqlConnection(builder.Configuration.GetConnectionString("SqlConnection")));
//builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtOptions"));

//var jwtSecret = builder.Configuration["ApiSettings:JwtOptions:Secret"];
//Console.WriteLine($"JwtSecret: {jwtSecret}");
//// Configure JWT authentication
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = true,
//            ValidateAudience = true,
//            ValidateLifetime = true,
//            ValidateIssuerSigningKey = true,
//            ValidIssuer = builder.Configuration["ApiSettings:JwtOptions:Issuer"],
//            ValidAudience = builder.Configuration["ApiSettings:JwtOptions:Audience"],
//            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["ApiSettings:JwtOptions:Secret"]))
//        };
//    });

//// Configure DbContext
////builder.Services.AddDbContext<AppDBContext>(options =>
////    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//// Add application services
//builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
//builder.Services.AddScoped<IAuthService, AuthService>();
//builder.Services.AddScoped<IRepository_, UserRepository>();
//builder.Services.AddScoped<IMessageBus, MessageBus>();

//var app = builder.Build();

//// Configure the HTTP request pipeline.
////if (app.Environment.IsDevelopment())
////{
//    app.UseSwagger();
//    app.UseSwaggerUI(c =>
//    {
//        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AuthAPI v1");
//        //c.RoutePrefix = string.Empty;
//    });
////} 


//app.UseHttpsRedirection();

//app.UseAuthentication(); // Use authentication middleware
//app.UseAuthorization();

//app.MapControllers(); // Simplified endpoint mapping

//app.Run();

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Mango.Services.AuthAPI;
public class Program
{
    public static void Main(string[] args)
    {

        var configuration = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json")
               .Build();

        // Configure Serilog
        Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(configuration)
            .CreateLogger();
        try
        {
            Log.Information("Application is starting");
            CreateHostBuilder(args).Build().Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application failed to start correctly");
        }
        finally
        {
            Log.Information("Application is shutting down");
            Log.CloseAndFlush();
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseSerilog() // Integrating Serilog with the host
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}