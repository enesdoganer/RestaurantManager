using Microsoft.Extensions.Configuration;
using RestaurantManager.Core.Data;
using RestaurantManager.Core.Services;
using RestaurantManager.Customer.Handlers;
using RestaurantManager.Customer.Views;
using Spectre.Console;

namespace RestaurantManager.Customer;

public class App
{
    private readonly TableService _tableService;
    private readonly MenuService _menuService;
    private readonly OrderService _orderService;

    public App()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var connectionString = config.GetConnectionString("DefaultConnection")
            ?? throw new Exception("Connection string not found.");

        var db = DbContextFactory.Create(connectionString);
        _tableService = new TableService(db);
        _menuService = new MenuService(db);
        _orderService = new OrderService(db, _tableService);
    }

    public void Run()
    {
        ShowSplashScreen();
    
        var orderHandler = new OrderHandler(_orderService, _menuService);
    
        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine("[bold purple]Welcome![/] Please select a free table to get started.\n");
    
            var freeTables = _tableService.GetAllTables().Where(t => t.Available).ToList();
    
            if (freeTables.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]No free tables available. Please ask a member of staff.[/]");
                AnsiConsole.MarkupLine("[grey]Press any key to refresh...[/]");
                Console.ReadKey();
                continue;
            }
    
            var tableNumber = TableView.PromptTableSelection(freeTables);
            if (tableNumber == null) continue;
    
            var table = freeTables.First(t => t.TableNumber == tableNumber);
            var order = _orderService.CreateOrder(table.Id);
    
            orderHandler.Handle(order);
        }
    }

    private static void ShowSplashScreen()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(
            new FigletText("Welcome!")
                .Centered()
                .Color(Color.Green));
        AnsiConsole.MarkupLine("[grey]Press any key to continue...[/]");
        Console.ReadKey();
    }
}