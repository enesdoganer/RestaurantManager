using Microsoft.Extensions.Configuration;
using RestaurantManager.Admin.Handlers;
using RestaurantManager.Admin.Views;
using RestaurantManager.Core.Data;
using RestaurantManager.Core.Services;
using Spectre.Console;

namespace RestaurantManager.Admin;

public class App
{
    private readonly TableService _tableService;
    private readonly MenuService _menuService;
    private readonly OrderService _orderService;

    public App()
    {
        // Get connection string from appsettings.json
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();
        var connectionString = config.GetConnectionString("DefaultConnection")
                               ?? throw new Exception("Connection string not found.");
        
        // Initialize tokenHash
        var tokenHash = config["Auth:TokenHash"]
                        ?? throw new Exception("Token hash not found.");
        AuthService.Initialize(tokenHash);

        // Create db and create the tables if they do not exist:
        var db = DbContextFactory.Create(connectionString);
        db.Database.EnsureCreated();

        // Seed initial data
        DbSeeder.Seed(db);

        // Services
        _tableService = new TableService(db);
        _menuService = new MenuService(db);
        _orderService = new OrderService(db, _tableService);
    }

    public void Run()
    {
        if (!Login()) return;
        
        ShowSplashScreen();
        
        var tableHandler = new TableHandler(_tableService, _menuService, _orderService);
        var menuHandler = new MenuHandler(_menuService);
        var orderHandler = new OrderHandler(_orderService);
        var tableManagementHandler = new TableManagementHandler(_tableService);

        bool running = true;

        while (running)
        {
            DashboardView.Render(_tableService.GetAllTables());

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("What would you like to do?")
                    .AddChoices(
                        "New Order",
                        "Manage Existing Order",
                        "View Menu",
                        "View Active Orders",
                        "View Past Orders",
                        "Manage Tables",
                        "Exit")
                    .WrapAround(true));

            switch (choice)
            {
                case "New Order":
                    tableHandler.HandleNewOrder();
                    break;
                case "Manage Existing Order":
                    tableHandler.HandleManageOrder();
                    break;
                case "View Menu":
                    menuHandler.HandleViewMenu();
                    break;
                case "View Active Orders":
                    orderHandler.HandleViewActiveOrders();
                    break;
                case "View Past Orders":
                    orderHandler.HandleViewPastOrders();
                    break;
                case "Manage Tables":
                    tableManagementHandler.Handle();
                    break;
                case "Exit":
                    running = false;
                    AnsiConsole.MarkupLine("[grey]Goodbye![/]");
                    break;
            }
        }
        
    }

    private static bool Login()
    {
        const int maxAttempts = 3;

        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            var token = LoginView.PromptToken();
            if (AuthService.ValidateToken(token))
            {
                LoginView.ShowAccessGranted();
                Thread.Sleep(800);
                return true;
            }
            
            LoginView.ShowAccessDenied();

            if (attempt < maxAttempts)
            {
                AnsiConsole.MarkupLine($"[darkorange]{maxAttempts - attempt} attempt(s) remaining.[/]");
                Thread.Sleep(800);
            }
        }
        
        AnsiConsole.MarkupLine("[red]Too many failed attempts. Exiting...[/]");
        Thread.Sleep(2000);
        return false;
    }

    public void ShowSplashScreen()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(
            new FigletText("Restaurant Manager")
                .Centered()
                .Color(Color.MediumPurple));

        AnsiConsole.MarkupLine("[white]Press any key to continue...[/]");
        Console.ReadKey();
    }
    
}