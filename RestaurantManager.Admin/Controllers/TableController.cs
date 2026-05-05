using RestaurantManager.Core.Models;

namespace RestaurantManager.Admin.Controllers;

public class TableController
{
    private readonly AppController _app;

    public TableController(AppController app)
    {
        _app = app;
    }

    public List<Table> GetAllTables() => _app.Tables;

    public Table? GetTable(int tableId) =>
        _app.Tables.FirstOrDefault(t => t.Id == tableId);

    public bool IsAvailable(int tableId) =>
        _app.Tables.First(t => t.Id == tableId).Available == false;
}