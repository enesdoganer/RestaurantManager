using RestaurantManager.Admin.Views;
using RestaurantManager.Core.Services;
using Spectre.Console;

namespace RestaurantManager.Admin.Handlers;

public class TableHandler
{
    private readonly ServiceLocator _locator;
    
    public TableHandler(ServiceLocator locator)
    {
        _locator = locator;
    }
    
    // New Order handler:
    public void HandleNewOrder()
    {
        AnsiConsole.Clear();
        var tableService = _locator.GetTableService();
        var freeTables = tableService.GetAllTables().Where(t => t.Available).ToList();

        if (freeTables.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No free tables available.[/]");
            AnsiConsole.MarkupLine("[grey]Press any key to go back...[/]");
            Console.ReadKey();
            return;
        }

        var tableNumber = DashboardView.PromptTableSelection(freeTables);
        if (tableNumber == null)
        {
            AnsiConsole.MarkupLine("[red]Going back...[/]");
            Thread.Sleep(800);
            return;
        }
        
        var table = freeTables.First(t => t.TableNumber == tableNumber);
        var order = _locator.GetOrderService().CreateOrder(table.Id);
        
        var orderHandler = new OrderHandler(_locator);
        orderHandler.HandleOrderItems(order);
    }
    
    // Manage Order handler:
    public void HandleManageOrder()
    {
        AnsiConsole.Clear();
        var tableService = _locator.GetTableService();
        var occupiedTables = tableService.GetAllTables().Where(t => !t.Available).ToList();

        if (occupiedTables.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No occupied tables.[/]");
            AnsiConsole.MarkupLine("[grey]Press any key to go back...[/]");
            Console.ReadKey();
            return;
        }

        var tableNumber = DashboardView.PromptTableSelection(occupiedTables);
        if (tableNumber == null)
        {
            AnsiConsole.MarkupLine("[red]Going back...[/]");
            return;
        }
        
        var table = occupiedTables.First(t => t.TableNumber == tableNumber);
        var order = _locator.GetOrderService().GetActiveOrder(table.Id);

        if (order == null)
        {
            AnsiConsole.MarkupLine("[red]No active order found for this table.[/]");
            AnsiConsole.MarkupLine("[grey]Press any key to go back...[/]");
            Console.ReadKey();
            return;
        }
    
        var orderHandler = new OrderHandler(_locator);
        orderHandler.HandleOrderItems(order);
    }
    
}