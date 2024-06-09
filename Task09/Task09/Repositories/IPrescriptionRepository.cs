using Task09.Models;

namespace Task09.Repositories
{
    public interface IPrescriptionRepository
    {
        Task AddPrescriptionAsync(Prescription prescription);
        Task<Doctor> GetDoctorAsync(int doctorId);
        Task<Medicament> GetMedicamentAsync(int medicamentId);
    }
}