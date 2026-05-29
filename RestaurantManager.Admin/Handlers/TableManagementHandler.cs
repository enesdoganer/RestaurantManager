using RestaurantManager.Admin.Views;
using RestaurantManager.Core.Models;
using RestaurantManager.Core.Services;
using Spectre.Console;
using Table = RestaurantManager.Core.Models.Table;

namespace RestaurantManager.Admin.Handlers;

public class TableManagementHandler
{
    private readonly ServiceLocator _locator;

    private const String BackOption = "<- Back";

    public TableManagementHandler(ServiceLocator locator)
    {
        _locator = locator;
    }

    public void Handle()
    {
        bool inMenu = true;

        while (inMenu)
        {
            AnsiConsole.Clear();
            DashboardView.Render(_locator.GetTableService().GetAllTables());

            var action = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("What would you like to do?")
                    .AddChoices(
                        BackOption,
                        "Table Management")
                    .WrapAround(true));

            switch (action)
            {
                case BackOption:
                    inMenu = false;
                    break;
                
                case "Table Management":
                    if(!VerifyToken()) break;
                    HandleManagement();
                    break;
            }
        }
    }

    private void HandleManagement()
    {
        var inManage = true;

        while (inManage)
        {
            AnsiConsole.Clear();
            DashboardView.Render(_locator.GetTableService().GetAllTables());
            AnsiConsole.MarkupLine("[bold purple]Table Management[/]\n");

            var action = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select an action:")
                    .AddChoices(
                        BackOption,
                        "Add Table",
                        "Edit Table",
                        "Remove Table")
                    .WrapAround(true));

            switch (action)
            {
                case BackOption:
                    inManage = false;
                    break;
                
                case "Add Table":
                    HandleAddTable();
                    break;
                
                case "Edit Table":
                    HandleEditTable();
                    break;
                
                case "Remove Table":
                    HandleRemoveTable();
                    break;
            }
        }
    }

    private static bool VerifyToken()
    {
        var token = LoginView.PromptToken();
        if (AuthService.ValidateToken(token)) return true;

        AnsiConsole.MarkupLine("[red]Invalid token. Action cancelled.[/]");
        Thread.Sleep(1500);
        return false;
    }

    private void HandleAddTable()
    {
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("[bold purple]Add Table[/]\n");

        var seats = AnsiConsole.Prompt(
            new SelectionPrompt<int>()
                .Title("Select number of [green]seats[/]:")
                .AddChoices(2, 4, 6, 8)
                .WrapAround(true));

        var nextTableNumber = _locator.GetTableService().GetAllTables().Max(t => t.TableNumber) + 1;
        var newTable = new Table(seats);
        _locator.GetTableService().AddTable(newTable);

        AnsiConsole.MarkupLine($"[green]Table {nextTableNumber} with {seats} seats added.[/]");
        Thread.Sleep(1000);
    }

    private void HandleEditTable()
    {
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("[bold purple]Edit Table[/]\n");
        
        var tables = _locator.GetTableService().GetAllTables();

        var choices = tables
            .Select(t => $"Table {t.TableNumber} ({t.Seats} seats) — {(t.Available ? "Free" : "Occupied")}")
            .ToList();
        choices.Insert(0, BackOption);

        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select table to [yellow]edit[/]:")
                .AddChoices(choices)
                .WrapAround(true));

        if (selected == BackOption) return;

        var tableNumber = int.Parse(selected.Split(' ')[1]);
        var table = tables.First(t => t.TableNumber == tableNumber);

        if (!table.Available)
        {
            AnsiConsole.MarkupLine("[red]Cannot edit an occupied table.[/]");
            Thread.Sleep(1500);
            return;
        }

        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine($"[bold purple]Editing: Table {table.TableNumber}[/]\n");
            AnsiConsole.MarkupLine($"1. Seats — [green]{table.Seats}[/]\n");

            var field = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("What would you like to edit?")
                    .AddChoices(
                        BackOption,
                        "Seats",
                        "Save & Exit")
                    .WrapAround(true));

            switch (field)
            {
                case BackOption:
                    return;
                
                case "Seats":
                    table.Seats = AnsiConsole.Prompt(
                        new SelectionPrompt<int>()
                            .Title("Select number of [green]seats[/]:")
                            .AddChoices(2, 4, 6, 8)
                            .WrapAround(true));
                    AnsiConsole.MarkupLine("[grey]Change applied.[/]");
                    Thread.Sleep(500);
                    break;

                case "Save & Exit":
                    _locator.GetTableService().UpdateTable(table);
                    AnsiConsole.MarkupLine($"[green]Table {table.TableNumber} updated.[/]");
                    Thread.Sleep(1000);
                    return;
                
            }
        }
    }

    private void HandleRemoveTable()
    {
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("[bold purple]Remove Table[/]\n");
        
        var tables = _locator.GetTableService().GetAllTables();

        var choices = tables
            .Select(t => $"Table {t.TableNumber} ({t.Seats} seats) — {(t.Available ? "Free" : "Occupied")}")
            .ToList();
        choices.Insert(0, BackOption);

        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select table to [red]remove[/]:")
                .AddChoices(choices)
                .WrapAround(true));

        if (selected == BackOption) return;

        var tableNumber = int.Parse(selected.Split(' ')[1]);
        var table = tables.First(t => t.TableNumber == tableNumber);

        if (!table.Available)
        {
            AnsiConsole.MarkupLine("[red]Cannot remove an occupied table.[/]");
            Thread.Sleep(1500);
            return;
        }

        var confirm = AnsiConsole.Prompt(
            new ConfirmationPrompt($"Are you sure you want to remove Table {table.TableNumber}?"));

        if (!confirm) return;

        _locator.GetTableService().RemoveTable(table);
        AnsiConsole.MarkupLine($"[red]Table {table.Id} removed.[/]");
        Thread.Sleep(1000);
    }
}