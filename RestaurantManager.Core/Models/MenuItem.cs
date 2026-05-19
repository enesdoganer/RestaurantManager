namespace RestaurantManager.Core.Models;

public class MenuItem
{
    public int Id { get; init; }
    public int ItemNumber { get; set; }
    public string Name { get; set; }
    public List<String>? Ingredients { get; set; }
    public List<Allergen>? Allergens { get; set; }
    public decimal Price { get; set; }
    public string Category { get; set; }
    
    public MenuItem() { }

    public MenuItem(string name, List<String> ingredients, List<Allergen> allergens, decimal price, string category)
    {
        Name = name;
        Ingredients = ingredients;
        Allergens = allergens;
        Price = price;
        Category = category;
    }
    
}

public enum Allergen
{
    Gluten,
    Peanuts,
    TreeNuts,
    Celery,
    Mustard,
    Eggs,
    Dairy,
    Sesame,
    Fish,
    Crustaceans,
    Molluscs,
    Soya,
    Sulphites,
    Lupin
}