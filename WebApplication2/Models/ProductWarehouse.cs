namespace APBD6.Models;

using System.ComponentModel.DataAnnotations;

public class ProductWarehouse
{
 
    public int IdProductWarehouse { get; set; }
    public int IdWarehouse { get; set; }

    public int IdProduct { get; set; }

    public int IdOrder { get; set; }

    public int Amount { get; set; }

    [MaxLength(200)]
    public decimal Price { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime CreatedAt { get; set; }
}