using Task09.Context;
using Task09.Models;
using Microsoft.EntityFrameworkCore;

namespace Task09.Repositories
{
    public class PrescriptionRepository : IPrescriptionRepository
    {
        private readonly ApplicationDbContext _context;

        public PrescriptionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddPrescriptionAsync(Prescription prescription)
        {
            _context.Prescriptions.Add(prescription);
            await _context.SaveChangesAsync();
        }

        public async Task<Doctor> GetDoctorAsync(int doctorId)
        {
            return await _context.Doctors.FindAsync(doctorId);
        }

        public async Task<Medicament> GetMedicamentAsync(int medicamentId)
        {
            return await _context.Medicaments.FindAsync(medicamentId);
        }
    }
}