using FMS.Models;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Reflection.Emit;
using System.Transactions;

namespace UITour.Models
{
    public class FMSDbContext : DbContext
    {
        public FMSDbContext(DbContextOptions<FMSDbContext> options)
            : base(options)
        {
        }

        // ================= Reference tables =================
        public DbSet<User> Users { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<FuelRecord> FuelRecords { get; set; }
        public DbSet<Maintenance> Maintenances { get; set; }
        public DbSet<VehicleDriverAssignment> VehicleDriverAssignments { get; set; }
        public DbSet<ExtraExpense> ExtraExpenses { get; set; }
        public DbSet<TripLog> TripLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // ================= DRIVER - VEHICLE ASSIGNMENT =================
            modelBuilder.Entity<VehicleDriverAssignment>()
                .HasOne(vda => vda.Driver)
                .WithMany(d => d.VehicleAssignments)
                .HasForeignKey(vda => vda.DriverID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<VehicleDriverAssignment>()
                .HasOne(vda => vda.Vehicle)
                .WithMany(v => v.VehicleAssignments)
                .HasForeignKey(vda => vda.VehicleID)
                .OnDelete(DeleteBehavior.Restrict);

            // ================= TRIP =================
            modelBuilder.Entity<Trip>()
                .HasOne(t => t.Driver)
                .WithMany()
                .HasForeignKey(t => t.DriverID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Trip>()
                .HasOne(t => t.Vehicle)
                .WithMany()
                .HasForeignKey(t => t.VehicleID)
                .OnDelete(DeleteBehavior.Restrict);

            // ================= TRIP LOG =================
            modelBuilder.Entity<TripLog>()
                .HasOne(tl => tl.Trip)
                .WithMany(t => t.TripLogs)
                .HasForeignKey(tl => tl.TripID);

            // ================= FUEL RECORD =================
            modelBuilder.Entity<FuelRecord>()
                .HasOne(fr => fr.Trip)
                .WithMany(t => t.FuelRecords)
                .HasForeignKey(fr => fr.TripID);


            // ================= EXTRA EXPENSE =================
            modelBuilder.Entity<ExtraExpense>()
                .HasOne(ee => ee.Trip)
                .WithMany(t => t.ExtraExpenses)
                .HasForeignKey(ee => ee.TripID);


            // ================= MAINTENANCE =================
            modelBuilder.Entity<Maintenance>()
                .HasOne(m => m.Vehicle)
                .WithMany(v => v.Maintenances)
                .HasForeignKey(m => m.VehicleID);


            //index
            modelBuilder.Entity<Trip>()
                .HasIndex(t => t.StartTime);

            modelBuilder.Entity<FuelRecord>()
                .HasIndex(fr => fr.FuelTime);

            modelBuilder.Entity<TripLog>()
                .HasIndex(tl => new { tl.TripID, tl.LogTime });

            modelBuilder.Entity<Vehicle>()
                .HasIndex(v => v.LicensePlate)
                .IsUnique();
        }
    }
}

