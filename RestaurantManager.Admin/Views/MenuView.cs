using RestaurantManager.Core.Models;
using Spectre.Console;
using SpectreTable = Spectre.Console.Table;

namespace RestaurantManager.Admin.Views;

public class MenuView
{
    public static void Render(List<MenuItem> items)
    {
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine(" — [yellow]View Menu[/] — \n");

        var categories = items.Select(i => i.Category).Distinct().ToList();

        foreach (var c in categories)
        {
            var table = new SpectreTable()
                .Border(TableBorder.Square)
                .BorderColor(Color.Blue)
                .Title($"[bold blue]{c}[/]")
                .AddColumn(new TableColumn("[bold deepskyblue1]#[/]").Centered())
                .AddColumn(new TableColumn("[bold]Name[/]"))
                .AddColumn(new TableColumn("[bold]Ingredients[/]"))
                .AddColumn(new TableColumn("[bold]Allergens[/]"))
                .AddColumn(new TableColumn("[bold]Price[/]").RightAligned());

            foreach (var i in items.Where(i => i.Category == c))
            {
                var allergens = i.Allergens != null && i.Allergens.Count > 0
                    ? string.Join(", ", i.Allergens.Select(a => a.ToString()))
                    : "[grey] - [/]";
                
                var ingredients = i.Ingredients != null && i.Ingredients.Count > 0
                    ? string.Join(", ", i.Ingredients.Select(a => a.ToString()))
                    : "[grey] - [/]";

                table.AddRow(
                    $"[deepskyblue1]{i.Id.ToString()}[/]",
                    i.Name,
                    ingredients,
                    allergens,
                    $"[green]${i.Price:F2}[/]"
                );
            }
            
            AnsiConsole.Write(table);
            AnsiConsole.WriteLine();
        }
    }

    public static (MenuItem? item, int quantity) PromptItemSelection(List<MenuItem> items)
    {
        const string backOption = "<- Back";
        
        var categoryChoices = items
            .Select(i => i.Category)
            .Distinct()
            .ToList();
        
        categoryChoices.Insert(0, backOption);

        while (true)
        {
            var selectedCategory = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[blue]Select Category[/]")
                    .AddChoices(categoryChoices)
                    .WrapAround(true));

            if (selectedCategory == backOption) return (null, 0);

            var choices = items
                .FindAll(i => i.Category == selectedCategory)
                .Select(i => $"{i.Id}. {i.Name} — ${i.Price:F2}")
                .ToList();

            choices.Insert(0, backOption);

            var selected = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select an [green]item[/] to add:")
                    .AddChoices(choices)
                    .WrapAround(true));

            if (selected == backOption) continue;

            var id = int.Parse(selected.Split('.')[0]);
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
}