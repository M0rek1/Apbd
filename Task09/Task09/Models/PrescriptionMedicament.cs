using System.ComponentModel.DataAnnotations;

namespace Task09.Models;

public class PrescriptionMedicament
{
    
    [Key]
    public int IdMedicament { get; set; }
    
    [Key]
    public int IdPrescription { get; set; }
    public Prescription Prescription { get; set; }
    public Medicament Medicament { get; set; }
    public int Dose { get; set; }
    [MaxLength(100)]
    public string Details { get; set; }
}