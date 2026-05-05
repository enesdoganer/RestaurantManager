namespace RestaurantManager.Core.Models;

public class MenuItem
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Ingredients { get; set; }
    public List<Allergen> Allergens { get; set; }
    public decimal Price { get; set; }
    public string Category { get; set; }
    
    public MenuItem() { }

    public MenuItem(int id, string name, string ingredients, List<Allergen> allergens, decimal price, string category)
    {
        Id = id;
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