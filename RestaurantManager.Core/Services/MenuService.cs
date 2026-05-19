using RestaurantManager.Core.Data;
using RestaurantManager.Core.Models;

namespace RestaurantManager.Core.Services;

public class MenuService
{
    private readonly DbContext _context;

    public MenuService(DbContext context)
    {
        _context = context;
    }

    public List<MenuItem> GetAllItems() =>
        _context.MenuItems.AsSorted().ToList();

    public List<string> GetCategories() =>
        _context.MenuItems.Select(m => m.Category).Distinct().ToList();

    public List<MenuItem> GetByCategory(string category) =>
        _context.MenuItems.Where(m => m.Category == category).ToList();

    public MenuItem? GetById(int id) =>
        _context.MenuItems.FirstOrDefault(m => m.Id == id);

    public void AddItem(MenuItem item)
    {
        _context.MenuItems.Add(item);
        _context.SaveChanges();
        RenumberItems();
    }

    public void UpdateItem(MenuItem item)
    {
        _context.MenuItems.Update(item);
        _context.SaveChanges();
    }

    public void RemoveItem(MenuItem item)
    {
        _context.MenuItems.Remove(item);
        _context.SaveChanges();
        RenumberItems();
    }
    
    public void RenumberItems()
    {
        var categoryOrder = new List<string> { "Starters", "Mains", "Desserts", "Drinks" };
        
        var items = _context.MenuItems
            .ToList()
            .OrderBy(i => categoryOrder.IndexOf(i.Category))
            .ThenBy(i => i.Id)
            .ToList();

        int number = 1;
        foreach (var item in items)
        {
            item.ItemNumber = number++;
        }
        
        _context.SaveChanges();
    }
}