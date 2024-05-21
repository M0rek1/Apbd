namespace APBD6.Models;

using System.ComponentModel.DataAnnotations;

public class Order
{
    public int IdOrder { get; set; }

    public int IdProduct { get; set; }

    public int Amount { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime CreatedAt { get; set; }
    
    [DataType(DataType.DateTime)]
    public DateTime? FulfilledAt { get; set; }
}