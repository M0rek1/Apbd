using Task09.Context;
using Task09.Models;
using Microsoft.EntityFrameworkCore;

namespace Task09.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly ApplicationDbContext _context;

        public PatientRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Patient> GetPatientAsync(int patientId)
        {
            return await _context.Patients
                .Include(p => p.Prescriptions)
                .ThenInclude(pr => pr.PrescriptionMedicaments)
                .ThenInclude(pm => pm.Medicament)
                .Include(p => p.Prescriptions)
                .ThenInclude(pr => pr.Doctor)
                .FirstOrDefaultAsync(p => p.IdPatient == patientId);
        }

        public async Task AddPatientAsync(Patient patient)
        {
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
        }
    }
}