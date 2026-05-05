using RestaurantManager.Core.Data;
using RestaurantManager.Core.Models;

namespace RestaurantManager.Core.Services;

public class MenuService
{
    private readonly RestaurantDbContext _context;

    public MenuService(RestaurantDbContext context)
    {
        _context = context;
    }

    public List<MenuItem> GetAllItems() =>
        _context.MenuItems.ToList();

    public List<string> GetCategories() =>
        _context.MenuItems.Select(m => m.Category).Distinct().ToList();

    public List<MenuItem> GetByCategory(string category) =>
        _context.MenuItems.Where(m => m.Category == category).ToList();

    public MenuItem? GetById(int id) =>
        _context.MenuItems.FirstOrDefault(m => m.Id == id);
}