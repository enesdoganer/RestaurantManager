using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestaurantManager.Admin.Handlers;
using RestaurantManager.Admin.Views;
using RestaurantManager.Core.Data;
using RestaurantManager.Core.Services;
using Spectre.Console;
using DbContext = RestaurantManager.Core.Data.DbContext;

namespace RestaurantManager.Admin;

public class App
{
    private readonly IServiceProvider _serviceProvider;

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
        
        // Initialize services:
        _serviceProvider = new ServiceCollection()
            .AddDbContext<DbContext>(options =>
                    options.UseNpgsql(connectionString),
                ServiceLifetime.Scoped)
            .AddScoped<TableService>()
            .AddScoped<MenuService>()
            .AddScoped<OrderService>()
            .BuildServiceProvider();
        
        // Database setup:
        using var setupScope = _serviceProvider.CreateScope();
        var db = setupScope.ServiceProvider.GetRequiredService<DbContext>();
        db.Database.EnsureCreated();
        DbSeeder.Seed(db);
    }

    public void Run()
    {
        if (!Login()) return;
        
        ShowSplashScreen();
        
        var locator = new ServiceLocator(_serviceProvider);

        while (true)
        {
            var tableHandler = new TableHandler(locator);
            var menuHandler = new MenuHandler(locator);
            var orderHandler = new OrderHandler(locator);
            var tableManagementHandler = new TableManagementHandler(locator);
            
            DashboardView.Render(locator.GetTableService().GetAllTables());

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
                    AnsiConsole.MarkupLine("[grey]Goodbye![/]");
                    Thread.Sleep(1000);
                    return;
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

    private static void ShowSplashScreen()
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