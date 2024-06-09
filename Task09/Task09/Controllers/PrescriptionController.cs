using Task09.Context;
using Task09.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task09.Services;

namespace Task09.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PrescriptionsController : ControllerBase
    {
        private readonly IPrescriptionService _prescriptionService;

        public PrescriptionsController(IPrescriptionService prescriptionService)
        {
            _prescriptionService = prescriptionService;
        }

        [HttpPost]
        public async Task<IActionResult> AddPrescription(AddPrescriptionRequest request)
        {
            try
            {
                var prescription = await _prescriptionService.AddPrescriptionAsync(request);
                return Ok(prescription);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An unexpected error occurred");
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
}