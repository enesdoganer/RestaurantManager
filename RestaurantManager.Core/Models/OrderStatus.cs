namespace RestaurantManager.Core.Models;

/*
 * These values are stored as integers in the db:
 * 
 * Pending      -> 0
 * Preparing    -> 1
 * Served       -> 2
 * Paid         -> 3
 * 
 */

public enum OrderStatus
{
    Pending,
    Preparing,
    Served,
    Paid
}