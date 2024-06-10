using Microsoft.EntityFrameworkCore;
using Task09.Models;

namespace Task09.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Medicament> Medicaments { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<PrescriptionMedicament> PrescriptionMedicaments { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Data Source=localhost;Initial Catalog=APBD_tut9;User Id=sa;Password=Sy24091976!;Trust Server Certificate=True;Encrypt=True");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PrescriptionMedicament>()
                .HasKey(pm => new { pm.IdPrescription, pm.IdMedicament });

            modelBuilder.Entity<PrescriptionMedicament>()
                .HasOne(pm => pm.Prescription)
                .WithMany(p => p.PrescriptionMedicaments)
                .HasForeignKey(pm => pm.IdPrescription);

            modelBuilder.Entity<PrescriptionMedicament>()
                .HasOne(pm => pm.Medicament)
                .WithMany(m => m.PrescriptionMedicaments)
                .HasForeignKey(pm => pm.IdMedicament);
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id );
                entity.Property(e => e.FirstName);
                entity.Property(e => e.LastName)
                    .IsRequired();
                entity.Property(e => e.Password)
                    .IsRequired();
                entity.Property(e => e.Salt)
                    .IsRequired();
                entity.Property(e => e.RefreshToken);
                entity.Property(e => e.RefreshTokenExp);
            });
        }
    }
}