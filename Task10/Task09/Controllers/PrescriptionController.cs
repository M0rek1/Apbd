using Task09.Context;
using Task09.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Task09.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrescriptionsController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public PrescriptionsController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddPrescription(AddPrescriptionRequest request)
        {
            if (request.DueDate < request.Date)
            {
                return BadRequest("DueDate cannot be earlier than Date.");
            }

            var patient = await context.Patients.FindAsync(request.PatientId);
            if (patient == null)
            {
                patient = new Patient
                {
                    FirstName = request.PatientFirstName,
                    LastName = request.PatientLastName,
                    Birthdate = request.PatientBirthdate
                };
                context.Patients.Add(patient);
                await context.SaveChangesAsync();
            }

            var doctor = await context.Doctors.FindAsync(request.DoctorId);
            if (doctor == null)
            {
                return NotFound("Doctor not found");
            }

            var prescription = new Prescription
            {
                Date = request.Date,
                DueDate = request.DueDate,
                IdPatient = patient.IdPatient,
                IdDoctor = doctor.IdDoctor
            };

            if (request.Medicaments.Count > 10)
            {
                return BadRequest("Prescription can include a maximum of 10 medications.");
            }

            foreach (var medicamentRequest in request.Medicaments)
            {
                var medicament = await context.Medicaments.FindAsync(medicamentRequest.IdMedicament);
                if (medicament == null)
                {
                    return NotFound($"Medicament with number {medicamentRequest.IdMedicament} not found");
                }

                var prescriptionMedicament = new PrescriptionMedicament
                {
                    IdPrescription = prescription.IdPrescription,
                    IdMedicament = medicament.IdMedicament,
                    Dose = medicamentRequest.Dose,
                    Details = medicamentRequest.Description
                };
                prescription.PrescriptionMedicaments.Add(prescriptionMedicament);
            }

            context.Prescriptions.Add(prescription);
            await context.SaveChangesAsync();

            return Ok(prescription);
        }
    }

    public class AddPrescriptionRequest
    {
        public int PatientId { get; set; }
        public string PatientFirstName { get; set; }
        public string PatientLastName { get; set; }
        public DateTime PatientBirthdate { get; set; }
        public int DoctorId { get; set; }
        public DateTime Date { get; set; }
        public DateTime DueDate { get; set; }
        public List<MedicamentRequest> Medicaments { get; set; }
    }

    public class MedicamentRequest
    {
        public int IdMedicament { get; set; }
        public int Dose { get; set; }
        public string Description { get; set; }
    }
}
