using Task09.Models;

namespace Task09.Repositories
{
    public interface IPatientRepository
    {
        Task<Patient> GetPatientAsync(int patientId);
        Task AddPatientAsync(Patient patient);
    }
}