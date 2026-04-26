namespace RestaurantManager.Models;

public class OrderItem
{
    public MenuItem MenuItem { get; set; }
    public int Quantity { get; set; }
    public decimal Subtotal => MenuItem.Price * Quantity;
    
    public OrderItem(int quantity, MenuItem menuItem)
    { 
        Quantity = quantity;
        MenuItem = menuItem;
    }
    
}