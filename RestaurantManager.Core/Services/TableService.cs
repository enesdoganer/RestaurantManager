using Microsoft.EntityFrameworkCore;
using RestaurantManager.Core.Data;
using RestaurantManager.Core.Models;
using DbContext = RestaurantManager.Core.Data.DbContext;

namespace RestaurantManager.Core.Services;

public class TableService
{
    private readonly DbContext _context;

    public TableService(DbContext context)
    {
        _context = context;
    }

    public List<Table> GetAllTables()
    {
        return _context.Tables
            .AsNoTracking()
            .AsSorted()
            .ToList();
    }

    public Table? GetById(int id) =>
        _context.Tables
            .AsNoTracking()
            .FirstOrDefault(t => t.Id == id);

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

    public void AddTable(Table table)
    {
        _context.Tables.Add(table);
        _context.SaveChanges();
        RenumberTables();
    }

    public void UpdateTable(Table table)
    {
        _context.Tables.Update(table);
        _context.SaveChanges();
    }

    public void RemoveTable(Table table)
    {
        _context.Tables.Remove(table);
        _context.SaveChanges();
        RenumberTables();
    }

    public void RenumberTables()
    {
        var tables = _context.Tables
            .OrderBy(t => t.Id)
            .ToList();

        int number = 1;
        foreach (var table in tables)
        {
            table.TableNumber = number++;
        }

        _context.SaveChanges();
    }
}