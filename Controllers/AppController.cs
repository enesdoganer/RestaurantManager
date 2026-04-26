using RestaurantManager.Models;

namespace RestaurantManager.Controllers;

public class AppController
{
    public List<Table> Tables { get; } = new List<Table>();
    public List<MenuItem> MenuItems { get; } = new List<MenuItem>();
    public List<Order> Orders { get; } = new List<Order>();

    private int _nextOrderId = 1;

    public AppController()
    {
        SeedData();
    }

    public Order CreateOrder(int tableId)
    {
        var order = new Order(_nextOrderId++, tableId);
        Orders.Add(order);

        var table = Tables.First(t => t.Id == order.TableId);
        table.Available = false;
        table.ActiveOrderId = order.Id;

        return order;
    }

    public void CloseOrder(int orderId)
    {
        var order = Orders.First(o => o.Id == orderId);
        order.Status = OrderStatus.Paid;

        var table = Tables.First(t => t.Id == order.TableId);
        table.Available = true;
        table.ActiveOrderId = null;
    }

    public Order? GetActiveOrder(int tableId)
    {
        var table = Tables.First(t => t.Id == tableId);
        if (table.ActiveOrderId == null) return null;
        return Orders.FirstOrDefault(o => o.Id == table.ActiveOrderId);
    }

