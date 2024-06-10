using Task09.Context;
using Task09.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task09.Services;
using Microsoft.AspNetCore.Authorization;
namespace Task09.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientsController(IPatientService patientService)
        {
            _patientService = patientService;
        }
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPatient(int id)
        {
            var response = await _patientService.GetPatientAsync(id);

            if (response == null)
            {
                return NotFound();
            }

            return Ok(response);
        }
    }

    public class GetPatientResponse
    {
        public int IdPatient { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Birthdate { get; set; }
        public List<PrescriptionResponse> Prescriptions { get; set; }
    }

    public class PrescriptionResponse
    {
        public int IdPrescription { get; set; }
        public DateTime Date { get; set; }
        public DateTime DueDate { get; set; }
        public DoctorResponse Doctor { get; set; }
        public List<MedicamentResponse> Medicaments { get; set; }
    }

    public class DoctorResponse
    {
        public int IdDoctor { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class MedicamentResponse
    {
        public int IdMedicament { get; set; }
        public string Name { get; set; }
        public int Dose { get; set; }
        public string Description { get; set; }
    }
}
