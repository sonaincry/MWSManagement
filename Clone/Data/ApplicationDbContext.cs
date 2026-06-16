using Indotalent.Infrastructures.Docs;
using Indotalent.Infrastructures.Images;
using Indotalent.Models.Configurations;
using Indotalent.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using YamlDotNet.Core.Tokens;

namespace Indotalent.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // --- CORE CORE SYSTEM / USER RELATED TABLES ONLY ---
        public DbSet<FileImage> FileImages { get; set; } = default!;
        public DbSet<FileDocument> FileDocument { get; set; } = default!;
        public DbSet<AspNetCompany> AspNetCompany { get; set; } = default!;
        public DbSet<LogSession> LogSession { get; set; } = default!;
        public DbSet<LogError> LogError { get; set; } = default!;
        public DbSet<LogAnalytic> LogAnalytic { get; set; } = default!;

        public DbSet<RetailTransaction> RetailTransaction { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Crucial: This configures the underlying Identity tables (AspNetUsers, AspNetUserRoles, etc.)
            base.OnModelCreating(modelBuilder);

            // Configure core file upload tracking keys
            modelBuilder.Entity<FileImage>().HasKey(f => f.Id);
            modelBuilder.Entity<FileImage>().Property(f => f.OriginalFileName).HasMaxLength(100);
            modelBuilder.Entity<FileDocument>().HasKey(f => f.Id);
            modelBuilder.Entity<FileDocument>().Property(f => f.OriginalFileName).HasMaxLength(100);

            // Configure RetailTransaction
            modelBuilder.Entity<RetailTransaction>().HasKey(x => x.TransactionId);
            modelBuilder.Entity<RetailTransaction>().ToTable("RETAILTRANSACTIONTABLE");
            modelBuilder.ApplyConfiguration(new ApplicationUserConfiguration());
            modelBuilder.ApplyConfiguration(new AspNetCompanyConfiguration());
            modelBuilder.ApplyConfiguration(new LogAnalyticConfiguration());
            modelBuilder.ApplyConfiguration(new LogErrorConfiguration());
            modelBuilder.ApplyConfiguration(new LogSessionConfiguration());
            modelBuilder.Ignore<CustomerContact>();
            modelBuilder.Ignore<AdjustmentMinus>();
            modelBuilder.Ignore<AdjustmentPlus>();
        }
    }
}