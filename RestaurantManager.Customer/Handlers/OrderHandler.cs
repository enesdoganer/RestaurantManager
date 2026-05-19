using RestaurantManager.Customer.Views;
using RestaurantManager.Core.Models;
using RestaurantManager.Core.Services;
using Spectre.Console;

namespace RestaurantManager.Customer.Handlers;

public class OrderHandler
{
    private readonly OrderService _orderService;
    private readonly MenuService _menuService;

    public OrderHandler(OrderService orderService, MenuService menuService)
    {
        _orderService = orderService;
        _menuService = menuService;
    }

    public void Handle(Order order)
    {
        bool active = true;

        while (active)
        {
            OrderView.Render(order);

            var action = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("What would you like to do?")
                    .AddChoices(
                        "Add Item",
                        "View Menu",
                        "Request Bill",
                        "<- Change Table")
                    .WrapAround(true));

            switch (action)
            {
                case "Add Item":
                    HandleAddItem(order);
                    break;

                case "View Menu":
                    AnsiConsole.Clear();
                    MenuView.Render(_menuService.GetAllItems());
                    AnsiConsole.MarkupLine("[grey]Press any key to go back...[/]");
                    Console.ReadKey();
                    break;

                case "Request Bill":
                    HandleRequestBill(order);
                    active = false;
                    break;

                case "<- Change Table":
                    if (order.Items.Count == 0)
                    {
                        // No items yet so safe to cancel the order
                        _orderService.CloseOrder(order);
                        active = false;
                    }
                    else
                    {
                        var confirm = AnsiConsole.Prompt(
                            new ConfirmationPrompt("You have items in your order. Are you sure you want to change table? Your order will be lost."));
                        if (confirm)
                        {
                            _orderService.CloseOrder(order);
                            active = false;
                        }
                    }
                    break;
            }
        }
    }

    private void HandleAddItem(Order order)
    {
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine($"[bold purple]Add Item[/] — Table {order.Table?.TableNumber ?? order.TableId}\n");

        var (menuItem, quantity) = MenuView.PromptItemSelection(_menuService.GetAllItems());
        if (menuItem == null) return;

        _orderService.AddItem(order, menuItem, quantity);
        AnsiConsole.MarkupLine($"[green]Added {quantity}x {Markup.Escape(menuItem.Name)}[/]");
        Thread.Sleep(800);
    }

    private void HandleRequestBill(Order order)
    {
        AnsiConsole.Clear();
        OrderView.Render(order);
        AnsiConsole.MarkupLine($"[bold]Final total: [green]${order.TotalPrice:F2}[/][/]\n");
        AnsiConsole.MarkupLine("[grey]Your bill has been requested. A member of staff will be with you shortly.[/]");

        _orderService.UpdateStatus(order, OrderStatus.BillRequested);
        Thread.Sleep(2000);
    }
}