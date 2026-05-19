using Spectre.Console;
using SpectreTable = Spectre.Console.Table;
using Table = RestaurantManager.Core.Models.Table;

namespace RestaurantManager.Admin.Views;

public static class DashboardView
{
    public static void Render(List<Table> tables)
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(
            new FigletText("Restaurant Manager")
                .Centered()
                .Color(Color.MediumPurple));
        AnsiConsole.WriteLine();
        var table = new SpectreTable()
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Blue)
            .AddColumns(new TableColumn("[bold]Table[/]").Centered(), 
                        new TableColumn("[bold]Seats[/]").Centered(),
                        new TableColumn("[bold]Status[/]").Centered(),
                        new TableColumn("[bold]Order ID[/]").Centered());
        foreach (var t in tables)
        {
            var status = t.Available ? "[green]Free[/]" : "[red]Occupied[/]";
            
            var orderId = t.ActiveOrderId.HasValue
                ? $"#{t.ActiveOrderId}"
                : "[grey] - [/]";

            table.AddRow(
                $"Table {t.TableNumber}",
                t.Seats.ToString(),
                status,
                orderId
            );
        }
        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();
    }

    // returns the id of the chosen table
    public static int? PromptTableSelection(List<Table> tables)
    {
        const string backOption = "<- Back";
        
        var choices = tables
            .Select(t => $"Table {t.TableNumber} ({t.Seats} seats) - {(t.Available ? "Free" : "Occupied")}")
            .ToList();
        choices.Insert(0, backOption);

        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select a [purple]table[/]:")
                .AddChoices(choices)
                .WrapAround(true));
        
        if (selected == backOption) return null;
        
        return int.Parse(selected.Split(' ')[1]);
    }
    
}