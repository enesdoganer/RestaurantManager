using Microsoft.Extensions.DependencyInjection;

namespace RestaurantManager.Core.Services;

public class ServiceLocator
{
    private readonly IServiceProvider _serviceProvider;

    public ServiceLocator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public TableService GetTableService() =>
        _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<TableService>();

    public MenuService GetMenuService() =>
        _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<MenuService>();

    public OrderService GetOrderService()
    {
        var scope = _serviceProvider.CreateScope();
        return scope.ServiceProvider.GetRequiredService<OrderService>();
    }
}