namespace APBD6.Models;

public class ProductWarehouse
{
    public int ProductWarehouseId { get; set; } 
    public int WarehouseId { get; set; }  
    public int ProductId { get; set; }  
    public int OrderId { get; set; }  
    public int Amount { get; set; }
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; }
}