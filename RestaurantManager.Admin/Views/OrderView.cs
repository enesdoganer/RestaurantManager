using RestaurantManager.Core.Models;
using Spectre.Console;
using SpectreTable = Spectre.Console.Table;

namespace RestaurantManager.Admin.Views;

public class OrderView
{
    public static void Render(Order order)
    {
        AnsiConsole.MarkupLine($"[bold green]Order #{order.Id}[/] — Table {order.TableId}\n");

        var statusColor = order.Status switch
        {
            OrderStatus.Pending => "grey",
            OrderStatus.Preparing => "yellow",
            OrderStatus.Served => "green",
            OrderStatus.Paid => "blue",
            _ => "white"
        };
        
        AnsiConsole.MarkupLine($"Status: [{statusColor}]{order.Status}[/]");
        AnsiConsole.MarkupLine($"Created: [grey]{order.CreatedAt:HH:mm}[/]\n");

        if (order.Items.Count == 0)
        {
            AnsiConsole.MarkupLine("[grey]No items yet.[/]\n");
            return;
        }
        
        var table = new SpectreTable()
            .Border(TableBorder.Square)
            .BorderColor(Color.Grey)
            .AddColumn(new TableColumn("[bold]Item[/]"))
            .AddColumn(new TableColumn("[bold]Unit Price[/]").RightAligned())
            .AddColumn(new TableColumn("[bold]Qty[/]").Centered())
            .AddColumn(new TableColumn("[bold]Subtotal[/]").RightAligned());
        
        foreach (var i in order.Items)
        {
            table.AddRow(
                i.MenuItem.Name,
                $"${i.MenuItem.Price:F2}",
                i.Quantity.ToString(),
                $"[green]${i.Subtotal:F2}[/]"
            );
        }
        
        AnsiConsole.Write(table);
        AnsiConsole.MarkupLine($"\n[bold]Total: [green]${order.TotalPrice:F2}[/][/]\n");
    }
    
    public static void RenderAllOrders(List<Order> orders)
    {
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("[red]Active Orders[/]\n");

        if (orders.Count == 0)
        {
            AnsiConsole.MarkupLine("[white]No active orders.[/]");
            return;
        }
        
        AnsiConsole.Clear();
        foreach (var order in orders)
            Render(order);
    }
    
}