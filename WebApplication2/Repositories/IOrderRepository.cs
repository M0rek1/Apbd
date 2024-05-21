
using APBD6.Models;

namespace APBD6.Repositories;

public interface IOrderRepository
{
    Task<Order> GetOrderByIdProductAndAmountAsync(int idProduct, int amount);
    Task<bool> UpdateOrderFulfillmentDateAsync(int idOrder, DateTime fulfilledAt);
}