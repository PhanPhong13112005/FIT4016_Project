using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using Shop.Application.Interfaces;
using Shop.Application.Services;
using Shop.Domain.Interfaces;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Repositories;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
//Đăng kí dịch vụ với DI Container
builder.Services.AddDbContext<ShopDbContext>(options =>options.UseSqlServer(builder.Configuration.GetConnectionString("ShopConnect")));
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repostitory<>));
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderService, OrderService>();



builder.Services.AddHttpContextAccessor();
builder.Services.AddSession();

var app = builder.Build();

app.UseSession();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapAreaControllerRoute(
    name: "Administrator",
    areaName: "Administrator",
    pattern: "Administrator/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"); 



app.Run();
