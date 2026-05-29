using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestaurantManager.Core.Data;
using RestaurantManager.Core.Services;
using RestaurantManager.Customer.Handlers;
using RestaurantManager.Customer.Views;
using Spectre.Console;
using DbContext = RestaurantManager.Core.Data.DbContext;

namespace RestaurantManager.Customer;

public class App
{
    private readonly IServiceProvider _serviceProvider;

    public App()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var connectionString = config.GetConnectionString("DefaultConnection")
            ?? throw new Exception("Connection string not found.");
        
        // Initialize services:
        _serviceProvider = new ServiceCollection()
            .AddDbContext<DbContext>(options =>
                    options.UseNpgsql(connectionString),
                ServiceLifetime.Scoped)
            .AddScoped<TableService>()
            .AddScoped<MenuService>()
            .AddScoped<OrderService>()
            .BuildServiceProvider();
        
        // Database setup:
        using var setupScope = _serviceProvider.CreateScope();
        var db = setupScope.ServiceProvider.GetRequiredService<DbContext>();
        db.Database.EnsureCreated();
        DbSeeder.Seed(db);
    }

    public void Run()
    {
        ShowSplashScreen();
    
        while (true)
        {
            var locator = new ServiceLocator(_serviceProvider);
            var orderHandler = new CustomerOrderHandler(locator);
            
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine("[bold purple]Welcome![/] Please select a free table to get started.\n");
    
            var freeTables = locator.GetTableService().GetAllTables().Where(t => t.Available).ToList();
    
            if (freeTables.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]No free tables available. Please ask a member of staff.[/]");
                AnsiConsole.MarkupLine("[grey]Press any key to refresh...[/]");
                Console.ReadKey();
                continue;
            }
    
            var tableNumber = CustomerTableView.PromptTableSelection(freeTables);
            if (tableNumber == null) continue;
    
            var table = freeTables.First(t => t.TableNumber == tableNumber);
            var order = locator.GetOrderService().CreateOrder(table.Id);
    
            orderHandler.Handle(order);
        }
    }

    private static void ShowSplashScreen()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(
            new FigletText("Welcome!")
                .Centered()
                .Color(Color.MediumPurple));
        
        AnsiConsole.MarkupLine("[grey]Press any key to continue...[/]");
        Console.ReadKey();
    }
}