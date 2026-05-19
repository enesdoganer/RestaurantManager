namespace RestaurantManager.Core.Models;

/*
 * These values are stored as integers in the db:
 * 
 * Pending      -> 0
 * Preparing    -> 1
 * Served       -> 2
 * BillRequested-> 3
 * Paid         -> 4
 * 
 */

public enum OrderStatus
{
    Pending,
    Preparing,
    Served,
    BillRequested,
    Paid
}