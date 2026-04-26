using RestaurantManager.Controllers;
using Spectre.Console;

var app = new AppController();

AnsiConsole.Write(
    new FigletText("Restaurant Manager")
        .Centered()
        .Color(Color.Red));

AnsiConsole.MarkupLine(("[grey]Press any key to continue...[/]"));
Console.ReadKey();        