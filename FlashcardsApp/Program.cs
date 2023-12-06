using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FlashcardsApp.Data;
using FlashcardsApp.Areas.Identity.Data;
using FlashcardsApp.Services;
using System;

var builder = WebApplication.CreateBuilder(args);

// Get the connection string from the configuration
var connectionString = builder.Configuration.GetConnectionString("FlashcardsAppContextConnection") ?? throw new InvalidOperationException("Connection string 'FlashcardsAppContextConnection' not found.");

// Add services to the container.
builder.Services.AddDbContext<FlashcardsAppContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddDefaultIdentity<FlashcardsAppUser>(options => options.SignIn.RequireConfirmedAccount = false).AddEntityFrameworkStores<FlashcardsAppContext>();
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
builder.Services.AddLogging();

builder.Services.AddHttpClient("FlashcardsAPI", httpClient =>
{
    httpClient.BaseAddress = new Uri("https://localhost:7296/api");
});

builder.Services.AddScoped<SearchService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
