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
        var trackedOrder = _context.Orders
            .Include(o => o.Items)
            .First(o => o.Id == order.Id);
        
        var existing = trackedOrder.Items.FirstOrDefault(i => i.MenuItemId == menuItem.Id);
        if (existing != null)
        {
            existing.Quantity += quantity;
        }
        else
        {
            trackedOrder.Items.Add(new OrderItem(quantity, menuItem) { OrderId = trackedOrder.Id });
        }

        _context.SaveChanges();
    }

    public void RemoveItem(Order order, int menuItemId)
    {
        var trackedOrder = _context.Orders
            .Include(o => o.Items)
            .First(o => o.Id == order.Id);
        
        var item = trackedOrder.Items.FirstOrDefault(i => i.MenuItemId == menuItemId);
        if (item != null)
        {
            trackedOrder.Items.Remove(item);
            _context.OrderItems.Remove(item);
            _context.SaveChanges();
        }
    }

    public void UpdateStatus(Order order, OrderStatus status)
    {
        var trackedOrder = _context.Orders.First(o => o.Id == order.Id);
        trackedOrder.Status = status;
        _context.SaveChanges();
    }

    public void CloseOrder(Order order)
    {
        var trackedOrder = _context.Orders.First(o => o.Id == order.Id);
        trackedOrder.Status = OrderStatus.Paid;
        _context.SaveChanges();
        _tableService.SetFree(order.TableId);
    }

    public Order? GetActiveOrder(int tableId)
    {
        var table = _context.Tables
            .AsNoTracking()
            .FirstOrDefault(t => t.Id == tableId);
        if (table?.ActiveOrderId == null) return null;

        return _context.Orders
            .AsNoTracking()
            .Include(o => o.Table)
            .Include(o => o.Items)
            .ThenInclude(i => i.MenuItem)
            .FirstOrDefault(o => o.Id == table.ActiveOrderId);
    }

    public Order? GetOrderById(int id) =>
        _context.Orders
            .AsNoTracking()
            .Include(o => o.Table)
            .Include(o => o.Items)
            .ThenInclude(i => i.MenuItem)
            .FirstOrDefault(o => o.Id == id);

    private IIncludableQueryable<Order, MenuItem> GetOrders() =>
        _context.Orders
            .AsNoTracking()
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
            .AsSorted()
            .ToList();
}