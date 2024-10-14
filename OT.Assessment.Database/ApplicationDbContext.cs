using Microsoft.EntityFrameworkCore;
using OT.Assessment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OT.Assessment.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() { }
        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!options.IsConfigured)
            {
                options.UseSqlServer("Server=3D9A-DB91\\MSSQLSERVER01;Database=OT_Assessment_DB;Integrated Security=SSPI;TrustServerCertificate=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define indexes
            modelBuilder.Entity<CasinoWager>()
                .HasIndex(cw => cw.AccountId)
                .HasDatabaseName("IX_CasinoWager_WagerId");

            modelBuilder.Entity<CasinoWager>()
               .HasIndex(cw => cw.Provider)
               .HasDatabaseName("IX_CasinoWager_AccountId");


            modelBuilder.Entity<CasinoWager>()
                .HasIndex(cw => cw.CreatedDateTime)
                .HasDatabaseName("IX_CasinoWager_CreatedDateTime");
   
            modelBuilder.Entity<CasinoWager>()
             .HasIndex(cw => cw.Provider)
             .HasDatabaseName("IX_CasinoWager_TransactionId");
        }

        public DbSet<CasinoWager> CasinoWager { get; set; }
    }
}
