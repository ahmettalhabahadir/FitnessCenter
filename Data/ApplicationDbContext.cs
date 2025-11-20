using FitnessCenter.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FitnessCenter.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Gym> Gyms { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<TrainerService> TrainerServices { get; set; }
        public DbSet<TrainerAvailability> TrainerAvailabilities { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure relationships
            builder.Entity<Service>()
                .HasOne(s => s.Gym)
                .WithMany(g => g.Services)
                .HasForeignKey(s => s.GymId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Trainer>()
                .HasOne(t => t.Gym)
                .WithMany(g => g.Trainers)
                .HasForeignKey(t => t.GymId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Appointment>()
                .HasOne(a => a.Trainer)
                .WithMany(t => t.Appointments)
                .HasForeignKey(a => a.TrainerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Appointment>()
                .HasOne(a => a.Service)
                .WithMany(s => s.Appointments)
                .HasForeignKey(a => a.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Appointment>()
                .HasOne(a => a.Gym)
                .WithMany(g => g.Appointments)
                .HasForeignKey(a => a.GymId)
                .OnDelete(DeleteBehavior.Restrict);

            // Many-to-many: Trainer-Service via TrainerService join table
            builder.Entity<TrainerService>()
                .HasKey(ts => new { ts.TrainerId, ts.ServiceId });

            builder.Entity<TrainerService>()
                .HasOne(ts => ts.Trainer)
                .WithMany(t => t.TrainerServices)
                .HasForeignKey(ts => ts.TrainerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TrainerService>()
                .HasOne(ts => ts.Service)
                .WithMany(s => s.TrainerServices)
                .HasForeignKey(ts => ts.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            // TrainerAvailability relationship
            builder.Entity<TrainerAvailability>()
                .HasOne(ta => ta.Trainer)
                .WithMany(t => t.TrainerAvailabilities)
                .HasForeignKey(ta => ta.TrainerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Member relationship with ApplicationUser
            builder.Entity<Member>()
                .HasOne(m => m.ApplicationUser)
                .WithMany()
                .HasForeignKey(m => m.IdentityUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure decimal precision for Member
            builder.Entity<Member>()
                .Property(m => m.Height)
                .HasPrecision(5, 2); // cm: 50.00 - 250.00

            builder.Entity<Member>()
                .Property(m => m.Weight)
                .HasPrecision(6, 2); // kg: 30.00 - 300.00

            builder.Entity<Appointment>()
                .HasOne(a => a.Member)
                .WithMany(m => m.Appointments)
                .HasForeignKey(a => a.MemberId)
                .OnDelete(DeleteBehavior.Restrict);

            // ApplicationUser (Admin) - Gym relationship
            builder.Entity<ApplicationUser>()
                .HasOne(u => u.Gym)
                .WithMany()
                .HasForeignKey(u => u.GymId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

