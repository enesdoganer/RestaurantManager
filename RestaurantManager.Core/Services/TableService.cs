using Microsoft.EntityFrameworkCore;
using RestaurantManager.Core.Data;
using RestaurantManager.Core.Models;

namespace RestaurantManager.Core.Services;

public class TableService
{
    private readonly RestaurantDbContext _context;

    public TableService(RestaurantDbContext context)
    {
        _context = context;
    }

    public List<Table> GetAllTables() =>
        _context.Tables.ToList();

    public Table? GetById(int id) =>
        _context.Tables.FirstOrDefault(t => t.Id == id);

    public void SetOccupied(int tableId, int orderId)
    {
        var table = _context.Tables.First(t => t.Id == tableId);
        table.Available = false;
        table.ActiveOrderId = orderId;
        _context.SaveChanges();
    }

    public void SetFree(int tableId)
    {
        var table = _context.Tables.First(t => t.Id == tableId);
        table.Available = true;
        table.ActiveOrderId = null;
        _context.SaveChanges();
    }
}