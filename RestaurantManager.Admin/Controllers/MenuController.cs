using RestaurantManager.Core.Models;

namespace RestaurantManager.Admin.Controllers;

public class MenuController
{
    private readonly AppController _app;

    public MenuController(AppController app)
    {
        _app = app;
    }

    public List<MenuItem> GetAllItems() => _app.MenuItems;

    public List<string> GetCategories() =>
        _app.MenuItems.Select(i => i.Category).Distinct().ToList();

    public List<MenuItem> GetByCategory(string category) =>
        _app.MenuItems.Where(i => i.Category == category).ToList();

    public MenuItem? GetItem(int id) =>
        _app.MenuItems.FirstOrDefault(i => i.Id == id);
}