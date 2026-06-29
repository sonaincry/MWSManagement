using Indotalent.DTOs;
using Indotalent.Infrastructures.Docs;
using Indotalent.Infrastructures.Images;
using Indotalent.Models.Configurations;
using Indotalent.Models.Entities;
using Indotalent.Models.Entities.AX;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MWSManagement.DTOs;
using MWSManagement.Models.DTOs;
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
        public DbSet<TransDetailDto> TransDetailDto { get; set; } = default!;
        public DbSet<CustomerDto> CustomerDto { get; set; } = default!;
        public DbSet<ProductDetailDto> ProductDetailDto { get; set; } = default!;
        public DbSet<UnitOfMeasureDto> UnitOfMeasureDto { get; set; } = default!;
        public DbSet<LookupItem> LookupItem { get; set; } = default!;
        public DbSet<ProductDto> ProductDTO { get; set; } = default!;
        public DbSet<CategoryDTO> CategoryDTO { get; set; } = default!;
        public DbSet<TaxVatNumTableAX> TaxVatNumTables { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Crucial: This configures the underlying Identity tables (AspNetUsers, AspNetUserRoles, etc.)
            base.OnModelCreating(modelBuilder);

            // Configure core file upload tracking keys
            modelBuilder.Entity<FileImage>().HasKey(f => f.Id);
            modelBuilder.Entity<FileImage>().Property(f => f.OriginalFileName).HasMaxLength(100);
            modelBuilder.Entity<FileDocument>().HasKey(f => f.Id);
            modelBuilder.Entity<FileDocument>().Property(f => f.OriginalFileName).HasMaxLength(100);

            modelBuilder.Entity<RetailTransaction>(entity =>
            {
                entity.HasNoKey();
                entity.ToTable("RETAILTRANSACTIONTABLE", "dbo");
                entity.Property(x => x.PaymentAmount).HasPrecision(18, 2);
                entity.Property(x => x.NetAmount).HasPrecision(18, 2);
                entity.Property(x => x.NumberOfItems).HasPrecision(18, 0);
                entity.Property(x => x.GrossAmount).HasPrecision(18, 2);
            });

            modelBuilder.Entity<TransDetailDto>(entity =>
            {
                entity.HasNoKey();
                entity.ToView("RETAILTRANSACTIONSVIEW", "crt");
                entity.Property(x => x.QTY).HasPrecision(18, 4);
                entity.Property(x => x.NETAMOUNT).HasPrecision(18, 2);
                entity.Property(x => x.TAXAMOUNT).HasPrecision(18, 2);
                entity.Property(x => x.PAYMENTAMOUNT).HasPrecision(18, 2);
            });

            modelBuilder.Entity<ProductDto>(entity => { entity.HasNoKey(); });
            modelBuilder.Entity<CategoryDTO>(entity => { entity.HasNoKey(); });
            modelBuilder.Entity<UnitOfMeasureDto>(entity => { entity.HasNoKey(); });

            modelBuilder.Entity<ProductDetailDto>(entity =>
            {
                entity.HasNoKey();
                entity.Property(x => x.SalesPrice).HasPrecision(18, 2);
            });

            modelBuilder.Entity<ApplicationUser>()
            .Property(x => x.IsNotDeleted)
            .HasDefaultValue(false);

            modelBuilder.Entity<TaxVatNumTableAX>(entity =>
            {
                entity.ToTable("TAXVATNUMTABLE", "ax");

                entity.HasKey(x => x.RecId);

                entity.Property(x => x.RecId)
                    .HasColumnName("RECID")
                    .ValueGeneratedNever();

                entity.Property(x => x.CountryRegionId)
                    .HasColumnName("COUNTRYREGIONID")
                    .HasMaxLength(10)
                    .IsRequired();

                entity.Property(x => x.Name)
                    .HasColumnName("NAME")
                    .HasMaxLength(60)
                    .IsRequired();

                entity.Property(x => x.VatNum)
                    .HasColumnName("VATNUM")
                    .HasMaxLength(20)
                    .IsRequired();

                entity.Property(x => x.DataAreaId)
                    .HasColumnName("DATAAREAID")
                    .HasMaxLength(4)
                    .IsRequired();

                entity.Property(x => x.RowVersion)
                    .HasColumnName("ROWVERSION")
                    .IsRowVersion()
                    .ValueGeneratedOnAddOrUpdate();
            });

            modelBuilder.Entity<LookupItem>().HasNoKey();
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