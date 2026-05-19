namespace RestaurantManager.Core.Models;

public class Table
{
    public int Id { get; init; }
    public int TableNumber { get; set; }
    public int Seats { get; set; }
    public bool Available { get; set; } = true;
    public int? ActiveOrderId { get; set; } = null;
    
    public Table() { }
    
    public Table(int seats)
    {
        Seats = seats;
    }
    
}