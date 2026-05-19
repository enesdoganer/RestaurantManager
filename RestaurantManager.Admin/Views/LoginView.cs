using Spectre.Console;

namespace RestaurantManager.Admin.Views;

public static class LoginView
{
    public static string PromptToken()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(
            new FigletText("Restaurant Manager")
                .Centered()
                .Color(Color.MediumPurple));

        AnsiConsole.MarkupLine("[darkorange]Admin Access Required![/]\n");

        return AnsiConsole.Prompt(
            new TextPrompt<string>("Enter admin token:")
                .Secret());
    }

    public static void ShowAccessDenied()
    {
        AnsiConsole.MarkupLine("[red]Access denied. Invalid token.[/]");
        Thread.Sleep(1500);
    }

    public static void ShowAccessGranted()
    {
        AnsiConsole.MarkupLine("[green]Access granted. Welcome![/]");
        Thread.Sleep(1000);
    }
}