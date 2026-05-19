using RestaurantManager.Admin.Views;
using RestaurantManager.Core.Models;
using RestaurantManager.Core.Services;
using Spectre.Console;

namespace RestaurantManager.Admin.Handlers;

public class MenuHandler
{
    private readonly MenuService _menuService;
    
    private const string BackOption = "<- Back";

    public MenuHandler(MenuService menuService)
    {
        _menuService = menuService;
    }

    public void HandleViewMenu()
    {
        bool inMenu = true;
        while (inMenu)
        {
            AnsiConsole.Clear();
            MenuView.Render(_menuService.GetAllItems());

            var action = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("What would you like to do?")
                    .AddChoices(
                        "<- Back",
                        "Menu Management")
                    .WrapAround(true));

            switch (action)
            {
                case "<- Back":
                    inMenu = false;
                    break;
                
                case "Menu Management":
                    if (!VerifyToken()) break;
                    HandleManageMenu();
                    break;
            }
        }
    }

    private void HandleManageMenu()
    {
        bool inManage = true;

        while (inManage)
        {
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine("[bold purple]Menu Management[/]\n");

            var action = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select an action:")
                    .AddChoices(
                        BackOption,
                        "Add Menu Item",
                        "Edit Menu Item",
                        "Remove Menu Item")
                    .WrapAround(true));

            switch (action)
            {
                case "<- Back":
                    inManage = false;
                    break;
                
                case "Add Menu Item":
                    HandleAddMenuItem();
                    break;

                case "Edit Menu Item":
                    HandleEditMenuItem();
                    break;

                case "Remove Menu Item":
                    HandleRemoveMenuItem();
                    break;
            }
        }
    }

    private static bool VerifyToken()
    {
        var token = LoginView.PromptToken();
        if (AuthService.ValidateToken(token)) return true;

        AnsiConsole.MarkupLine("[red]Invalid token. Action cancelled![/]");
        Thread.Sleep(1500);
        return false;
    }

    private void HandleAddMenuItem()
    {
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("[bold purple]Add Menu Item[/]\n");
        AnsiConsole.MarkupLine("[grey]Type '!b' at any prompt to cancel.[/]\n");
        
        const string backCommand = "!b";

        var name = AnsiConsole.Prompt(
            new TextPrompt<string>("Item [green]name[/]:"));
        if (name == backCommand) return;

        var category = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select [green]category[/]:")
                .AddChoices("Starters", "Mains", "Desserts", "Drinks")
                .WrapAround(true));

        var ingredientsInput = AnsiConsole.Prompt(
            new TextPrompt<string>("Ingredients (space separated):"));
        if (ingredientsInput == backCommand) return;
        var ingredients = ingredientsInput.Split(' ').ToList();

        var allergenChoices = Enum.GetNames<Allergen>().ToList();
        var selectedAllergens = AnsiConsole.Prompt(
            new MultiSelectionPrompt<string>()
                .Title("Select [yellow]allergens[/] (space to select, enter to confirm):")
                .NotRequired()
                .AddChoices(allergenChoices));

        var allergens = selectedAllergens
            .Select(a => Enum.Parse<Allergen>(a))
            .ToList();

        var price = AnsiConsole.Prompt(
            new TextPrompt<decimal>("Price:")
                .ValidationErrorMessage("[red]Please enter a valid price[/]")
                .Validate(p => p > 0
                    ? ValidationResult.Success()
                    : ValidationResult.Error("[red]Price must be greater than 0[/]")));
        if (price.ToString() == backCommand) return;

        var newItem = new MenuItem(name, ingredients, allergens, price, category);
        _menuService.AddItem(newItem);

        AnsiConsole.MarkupLine($"[green]'{Markup.Escape(name)}' added to the menu.[/]");
        Thread.Sleep(1000);
    }

    private void HandleEditMenuItem()
    {
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("[bold purple]Edit Menu Item[/]\n");
        
        var items = _menuService.GetAllItems();

        var choices = items
            .Select(i => $"{i.Id}. {i.Name} — ${i.Price:F2}")
            .ToList();
        choices.Insert(0, BackOption);

        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select item to [yellow]edit[/]:")
                .AddChoices(choices)
                .WrapAround(true));

        if (selected == BackOption) return;

        var id = int.Parse(selected.Split('.')[0]);
        var item = items.First(i => i.Id == id);

        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine($"[bold purple]Editing: {Markup.Escape(item.Name)}[/]\n");
            AnsiConsole.MarkupLine($"1. Name        — [green]{Markup.Escape(item.Name)}[/]");
            AnsiConsole.MarkupLine($"2. Category    — [green]{item.Category}[/]");
            AnsiConsole.MarkupLine($"3. Ingredients — [green]{Markup.Escape(string.Join(", ", item.Ingredients))}[/]");
            AnsiConsole.MarkupLine(
                $"4. Allergens   — [green]{(item.Allergens.Count > 0 ? string.Join(", ", item.Allergens) : "None")}[/]");
            AnsiConsole.MarkupLine($"5. Price       — [green]${item.Price:F2}[/]\n");

            var field = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("What would you like to edit?")
                    .AddChoices(
                        "Name",
                        "Category",
                        "Ingredients",
                        "Allergens",
                        "Price",
                        "Save & Exit")
                    .WrapAround(true));

            switch (field)
            {
                case "Name":
                    var name = AnsiConsole.Prompt(
                        new TextPrompt<string>($"New name [[{Markup.Escape(item.Name)}]]:"));
                    item.Name = name;
                    break;

                case "Category":
                    item.Category = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("Select [green]category[/]:")
                            .AddChoices("Starters", "Mains", "Desserts", "Drinks")
                            .WrapAround(true));
                    break;

                case "Ingredients":
                    var ingredientsInput = AnsiConsole.Prompt(
                        new TextPrompt<string>(
                            $"New ingredients [[{Markup.Escape(string.Join(' ', item.Ingredients))}]]:"));
                    item.Ingredients = ingredientsInput.Split(' ').ToList();
                    break;

                case "Allergens":
                    // var previousAllergens = item.Allergens.Select(a => a.ToString()).ToList();
                    var selectedAllergens = AnsiConsole.Prompt(
                        new MultiSelectionPrompt<string>()
                            .Title("Select [yellow]allergens[/]:")
                            .NotRequired()
                            .AddChoices(Enum.GetNames<Allergen>().ToList()));
                    item.Allergens = selectedAllergens
                        .Select(a => Enum.Parse<Allergen>(a))
                        .ToList();
                    break;

                case "Price":
                    item.Price = AnsiConsole.Prompt(
                        new TextPrompt<decimal>($"New price [[{item.Price:F2}]]:")
                            .ValidationErrorMessage("[red]Please enter a valid price[/]")
                            .Validate(p => p > 0
                                ? ValidationResult.Success()
                                : ValidationResult.Error("[red]Price must be greater than 0[/]")));
                    break;

                case "Save & Exit":
                    _menuService.UpdateItem(item);
                    AnsiConsole.MarkupLine($"[green]'{Markup.Escape(item.Name)}' saved.[/]");
                    Thread.Sleep(1000);
                    return;
            }

            AnsiConsole.MarkupLine("[grey]Change applied.[/]");
            Thread.Sleep(800);
        }
    }
    
    private void HandleRemoveMenuItem()
    {
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("[bold purple]Remove Menu Item[/]\n");
        
        var items = _menuService.GetAllItems();

        var choices = items
            .Select(i => $"{i.Id}. {i.Name} — ${i.Price:F2}")
            .ToList();
        choices.Insert(0, BackOption);

        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select item to [red]remove[/]:")
                .AddChoices(choices)
                .WrapAround(true));

        if (selected == BackOption) return;

        var id = int.Parse(selected.Split('.')[0]);
        var item = items.First(i => i.Id == id);

        var confirm = AnsiConsole.Prompt(
            new ConfirmationPrompt($"Are you sure you want to remove '[red]{Markup.Escape(item.Name)}[/]'?"));

        if (!confirm) return;

        _menuService.RemoveItem(item);
        AnsiConsole.MarkupLine($"[red]'{Markup.Escape(item.Name)}' removed from the menu.[/]");
        Thread.Sleep(1000);
    }
    
}