using Microsoft.Extensions.DependencyInjection;
using RestaurantManager.Customer.Views;
using RestaurantManager.Core.Models;
using RestaurantManager.Core.Services;
using Spectre.Console;

namespace RestaurantManager.Customer.Handlers;

public class CustomerOrderHandler
{
    private readonly ServiceLocator _locator;

    public CustomerOrderHandler(ServiceLocator locator)
    {
        _locator = locator;
    }

    public void Handle(Order order)
    {
        bool active = true;

        while (active)
        {
            var freshOrder = _locator.GetOrderService().GetOrderById(order.Id);
            if (freshOrder == null) return;
            
            if (freshOrder.Status == OrderStatus.Paid)
            {
                AnsiConsole.Clear();
                AnsiConsole.Write(
                    new FigletText("Thank You!")
                        .Centered()
                        .Color(Color.Green));
                AnsiConsole.MarkupLine("[grey]We hope to see you again soon![/]".PadLeft(40));
                Thread.Sleep(3000);
                return;
            }
            
            CustomerOrderView.Render(freshOrder);
            
            AnsiConsole.MarkupLine("[grey]Screen refreshes every 10 seconds. Press any key to act now.[/]");
            
            var sw = System.Diagnostics.Stopwatch.StartNew();
            bool keyPressed = false;
            
            while (sw.Elapsed.TotalSeconds < 10)
            {
                if (Console.KeyAvailable)
                {
                    Console.ReadKey(true);
                    keyPressed = true;
                    break;
                }
                Thread.Sleep(200);
            }
            
            if (!keyPressed) continue;

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
                    HandleAddItem(freshOrder);
                    break;

                case "View Menu":
                    AnsiConsole.Clear();
                    CustomerMenuView.Render(_locator.GetMenuService().GetAllItems());
                    AnsiConsole.MarkupLine("[grey]Press any key to go back...[/]");
                    Console.ReadKey();
                    break;

                case "Request Bill":
                    HandleRequestBill(freshOrder);
                    active = false;
                    break;

                case "<- Change Table":
                    if (order.Items.Count == 0)
                    {
                        _locator.GetOrderService().CloseOrder(freshOrder);
                        active = false;
                    }
                    else
                    {
                        var confirm = AnsiConsole.Prompt(
                            new ConfirmationPrompt("You have items in your order. Are you sure you want to change table?"));
                        if (confirm)
                        {
                            _locator.GetOrderService().CloseOrder(freshOrder);
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

        var (menuItem, quantity) = CustomerMenuView.PromptItemSelection(_locator.GetMenuService().GetAllItems());
        if (menuItem == null) return;

        _locator.GetOrderService().AddItem(order, menuItem, quantity);
        AnsiConsole.MarkupLine($"[green]Added {quantity}x {Markup.Escape(menuItem.Name)}[/]");
        Thread.Sleep(800);
    }

    private void HandleRequestBill(Order order)
    {
        AnsiConsole.Clear();
        CustomerOrderView.Render(order);
        AnsiConsole.MarkupLine($"[bold]Final total: [green]${order.TotalPrice:F2}[/][/]\n");
        _locator.GetOrderService().UpdateStatus(order, OrderStatus.BillRequested);
        AnsiConsole.MarkupLine("[yellow]Bill requested! A member of staff will be with you shortly.[/]\n");
    
        // Wait until admin marks as paid
        AnsiConsole.MarkupLine("[grey]Please wait...[/]");
    
        while (true)
        {
            Thread.Sleep(3000);
            var freshOrder = _locator.GetOrderService().GetOrderById(order.Id);
            if (freshOrder?.Status == OrderStatus.Paid)
            {
                AnsiConsole.Clear();
                AnsiConsole.Write(
                    new FigletText("Thank You!")
                        .Centered()
                        .Color(Color.Green));
                AnsiConsole.MarkupLine("[grey]We hope to see you again soon![/]".PadLeft(40));
                Thread.Sleep(3000);
                return;
            }
        }
    }
}