    public void SeedData()
    {
        // Tables
        Tables.Add(new Table(1, 2));
        Tables.Add(new Table(2, 2));
        Tables.Add(new Table(3, 2));
        Tables.Add(new Table(4, 2));
        Tables.Add(new Table(5, 4));
        Tables.Add(new Table(6, 4));
        Tables.Add(new Table(7, 4));
        Tables.Add(new Table(8, 4));
        Tables.Add(new Table(9, 6));
        Tables.Add(new Table(10, 6));

        // Starters
        MenuItems.Add(new MenuItem(1, "Tomato Soup",
            "Tomatoes, onion, garlic, cream",
            new List<Allergen> { Allergen.Dairy },
            5.99m, "Starters"));

        MenuItems.Add(new MenuItem(2, "Garlic Bread",
            "Wheat flour, butter, garlic, parsley",
            new List<Allergen> { Allergen.Gluten, Allergen.Dairy },
            3.99m, "Starters"));

        MenuItems.Add(new MenuItem(3, "Caesar Salad",
            "Romaine lettuce, parmesan, croutons, caesar dressing, anchovies",
            new List<Allergen> { Allergen.Gluten, Allergen.Dairy, Allergen.Fish, Allergen.Eggs },
            7.49m, "Starters"));

        MenuItems.Add(new MenuItem(4, "Bruschetta",
            "Sourdough bread, tomatoes, basil, garlic, olive oil",
            new List<Allergen> { Allergen.Gluten },
            4.99m, "Starters"));

        MenuItems.Add(new MenuItem(5, "Chicken Wings",
            "Chicken, flour, paprika, garlic, hot sauce, butter",
            new List<Allergen> { Allergen.Gluten, Allergen.Dairy },
            8.49m, "Starters"));

        MenuItems.Add(new MenuItem(6, "Prawn Cocktail",
            "Prawns, lettuce, marie rose sauce, lemon",
            new List<Allergen> { Allergen.Crustaceans, Allergen.Eggs },
            8.99m, "Starters"));

        // Mains
        MenuItems.Add(new MenuItem(7, "Margherita Pizza",
            "Wheat flour, tomato, mozzarella, basil",
            new List<Allergen> { Allergen.Gluten, Allergen.Dairy },
            11.99m, "Mains"));

        MenuItems.Add(new MenuItem(8, "Beef Burger",
            "Beef, brioche bun, lettuce, tomato, cheddar, mustard mayo",
            new List<Allergen> { Allergen.Gluten, Allergen.Dairy, Allergen.Eggs, Allergen.Mustard },
            13.49m, "Mains"));

        MenuItems.Add(new MenuItem(9, "Grilled Salmon",
            "Salmon fillet, lemon butter, capers, dill",
            new List<Allergen> { Allergen.Fish, Allergen.Dairy },
            15.99m, "Mains"));

        MenuItems.Add(new MenuItem(10, "Pasta Carbonara",
            "Spaghetti, pancetta, egg yolk, parmesan, black pepper",
            new List<Allergen> { Allergen.Gluten, Allergen.Eggs, Allergen.Dairy },
            12.49m, "Mains"));

        MenuItems.Add(new MenuItem(11, "Veggie Stir Fry",
            "Rice noodles, tofu, soy sauce, sesame oil, vegetables",
            new List<Allergen> { Allergen.Soya, Allergen.Sesame },
            10.99m, "Mains"));

        MenuItems.Add(new MenuItem(12, "BBQ Ribs",
            "Pork ribs, BBQ sauce, garlic, paprika, mustard",
            new List<Allergen> { Allergen.Mustard, Allergen.Sulphites },
            18.99m, "Mains"));

        MenuItems.Add(new MenuItem(13, "Fish and Chips",
            "Cod fillet, wheat flour batter, potatoes, mushy peas, tartar sauce",
            new List<Allergen> { Allergen.Gluten, Allergen.Fish, Allergen.Eggs },
            14.49m, "Mains"));

        MenuItems.Add(new MenuItem(14, "Chicken Alfredo",
            "Fettuccine, chicken breast, cream, parmesan, garlic",
            new List<Allergen> { Allergen.Gluten, Allergen.Dairy },
            13.99m, "Mains"));

        // Desserts
        MenuItems.Add(new MenuItem(15, "Cheesecake",
            "Cream cheese, digestive biscuits, butter, sugar, vanilla",
            new List<Allergen> { Allergen.Gluten, Allergen.Dairy, Allergen.Eggs },
            6.49m, "Desserts"));

        MenuItems.Add(new MenuItem(16, "Tiramisu",
            "Mascarpone, espresso, ladyfinger biscuits, eggs, cocoa",
            new List<Allergen> { Allergen.Gluten, Allergen.Dairy, Allergen.Eggs },
            5.99m, "Desserts"));

        MenuItems.Add(new MenuItem(17, "Chocolate Lava Cake",
            "Dark chocolate, butter, eggs, flour, sugar",
            new List<Allergen> { Allergen.Gluten, Allergen.Dairy, Allergen.Eggs },
            6.99m, "Desserts"));

        MenuItems.Add(new MenuItem(18, "Panna Cotta",
            "Cream, sugar, vanilla, gelatin, berry coulis",
            new List<Allergen> { Allergen.Dairy },
            5.49m, "Desserts"));

        MenuItems.Add(new MenuItem(19, "Fruit Sorbet",
            "Seasonal fruit, sugar, lemon juice",
            new List<Allergen>(),
            4.49m, "Desserts"));

        // Drinks
        MenuItems.Add(new MenuItem(20, "Cola",
            "Carbonated water, sugar, caramel colour, phosphoric acid",
            new List<Allergen>(),
            2.49m, "Drinks"));

        MenuItems.Add(new MenuItem(21, "Orange Juice",
            "Freshly squeezed oranges",
            new List<Allergen>(),
            2.99m, "Drinks"));

        MenuItems.Add(new MenuItem(22, "Sparkling Water",
            "Carbonated natural mineral water",
            new List<Allergen>(),
            1.99m, "Drinks"));

        MenuItems.Add(new MenuItem(23, "Lemonade",
            "Water, lemon juice, sugar, mint",
            new List<Allergen>(),
            3.49m, "Drinks"));

        MenuItems.Add(new MenuItem(24, "Mango Smoothie",
            "Mango, banana, orange juice, yoghurt",
            new List<Allergen> { Allergen.Dairy },
            3.99m, "Drinks"));
    }
}