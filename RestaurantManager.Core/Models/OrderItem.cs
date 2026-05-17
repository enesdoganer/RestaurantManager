namespace RestaurantManager.Core.Models;

public class OrderItem
{
    public int Id { get; init; }
    public int OrderId { get; set; }
    public int MenuItemId { get; set; }
    public MenuItem MenuItem { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal Subtotal => MenuItem.Price * Quantity;
    
    public OrderItem() { }
    
    public OrderItem(int quantity, MenuItem menuItem)
    { 
        Quantity = quantity;
        MenuItem = menuItem;
        MenuItemId = menuItem.Id;
    }
    
}