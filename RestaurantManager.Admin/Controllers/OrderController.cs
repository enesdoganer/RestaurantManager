using RestaurantManager.Core.Models;

namespace RestaurantManager.Admin.Controllers;

public class OrderController
{
    private readonly AppController _app;

    public OrderController(AppController app)
    {
        _app = app;
    }

    public Order StartOrder(int tableId) => _app.CreateOrder(tableId);

    public void AddItem(Order order, MenuItem menuItem, int quantity)
    {
        var exists = order.Items.FirstOrDefault(i => i.MenuItem.Id == menuItem.Id);
        if (exists != null)
            exists.Quantity += quantity;
        else
            order.Items.Add(new OrderItem(quantity, menuItem));
    }

    public void RemoveItem(Order order, int menuItemId)
    {
        var item = order.Items.FirstOrDefault(i => i.MenuItem.Id == menuItemId);
        if (item != null) order.Items.Remove(item);
    }

    public void UpdateStatus(Order order, OrderStatus status) =>
        order.Status = status;

    public void CloseOrder(int orderId) => _app.CloseOrder(orderId);

    public Order? GetActiveOrder(int tableId) => _app.GetActiveOrder(tableId);

    public List<Order> GetActiveOrders() =>
        _app.Orders.Where(o => o.Status != OrderStatus.Paid).ToList();
}