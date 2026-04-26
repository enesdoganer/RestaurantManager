namespace RestaurantManager.Models;

public class Table
{
    public int Id { get; set; }
    public int Seats { get; set; }
    public bool Available { get; set; } = true;
    public int? ActiveOrderId { get; set; } = null;

    public Table(int id, int seats)
    {
        Id = id;
        Seats = seats;
    }
    
}