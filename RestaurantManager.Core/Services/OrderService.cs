using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using RestaurantManager.Core.Data;
using RestaurantManager.Core.Models;
using DbContext = RestaurantManager.Core.Data.DbContext;

namespace RestaurantManager.Core.Services;

public class OrderService
{
    private readonly DbContext _context;
    private readonly TableService _tableService;

    public OrderService(DbContext context, TableService tableService)
    {
        _context = context;
        _tableService = tableService;
    }

    public Order CreateOrder(int tableId)
    {
        var order = new Order { TableId = tableId, Status = OrderStatus.Pending, CreatedAt = DateTime.Now };
        _context.Orders.Add(order);
        _context.SaveChanges();

        _tableService.SetOccupied(tableId, order.Id);
        return order;
    }

    public void AddItem(Order order, MenuItem menuItem, int quantity)
    {
        var existing = order.Items.FirstOrDefault(i => i.MenuItemId == menuItem.Id);
        if (existing != null)
        {
            existing.Quantity += quantity;
        }
        else
        {
            order.Items.Add(new OrderItem(quantity, menuItem) { OrderId = order.Id });
        }
        _context.SaveChanges();
    }

    public void RemoveItem(Order order, int menuItemId)
    {
        var item = order.Items.FirstOrDefault(i => i.MenuItemId == menuItemId);
        if (item != null)
        {
            order.Items.Remove(item);
            _context.OrderItems.Remove(item);
            _context.SaveChanges();
        }
    }

    public void UpdateStatus(Order order, OrderStatus status)
    {
        order.Status = status;
        _context.SaveChanges();
    }

    public void CloseOrder(Order order)
    {
        order.Status = OrderStatus.Paid;
        _context.SaveChanges();
        _tableService.SetFree(order.TableId);
    }

    public Order? GetActiveOrder(int tableId)
    {
        var table = _tableService.GetById(tableId);
        if (table?.ActiveOrderId == null) return null;

        return _context.Orders
            .Include(o => o.Table)
            .Include(o => o.Items)
            .ThenInclude(i => i.MenuItem)
            .FirstOrDefault(o => o.Id == table.ActiveOrderId);
    }

    private IIncludableQueryable<Order, MenuItem> GetOrders() =>
        _context.Orders
            .Include(o => o.Table)
            .Include(o => o.Items)
            .ThenInclude(i => i.MenuItem);

    public List<Order> GetActiveOrders() =>
        GetOrders()
            .Where(o => o.Status != OrderStatus.Paid)
            .AsSorted()
            .ToList();
    
    public List<Order> GetPastOrders() =>
        GetOrders()
            .Where(o => o.Status == OrderStatus.Paid)
            .ToList();
}