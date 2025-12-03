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
        DbSet<User> Users { get; set; }
       
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

       
        }
    }
}

