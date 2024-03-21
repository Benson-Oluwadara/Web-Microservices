using mango.web.frontend.Services.Iservices;
using mango.web.frontend.Services.services;
using mango.web.frontend.Services.Services;
using mango.web.frontend.Utility;
using Mango.web.frontend.Services.services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();

SD.CouponAPIBase = builder.Configuration["ServiceUrls:CouponAPI"];
SD.AuthAPIBase = builder.Configuration["ServiceUrls:AuthAPI"];
SD.ProductAPIBase = builder.Configuration["ServiceUrls:ProductAPI"];
SD.ShoppingCartAPIBase = builder.Configuration["ServiceUrls:ShoppingCartAPI"];
Console.WriteLine($"CouponAPIBase: {SD.CouponAPIBase}");
Console.WriteLine($"AuthAPIBase: {SD.AuthAPIBase}");
Console.WriteLine($"ProductAPIBase: {SD.ProductAPIBase}");
Console.WriteLine($"ShoppingCartAPIBase: {SD.ShoppingCartAPIBase}");
// Add services to the container.
builder.Services.AddControllersWithViews();
//builder.Services.AddAutoMapper(typeof(Startup));
builder.Services.AddHttpClient<ICouponService, CouponService>();
builder.Services.AddScoped<IBaseService, BaseService>();
builder.Services.AddScoped<ICouponService, CouponService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenProvider, TokenProvider>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddHttpClient("CouponAPI", client =>
{
    client.BaseAddress = new Uri("https://localhost:6001/");
});
builder.Services.AddHttpClient("AuthAPI", client =>
{
    client.BaseAddress = new Uri("https://localhost:6002/");
});
builder.Services.AddHttpClient("ProductAPI", client =>
{
    client.BaseAddress = new Uri("https://localhost:6003/");
});
builder.Services.AddHttpClient("ShoppingCartAPI", client =>
{
    client.BaseAddress = new Uri("https://localhost:6004/");
});
builder.Services.AddLogging();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan= TimeSpan.FromMinutes(30);
        options.LoginPath = "/Auth/Login"; // Adjust the login path based on your setup
        options.AccessDeniedPath = "/Auth/AccessDenied"; // Adjust the access denied path based on your setup
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
