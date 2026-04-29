using RestaurantManager.Models;
using Spectre.Console;
using SpectreTable = Spectre.Console.Table;

namespace RestaurantManager.Views;

public class MenuView
{
    public static void Render(List<MenuItem> items)
    {
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("[yellow]Menu[/]\n");

        var categories = items.Select(i => i.Category).Distinct().ToList();

        foreach (var c in categories)
        {
            var table = new SpectreTable()
                .Border(TableBorder.Simple)
                .BorderColor(Color.BlueViolet)
                .Title($"[orange]{c}[/]")
                .AddColumn(new TableColumn("[bold]#[/]").Centered())
                .AddColumn(new TableColumn("[bold]Name[/]"))
                .AddColumn(new TableColumn("[bold]Ingredients[/]"))
                .AddColumn(new TableColumn("[bold]Allergens[/]"))
                .AddColumn(new TableColumn("[bold]Price[/]").RightAligned());

            foreach (var i in items.Where(i => i.Category == c))
            {
                var allergens = i.Allergens.Count > 0
                    ? string.Join(", ", i.Allergens.Select(a => a.ToString()))
                    : "[grey] - [/]";

                table.AddRow(
                    i.Id.ToString(),
                    i.Name,
                    i.Ingredients,
                    allergens,
                    $"[green]${i.Price:F2}[/]"
                );
            }
            
            AnsiConsole.Write(table);
            AnsiConsole.WriteLine();
        }
    }

    public static (MenuItem item, int quantity) PromptItemSelection(List<MenuItem> items)
    {
        var choices = items
            .Select(i => $"[{i.Id}] {i.Name} — ${i.Price:F2}")
            .ToList();

        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select an [green]item[/] to add:")
                .AddChoices(choices));
        
        var id = int.Parse(selected.Split(']')[0].TrimStart('['));
        var menuItem = items.First(i => i.Id == id);

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