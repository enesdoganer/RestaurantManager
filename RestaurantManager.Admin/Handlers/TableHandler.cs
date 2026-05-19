using RestaurantManager.Admin.Views;
using RestaurantManager.Core.Services;
using Spectre.Console;

namespace RestaurantManager.Admin.Handlers;

public class TableHandler
{
    private readonly TableService _tableService;
    private readonly MenuService _menuService;
    private readonly OrderService _orderService;
    
    public TableHandler(TableService tableService, MenuService menuService, OrderService orderService)
    {
        _tableService = tableService;
        _menuService = menuService;
        _orderService = orderService;
    }
    
    // New Order handler:
    public void HandleNewOrder()
    {
        AnsiConsole.Clear();
    
        var freeTables = _tableService.GetAllTables().Where(t => t.Available).ToList();

        if (freeTables.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No free tables available.[/]");
            AnsiConsole.MarkupLine("[grey]Press any key to go back...[/]");
            Console.ReadKey();
            return;
        }

        var tableId = DashboardView.PromptTableSelection(freeTables);
        if (tableId == null)
        {
            AnsiConsole.MarkupLine("[red]Going back...[/]");
            Thread.Sleep(800);
            return;
        }

        var order = _orderService.CreateOrder(tableId.Value);
        var orderHandler = new OrderHandler(_orderService, _menuService);
        orderHandler.HandleOrderItems(order);
    }
    
    // Manage Order handler:
    public void HandleManageOrder()
    {
        AnsiConsole.Clear();
    
        var occupiedTables = _tableService.GetAllTables().Where(t => !t.Available).ToList();

        if (occupiedTables.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No occupied tables.[/]");
            AnsiConsole.MarkupLine("[grey]Press any key to go back...[/]");
            Console.ReadKey();
            return;
        }

        var tableId = DashboardView.PromptTableSelection(occupiedTables);
        if (tableId == null)
        {
            AnsiConsole.MarkupLine("[red]Going back...[/]");
            return;
        }

        var order = _orderService.GetActiveOrder(tableId.Value);

        if (order == null)
        {
            AnsiConsole.MarkupLine("[red]No active order found for this table.[/]");
            AnsiConsole.MarkupLine("[grey]Press any key to go back...[/]");
            Console.ReadKey();
            return;
        }
    
        var orderHandler = new OrderHandler(_orderService, _menuService);
        orderHandler.HandleOrderItems(order);
    }
    
}