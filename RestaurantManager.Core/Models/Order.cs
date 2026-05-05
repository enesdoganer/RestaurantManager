using System.ComponentModel.DataAnnotations.Schema;
using RestaurantManager.Core.Models;

namespace RestaurantManager.Core.Models;

public class Order
{
    public int Id { get; set; }
    public int TableId { get; set; }
    public List<OrderItem> Items { get; set; } = new List<OrderItem>();
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    
    [NotMapped]
    public decimal TotalPrice => Items.Sum(i => i.Subtotal);
    
    public Order() { }
    
    public Order(int id, int tableId)
    {
        Id = id;
        TableId = tableId;
    }
    
}