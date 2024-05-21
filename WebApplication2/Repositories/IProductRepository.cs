using APBD6.Models;

namespace APBD6.Repositories;

public interface IProductRepository
{
    Task<Product> GetProductByIdAsync(int idProduct);
}