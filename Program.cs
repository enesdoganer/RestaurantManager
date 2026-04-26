using Spectre.Console;

AnsiConsole.Write(
    new FigletText("Restaurant Manager")
        .Centered()
        .Color(Color.Red));

AnsiConsole.MarkupLine(("[grey]Press any key to continue...[/]"));
Console.ReadKey();        