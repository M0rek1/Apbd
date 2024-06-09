using Task09.Controllers;
using Task09.Models;

namespace Task09.Services
{
    public interface IPrescriptionService
    {
        Task<Prescription> AddPrescriptionAsync(PrescriptionsController.AddPrescriptionRequest request);
    }
}