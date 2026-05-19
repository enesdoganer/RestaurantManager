using RestaurantManager.Core.Models;
using Spectre.Console;
using SpectreTable = Spectre.Console.Table;

namespace RestaurantManager.Customer.Views;

public static class OrderView
{
    public static void Render(Order order)
    {
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine($"[bold purple]Your Order[/] — Table {order.Table?.TableNumber ?? order.TableId}\n");

        var statusColor = order.Status switch
        {
            OrderStatus.Pending         => "grey",
            OrderStatus.Preparing       => "yellow",
            OrderStatus.Served          => "green",
            OrderStatus.BillRequested   => "orange",
            OrderStatus.Paid            => "blue",
            _                           => "white"
        };

        AnsiConsole.MarkupLine($"Status: [{statusColor}]{order.Status}[/]\n");

        if (order.Items.Count == 0)
        {
            AnsiConsole.MarkupLine("[grey]No items yet.[/]\n");
            return;
        }

        var table = new SpectreTable()
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Grey)
            .AddColumn(new TableColumn("[bold]Item[/]"))
            .AddColumn(new TableColumn("[bold]Unit Price[/]").RightAligned())
            .AddColumn(new TableColumn("[bold]Qty[/]").Centered())
            .AddColumn(new TableColumn("[bold]Subtotal[/]").RightAligned());

        foreach (var item in order.Items)
        {
            table.AddRow(
                Markup.Escape(item.MenuItem.Name),
                $"${item.MenuItem.Price:F2}",
                item.Quantity.ToString(),
                $"[green]${item.Subtotal:F2}[/]");
        }

        AnsiConsole.Write(table);
        AnsiConsole.MarkupLine($"\n[bold]Total: [green]${order.TotalPrice:F2}[/][/]\n");
    }
}