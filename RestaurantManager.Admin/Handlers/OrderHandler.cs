using RestaurantManager.Admin.Views;
using RestaurantManager.Core.Models;
using RestaurantManager.Core.Services;
using Spectre.Console;

namespace RestaurantManager.Admin.Handlers;

public class OrderHandler
{
    private readonly ServiceLocator _locator;
    
    private const String BackOption = "<- Back";

    public OrderHandler(ServiceLocator locator)
    {
        _locator = locator;
    }

    public void HandleViewActiveOrders()
    {
        AnsiConsole.Clear();
        OrderView.RenderAllOrders(_locator.GetOrderService().GetActiveOrders());
        AnsiConsole.MarkupLine("[grey]Press any key to go back...[/]");
        Console.ReadKey();
    }

    public void HandleViewPastOrders()
    {
        AnsiConsole.Clear();
        OrderView.RenderAllOrders(_locator.GetOrderService().GetPastOrders());
        AnsiConsole.MarkupLine("[grey]Press any key to go back...[/]");
        Console.ReadKey();
    }

    public void HandleOrderItems(Order order)
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
                        "Back to Dashboard")
                    .WrapAround(true));

            switch (action)
            {
                case "Add Item":
                    HandleAddItem(order);
                    break;

                case "Remove Item":
                    HandleRemoveItem(order);
                    break;

                case "Update Order Status":
                    HandleUpdateStatus(order);
                    continue;
                    break;

                case "Close & Pay":
                    if (HandleClosePay(order)) managing = false;
                    break;

                case "Back to Dashboard":
                    managing = false;
                    break;
            }
        }
    }

    private void HandleAddItem(Order order)
    {
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine(
            $"[bold purple]Order #{order.Id}[/] — Table {order.TableId} — [green]Add Item[/]\n");
        var (menuItem, quantity) = MenuView.PromptItemSelection(_locator.GetMenuService()!.GetAllItems());
        if (menuItem == null)
        {
            AnsiConsole.MarkupLine("[red]Going back...[/]");
            Thread.Sleep(800);
            AnsiConsole.Clear();
            return;
        }

        _locator.GetOrderService().AddItem(order, menuItem, quantity);
        AnsiConsole.MarkupLine($"[green]Added {quantity}x {menuItem.Name}[/]");
        Thread.Sleep(800);
        AnsiConsole.Clear();
    }

    private void HandleRemoveItem(Order order)
    {
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine($"[bold purple]Order #{order.Id}[/] — Table {order.TableId} — [red]Remove Item[/]\n");

        if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Preparing)
        {
            AnsiConsole.MarkupLine("[red]Cannot remove items at this stage.[/]");
            Thread.Sleep(800);
            return;
        }

        if (order.Items.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No items to remove.[/]");
            Thread.Sleep(800);
            AnsiConsole.Clear();
            return;
        }
        
        var itemChoices = order.Items
            .Select(i => $"{i.MenuItem.Name} x{i.Quantity}")
            .ToList();
        itemChoices.Insert(0, BackOption);

        var selectedItem = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select item to [red]remove[/]:")
                .AddChoices(itemChoices)
                .WrapAround(true));

        if (selectedItem == BackOption) return;

        var itemName = selectedItem.Split(" x")[0];
        var itemToRemove = order.Items.First(i => i.MenuItem.Name == itemName);
        _locator.GetOrderService().RemoveItem(order, itemToRemove.MenuItem.Id);
        AnsiConsole.MarkupLine($"[red]Removed {Markup.Escape(itemName)}[/]");
        Thread.Sleep(800);
        AnsiConsole.Clear();
    }

    private void HandleUpdateStatus(Order order)
    {
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine(
            $"[bold purple]Order #{order.Id}[/] — Table {order.TableId} — [yellow]Update Status[/]\n");
        
        var choices = new List<string>
        {
            BackOption,
            nameof(OrderStatus.Pending),
            nameof(OrderStatus.Preparing),
            nameof(OrderStatus.Served),
            nameof(OrderStatus.BillRequested)
        };

        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<String>()
                .Title("Select new [yellow]status[/]:")
                .AddChoices(choices)
                .WrapAround(true));
        
        if (selected == BackOption) return;
        
        var newStatus = Enum.Parse<OrderStatus>(selected);
        _locator.GetOrderService().UpdateStatus(order, newStatus);
        AnsiConsole.MarkupLine($"[green]Status updated to {newStatus}[/]");
        Thread.Sleep(800);
        AnsiConsole.Clear();
    }

    private bool HandleClosePay(Order order)
    {
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine($"[bold purple]Order #{order.Id}[/] — Table {order.TableId} — [red]Close & Pay[/]\n");

        OrderView.Render(order);
        AnsiConsole.MarkupLine($"[bold]Final total: [green]${order.TotalPrice:F2}[/][/]");

        var confirm = AnsiConsole.Prompt(
            new ConfirmationPrompt("Confirm payment and close table?"));

        if (!confirm)
        {
            AnsiConsole.MarkupLine("[red]Your order has been closed[/]");
            Thread.Sleep(800);
            AnsiConsole.Clear();
            return false;
        }

        _locator.GetOrderService().CloseOrder(order);
        AnsiConsole.MarkupLine("[green]Order closed. Table is now free.[/]");
        Thread.Sleep(1000);
        AnsiConsole.Clear();
        return true;
    }
}