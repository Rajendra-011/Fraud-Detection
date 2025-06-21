using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FraudDetectionWeb.Services;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register custom services
builder.Services.AddSingleton<DataLoader>();
builder.Services.AddSingleton<FraudDetector>();

var app = builder.Build();

// Train the model during startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dataLoader = services.GetRequiredService<DataLoader>();
    var fraudDetector = services.GetRequiredService<FraudDetector>();

    // Load transactions and train the model
    var transactions = dataLoader.LoadTransactions("transactions.csv");
    fraudDetector.Train(transactions);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Transactions}/{action=Index}/{id?}");

app.Run();
