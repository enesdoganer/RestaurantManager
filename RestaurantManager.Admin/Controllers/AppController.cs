using RestaurantManager.Core.Models;

namespace RestaurantManager.Admin.Controllers;

public class AppController
{
    public List<Table> Tables { get; } = new List<Table>();
    public List<MenuItem> MenuItems { get; } = new List<MenuItem>();
    public List<Order> Orders { get; } = new List<Order>();

    private int _nextOrderId = 1;

    public AppController()
    {
        //SeedData();
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
    
}