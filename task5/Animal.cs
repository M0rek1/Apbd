namespace task5.Properties;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Animal
{
    [Key] 
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
    public int IdAnimal { get; set; }
        
    
    [StringLength(200)]
    public string Name { get; set; }

    [StringLength(200)]
    public string Description { get; set; }

  
    [StringLength(200)]
    public string Category { get; set; }


    [StringLength(200)]
    public string Area { get; set; }
}