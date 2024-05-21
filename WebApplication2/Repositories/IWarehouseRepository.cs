using APBD6.Models;

namespace APBD6.Repositories;

public interface IWarehouseRepository
{
    Task<Warehouse> GetWarehouseByIdAsync(int idWarehouse);
}