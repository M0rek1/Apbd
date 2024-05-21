using APBD6.Models;

namespace APBD6.Repositories;

public interface IProductWarehouseRepository
{
    Task<bool> AddProductToWarehouseAsync(ProductWarehouse productWarehouse);
    Task<bool> CheckProductWarehouseExistenceAsync(int idOrder);
}