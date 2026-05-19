using RestaurantManager.Core.Models;
using Spectre.Console;
using Table = RestaurantManager.Core.Models.Table;

namespace RestaurantManager.Customer.Views;

public static class TableView
{
    public static int? PromptTableSelection(List<Table> tables)
    {
        var choices = tables
            .Select(t => $"Table {t.TableNumber} ({t.Seats} seats)")
            .ToList();

        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select your [green]table[/]:")
                .AddChoices(choices)
                .WrapAround(true));

        return int.Parse(selected.Split(' ')[1]);
    }
}