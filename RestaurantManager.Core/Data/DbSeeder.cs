using RestaurantManager.Core.Models;

namespace RestaurantManager.Core.Data;

public static class DbSeeder
{
    public static void Seed(DbContext context)
    {
        if (context.Tables.Any() || context.MenuItems.Any())
            return; // Already seeded

        var tables = new List<Table>
        {
            new Table(1, 2),
            new Table(2, 2),
            new Table(3, 2),
            new Table(4, 2),
            new Table(5, 4),
            new Table(6, 4),
            new Table(7, 4),
            new Table(8, 4),
            new Table(9, 6),
            new Table(10, 6)
        };

        var menuItems = new List<MenuItem>
        {
            // Starters
            new MenuItem(1, "Tomato Soup",
                ["tomatoes", "onion", "garlic", "cream"],
                new List<Allergen> { Allergen.Dairy }, 5.99m, "Starters"),
            new MenuItem(2, "Garlic Bread",
                ["wheat flour", "butter", "garlic", "parsley"],
                new List<Allergen> { Allergen.Gluten, Allergen.Dairy }, 3.99m, "Starters"),
            new MenuItem(3, "Caesar Salad",
                ["romaine lettuce", "parmesan", "croutons", "ceaser dressing", "anchovies"],
                new List<Allergen> { Allergen.Gluten, Allergen.Dairy, Allergen.Fish, Allergen.Eggs }, 7.49m,
                "Starters"),
            new MenuItem(4, "Bruschetta",
                ["sourdough bread", "tomatoes", "basil", "garlic", "olive oil"],
                new List<Allergen> { Allergen.Gluten }, 4.99m, "Starters"),
            new MenuItem(5, "Chicken Wings",
                ["chicken", "flour", "paprika", "garlic", "hot sauce", "butter"],
                new List<Allergen> { Allergen.Gluten, Allergen.Dairy }, 8.49m, "Starters"),
            new MenuItem(6, "Prawn Cocktail",
                ["prawns", "lettuce", "marie rose sauce", "lemon"],
                new List<Allergen> { Allergen.Crustaceans, Allergen.Eggs }, 8.99m, "Starters"),
            new MenuItem(7, "Mushroom Soup",
                ["mixed mushrooms", "cream", "thyme", "onion", "garlic"],
                new List<Allergen> { Allergen.Dairy }, 5.49m, "Starters"),
            new MenuItem(8, "Mozzarella Sticks",
                ["mozzarella", "breadcrumbs", "eggs", "flour", "marinara sauce"],
                new List<Allergen> { Allergen.Gluten, Allergen.Dairy, Allergen.Eggs }, 6.49m, "Starters"),

            // Mains
            new MenuItem(9, "Margherita Pizza",
                ["wheat flour", "tomato", "mozzarella", "basil"],
                new List<Allergen> { Allergen.Gluten, Allergen.Dairy }, 11.99m, "Mains"),
            new MenuItem(10, "Beef Burger",
                ["beef", "brioche bun", "lettuce", "tomato", "cheddar", "mustard mayo"],
                new List<Allergen> { Allergen.Gluten, Allergen.Dairy, Allergen.Eggs, Allergen.Mustard }, 13.49m,
                "Mains"),
            new MenuItem(11, "Grilled Salmon",
                ["salmon fillet", "lemon butter", "capers", "dill"],
                new List<Allergen> { Allergen.Fish, Allergen.Dairy }, 15.99m, "Mains"),
            new MenuItem(12, "Pasta Carbonara",
                ["spaghetti", "pancetta", "egg yolk", "parmesan", "black pepper"],
                new List<Allergen> { Allergen.Gluten, Allergen.Eggs, Allergen.Dairy }, 12.49m, "Mains"),
            new MenuItem(13, "Veggie Stir Fry",
                ["rice noodles", "tofu", "soy sauce", "sesame oil", "vegetables"],
                new List<Allergen> { Allergen.Soya, Allergen.Sesame }, 10.99m, "Mains"),
            new MenuItem(14, "BBQ Ribs",
                ["pork ribs", "bbq sauce", "garlic", "paprika", "mustard"],
                new List<Allergen> { Allergen.Mustard, Allergen.Sulphites }, 18.99m, "Mains"),
            new MenuItem(15, "Fish and Chips",
                ["cod fillet", "wheat flour batter", "potatoes", "mushy peas", "tartar sauce"],
                new List<Allergen> { Allergen.Gluten, Allergen.Fish, Allergen.Eggs }, 14.49m, "Mains"),
            new MenuItem(16, "Chicken Alfredo",
                ["fettuccine", "chicken breast", "cream", "parmesan", "garlic"],
                new List<Allergen> { Allergen.Gluten, Allergen.Dairy }, 13.99m, "Mains"),
            new MenuItem(17, "Lamb Chops",
                ["lamb", "rosemary", "garlic", "olive oil", "mint sauce"],
                new List<Allergen> { Allergen.Sulphites }, 19.99m, "Mains"),
            new MenuItem(18, "Mushroom Risotto",
                ["arborio rice", "mixed mushrooms", "parmesan", "white wine", "butter"],
                new List<Allergen> { Allergen.Dairy, Allergen.Sulphites }, 12.99m, "Mains"),
            new MenuItem(19, "Chicken Tikka Masala",
                ["chicken", "tomato", "cream", "tikka spices", "basmati rice"],
                new List<Allergen> { Allergen.Dairy }, 14.99m, "Mains"),

            // Desserts
            new MenuItem(20, "Cheesecake",
                ["cream cheese", "digestive biscuits", "butter", "sugar", "vanilla"],
                new List<Allergen> { Allergen.Gluten, Allergen.Dairy, Allergen.Eggs }, 6.49m, "Desserts"),
            new MenuItem(21, "Tiramisu",
                ["mascarpone", "espresso", "ladyfinger biscuits", "eggs", "cocoa"],
                new List<Allergen> { Allergen.Gluten, Allergen.Dairy, Allergen.Eggs }, 5.99m, "Desserts"),
            new MenuItem(22, "Chocolate Lava Cake",
                ["dark chocolate", "butter", "eggs", "flour", "sugar"],
                new List<Allergen> { Allergen.Gluten, Allergen.Dairy, Allergen.Eggs }, 6.99m, "Desserts"),
            new MenuItem(23, "Panna Cotta",
                ["cream", "sugar", "vanilla", "gelatin", "berry coulis"],
                new List<Allergen> { Allergen.Dairy }, 5.49m, "Desserts"),
            new MenuItem(24, "Fruit Sorbet",
                ["seasonal fruit", "sugar", "lemon juice"],
                new List<Allergen>(), 4.49m, "Desserts"),
            new MenuItem(25, "Sticky Toffee Pudding",
                ["dates", "flour", "butter", "brown sugar", "toffee sauce", "cream"],
                new List<Allergen> { Allergen.Gluten, Allergen.Dairy, Allergen.Eggs }, 6.99m, "Desserts"),
            new MenuItem(26, "Creme Brulee",
                ["cream", "egg yolks", "sugar", "vanilla"],
                new List<Allergen> { Allergen.Dairy, Allergen.Eggs }, 5.99m, "Desserts"),

            // Drinks
            new MenuItem(27, "Cola",
                ["carbonated water", "sugar", "caramel colour", "phosphoric acid"],
                new List<Allergen>(), 2.49m, "Drinks"),
            new MenuItem(28, "Orange Juice",
                ["freshly squeezed oranges"],
                new List<Allergen>(), 2.99m, "Drinks"),
            new MenuItem(29, "Sparkling Water",
                ["carbonated natural mineral water"],
                new List<Allergen>(), 1.99m, "Drinks"),
            new MenuItem(30, "Lemonade",
                ["water", "lemon juice", "sugar", "mint"],
                new List<Allergen>(), 3.49m, "Drinks"),
            new MenuItem(31, "Mango Smoothie",
                ["mango", "banana", "orange juice", "yoghurt"],
                new List<Allergen> { Allergen.Dairy }, 3.99m, "Drinks"),
            new MenuItem(32, "Iced Tea",
                ["black tea", "lemon", "sugar", "ice"],
                new List<Allergen>(), 2.99m, "Drinks"),
            new MenuItem(33, "Hot Chocolate",
                ["whole milk", "dark chocolate", "whipped cream"],
                new List<Allergen> { Allergen.Dairy }, 3.49m, "Drinks"),
            new MenuItem(34, "Espresso",
                ["arabica coffee beans", "water"],
                new List<Allergen>(), 2.49m, "Drinks"),
            new MenuItem(35, "Cappuccino",
                ["espresso", "steamed milk", "milk foam"],
                new List<Allergen> { Allergen.Dairy }, 3.29m, "Drinks")
        };

        context.Tables.AddRange(tables);
        context.MenuItems.AddRange(menuItems);
        context.SaveChanges();
    }
}