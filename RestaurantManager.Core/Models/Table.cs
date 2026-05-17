namespace RestaurantManager.Core.Models;

public class Table
{
    public int Id { get; init; }
    public int Seats { get; set; }
    public bool Available { get; set; } = true;
    public int? ActiveOrderId { get; set; } = null;
    
    public Table() { }
    
    public Table(int id, int seats)
    {
        Id = id;
        Seats = seats;
    }
    
}