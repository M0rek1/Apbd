using Task09.Controllers;
using Task09.Models;
using Task09.Repositories;

namespace Task09.Services
{
    public class PrescriptionService : IPrescriptionService
    {
        private readonly IPrescriptionRepository _prescriptionRepository;
        private readonly IPatientRepository _patientRepository;

        public PrescriptionService(IPrescriptionRepository prescriptionRepository, IPatientRepository patientRepository)
        {
            _prescriptionRepository = prescriptionRepository;
            _patientRepository = patientRepository;
        }

        public async Task<Prescription> AddPrescriptionAsync(PrescriptionsController.AddPrescriptionRequest request)
        {
            if (request.DueDate < request.Date)
            {
                throw new ArgumentException("DueDate cannot be earlier than Date.");
            }

            var patient = await _patientRepository.GetPatientAsync(request.PatientId);
            if (patient == null)
            {
                patient = new Patient
                {
                    FirstName = request.PatientFirstName,
                    LastName = request.PatientLastName,
                    Birthdate = request.PatientBirthdate
                };
                await _patientRepository.AddPatientAsync(patient);
            }

            var doctor = await _prescriptionRepository.GetDoctorAsync(request.DoctorId);
            if (doctor == null)
            {
                throw new ArgumentException("Doctor not found");
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
                throw new ArgumentException("Prescription can include a maximum of 10 medications.");
            }

            foreach (var medicamentRequest in request.Medicaments)
            {
                var medicament = await _prescriptionRepository.GetMedicamentAsync(medicamentRequest.IdMedicament);
                if (medicament == null)
                {
                    throw new ArgumentException($"Medicament with number {medicamentRequest.IdMedicament} not found");
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

            await _prescriptionRepository.AddPrescriptionAsync(prescription);

            return prescription;
        }
    }
}
