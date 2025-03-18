using Microsoft.EntityFrameworkCore;
using Stationery_Management_System.db_context;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddDbContext<sqldb>(db =>
{
    db.UseSqlServer(builder.Configuration.GetConnectionString("conn"));
});

builder.Services.AddSession(option =>
{

    option.IdleTimeout = TimeSpan.FromHours(24);
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

app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Admin_}/{action=Login}/{id?}");

app.Run();
