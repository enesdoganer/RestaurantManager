using RestaurantManager.Core.Models;
using Spectre.Console;
using SpectreTable = Spectre.Console.Table;

namespace RestaurantManager.Customer.Views;

public static class CustomerMenuView
{
    public static void Render(List<MenuItem> items)
    {
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("[bold purple]Our Menu[/]\n");

        var categories = items.Select(i => i.Category).Distinct();

        foreach (var category in categories)
        {
            var table = new SpectreTable()
                .Border(TableBorder.Rounded)
                .BorderColor(Color.Grey)
                .Title($"[yellow]{category}[/]")
                .AddColumn(new TableColumn("[bold]#[/]").Centered())
                .AddColumn(new TableColumn("[bold]Name[/]"))
                .AddColumn(new TableColumn("[bold]Ingredients[/]"))
                .AddColumn(new TableColumn("[bold]Allergens[/]"))
                .AddColumn(new TableColumn("[bold]Price[/]").RightAligned());

            foreach (var item in items.Where(i => i.Category == category))
            {
                var allergens = item.Allergens.Count > 0
                    ? Markup.Escape(string.Join(", ", item.Allergens))
                    : "[grey]None[/]";

                table.AddRow(
                    new Markup(item.ItemNumber.ToString()),
                    new Markup(Markup.Escape(item.Name)),
                    new Markup(Markup.Escape(string.Join(", ", item.Ingredients))),
                    new Markup(allergens),
                    new Markup($"[green]${item.Price:F2}[/]"));
            }

            AnsiConsole.Write(table);
            AnsiConsole.WriteLine();
        }
    }

    public static (MenuItem? item, int quantity) PromptItemSelection(List<MenuItem> items)
    {
        const string backOption = "<- Back";

        var categoryChoices = items.Select(i => i.Category).Distinct().ToList();
        categoryChoices.Insert(0, backOption);

        while (true)
        {
            var selectedCategory = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[blue]Select category[/]:")
                    .AddChoices(categoryChoices)
                    .WrapAround(true));

            if (selectedCategory == backOption) return (null, 0);

            var choices = items
                .Where(i => i.Category == selectedCategory)
                .Select(i => $"{i.ItemNumber}. {i.Name} — ${i.Price:F2}")
                .ToList();
            choices.Insert(0, backOption);

            var selected = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select an [green]item[/] to add:")
                    .AddChoices(choices)
                    .WrapAround(true));

            if (selected == backOption) continue;

            var itemNumber = int.Parse(selected.Split('.')[0]);
            var menuItem = items.First(i => i.ItemNumber == itemNumber);

            var quantity = AnsiConsole.Prompt(
                new TextPrompt<int>("How many?")
                    .DefaultValue(1)
                    .ValidationErrorMessage("[red]Please enter a valid number[/]")
                    .Validate(q => q > 0
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Quantity must be at least 1[/]")));

            return (menuItem, quantity);
        }
    }
}