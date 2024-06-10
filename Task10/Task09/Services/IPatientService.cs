using Task09.Controllers;
using Task09.Models;

namespace Task09.Services
{
    public interface IPatientService
    {
        Task<GetPatientResponse> GetPatientAsync(int patientId);
    }
}