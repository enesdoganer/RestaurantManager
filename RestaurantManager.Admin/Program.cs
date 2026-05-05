using System.Diagnostics;
using RestaurantManager.Core.Data;
using RestaurantManager.Core.Services;
using RestaurantManager.Core.Models;
using RestaurantManager.Admin.Views;
using Spectre.Console;
using Microsoft.Extensions.Configuration;

// Get connection string from appsettings.json
var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false)
    .Build();
var connectionString = config.GetConnectionString("DefaultConnection")
                       ?? throw new Exception("Connection string not found.");

// Create db and create the tables if they do not exist:
var db = DbContextFactory.Create(connectionString);
db.Database.EnsureCreated();

// Seed initial data
DbSeeder.Seed(db);

// Services
var tableService = new TableService(db);
var menuService = new MenuService(db);
var orderService = new OrderService(db, tableService);

// Splash screen
AnsiConsole.Clear();
AnsiConsole.Write(
    new FigletText("Restaurant Manager")
        .Centered()
        .Color(Color.MediumPurple));

AnsiConsole.MarkupLine("[white]Press any key to continue...[/]");
Console.ReadKey();

// Main loop
bool running = true;

while (running)
{
    DashboardView.Render(tableService.GetAllTables());

    var choice = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("What would you like to do?")
            .AddChoices(
                "New Order",
                "Manage Existing Order",
                "View All Active Orders",
                "View Menu",
                "Exit"));

    switch (choice)
    {
        case "New Order":
            HandleNewOrder();
            break;

        case "Manage Existing Order":
            HandleManageOrder();
            break;

        case "View All Active Orders":
            OrderView.RenderAllOrders(orderService.GetActiveOrders());
            AnsiConsole.MarkupLine("[grey]Press any key to go back...[/]");
            Console.ReadKey();
            break;

        case "View Menu":
            MenuView.Render(menuService.GetAllItems());
            AnsiConsole.MarkupLine("[grey]Press any key to go back...[/]");
            Console.ReadKey();
            break;

        case "Exit":
            running = false;
            AnsiConsole.MarkupLine("[grey]Goodbye![/]");
            break;
    }
}

// New Order handler:
void HandleNewOrder()
{
    var freeTables = tableService.GetAllTables().Where(t => t.Available).ToList();

    if (freeTables.Count == 0)
    {
        AnsiConsole.MarkupLine("[red]No free tables available.[/]");
        AnsiConsole.MarkupLine("[grey]Press any key to go back...[/]");
        Console.ReadKey();
        return;
    }

    var tableId = DashboardView.PromptTableSelection(freeTables);
    var order = orderService.CreateOrder(tableId);

    HandleOrderItems(order);
}

// Manage Order handler:
void HandleManageOrder()
{
    var occupiedTables = tableService.GetAllTables().Where(t => !t.Available).ToList();

    if (occupiedTables.Count == 0)
    {
        AnsiConsole.MarkupLine("[red]No occupied tables.[/]");
        AnsiConsole.MarkupLine("[grey]Press any key to go back...[/]");
        Console.ReadKey();
        return;
    }

    var tableId = DashboardView.PromptTableSelection(occupiedTables);
    var order = orderService.GetActiveOrder(tableId);

    if (order == null)
    {
        AnsiConsole.MarkupLine("[red]No active order found for this table.[/]");
        AnsiConsole.MarkupLine("[grey]Press any key to go back...[/]");
        Console.ReadKey();
        return;
    }
    
    HandleOrderItems(order);
}

// Order Items handler:
void HandleOrderItems(Order order)
{
    bool managing = true;

    while (managing)
    {
        OrderView.Render(order);

        var action = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("What would you like to do?")
                .AddChoices(
                    "Add Item",
                    "Remove Item",
                    "Update Order Status",
                    "Close & Pay",
                    "Back to Dashboard"));

        switch (action)
        {
            case "Add Item":
                var (menuItem, quantity) = MenuView.PromptItemSelection(menuService.GetAllItems());
                orderService.AddItem(order, menuItem, quantity);
                AnsiConsole.MarkupLine($"[green]Added {quantity}x {menuItem.Name}[/]");
                Thread.Sleep(800);
                break;

            case "Remove Item":
                if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Preparing)
                {
                    AnsiConsole.MarkupLine("[red]Can not remove item at this time.[/]");
                    Thread.Sleep(800);
                    break;
                }
                if (order.Items.Count == 0)
                {
                    AnsiConsole.MarkupLine("[red]No items to remove.[/]");
                    Thread.Sleep(800);
                    break;
                }

                var itemChoices = order.Items
                    .Select(i => $"{i.MenuItem.Name} x{i.Quantity}")
                    .ToList();

                var selectedItem = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Select item to [red]remove[/]:")
                        .AddChoices(itemChoices));

                var itemName = selectedItem.Split(" x")[0];
                var itemToRemove = order.Items.First(i => i.MenuItem.Name == itemName);
                orderService.RemoveItem(order, itemToRemove.MenuItem.Id);
                AnsiConsole.MarkupLine($"[red]Removed {itemName}[/]");
                Thread.Sleep(800);
                break;

            case "Update Order Status":
                var newStatus = AnsiConsole.Prompt(
                    new SelectionPrompt<OrderStatus>()
                        .Title("Select new [yellow]status[/]:")
                        .AddChoices(
                            OrderStatus.Pending,
                            OrderStatus.Preparing,
                            OrderStatus.Served));

                orderService.UpdateStatus(order, newStatus);
                AnsiConsole.MarkupLine($"[green]Status updated to {newStatus}[/]");
                Thread.Sleep(800);
                break;

            case "Close & Pay":
                OrderView.Render(order);
                AnsiConsole.MarkupLine($"[bold]Final total: [green]${order.TotalPrice:F2}[/][/]");

                var confirm = AnsiConsole.Prompt(
                    new ConfirmationPrompt("Confirm payment and close table?"));

                if (confirm)
                {
                    orderService.CloseOrder(order);
                    AnsiConsole.MarkupLine("[green]Order closed. Table is now free.[/]");
                    Thread.Sleep(1000);
                    managing = false;
                }
                break;

            case "Back to Dashboard":
                managing = false;
                break;
        }
    }
}       