using System;
using ContractorBackend.Domain.Entities.Ghg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContractorBackend.Persistence.Configurations
{
    /// <summary>
    /// پیکربندی موجودیت‌های ماژول ردپای کربن
    /// </summary>
    public static class GhgEntityConfigurations
    {
        public static void ConfigureGhgEntities(this ModelBuilder builder)
        {
            builder.Entity<GhgArea>(entity =>
            {
                entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
                entity.Property(x => x.FaName).HasMaxLength(200).IsRequired();
                entity.Property(x => x.CommitteeCode).HasMaxLength(50);
                entity.HasIndex(x => x.Code).IsUnique();
            });

            builder.Entity<CostCenter>(entity =>
            {
                entity.Property(x => x.Name).HasMaxLength(400).IsRequired();
                entity.Property(x => x.UnitProcess).HasMaxLength(400);
                entity.HasIndex(x => x.Code).IsUnique();
                entity.HasOne(x => x.Area)
                    .WithMany(x => x.CostCenters)
                    .HasForeignKey(x => x.AreaId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<GhgGas>(entity =>
            {
                entity.Property(x => x.Name).HasMaxLength(100).IsRequired();
                entity.Property(x => x.FaName).HasMaxLength(200);
                entity.Property(x => x.ChemicalFormula).HasMaxLength(50);
            });

            builder.Entity<GlobalWarmingPotential>(entity =>
            {
                entity.Property(x => x.GasKey).HasMaxLength(100).IsRequired();
                entity.Property(x => x.AssessmentReport).HasMaxLength(20);
                entity.HasOne(x => x.Gas)
                    .WithMany(x => x.GlobalWarmingPotentials)
                    .HasForeignKey(x => x.GasId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(x => new { x.GasKey, x.AssessmentReport });
            });

            builder.Entity<EmissionCategory>(entity =>
            {
                entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
                entity.Property(x => x.FaName).HasMaxLength(200).IsRequired();
                entity.Property(x => x.Sign).HasMaxLength(5).IsRequired();
                entity.Property(x => x.Description).HasMaxLength(2000);
                entity.HasIndex(x => x.Sign).IsUnique();
            });

            builder.Entity<GhgParameter>(entity =>
            {
                entity.Property(x => x.Key).HasMaxLength(100).IsRequired();
                entity.Property(x => x.Name).HasMaxLength(400).IsRequired();
                entity.Property(x => x.FaName).HasMaxLength(400);
                entity.Property(x => x.Unit).HasMaxLength(100);
                entity.Property(x => x.Source).HasMaxLength(1000);
                entity.HasIndex(x => new { x.Key, x.Year });
            });

            builder.Entity<Fuel>(entity =>
            {
                entity.Property(x => x.Name).HasMaxLength(100).IsRequired();
                entity.Property(x => x.FaName).HasMaxLength(200);
                entity.Property(x => x.LhvUnit).HasMaxLength(100).IsRequired();
                entity.Property(x => x.ConsumptionUnit).HasMaxLength(100).IsRequired();
                entity.Property(x => x.Source).HasMaxLength(1000);
            });

            builder.Entity<EmissionFactor>(entity =>
            {
                entity.Property(x => x.RefKey).HasMaxLength(200).IsRequired();
                entity.Property(x => x.RefLabel).HasMaxLength(400);
                entity.Property(x => x.SubKey).HasMaxLength(100);
                entity.Property(x => x.Gas).HasMaxLength(50).IsRequired();
                entity.Property(x => x.Unit).HasMaxLength(200).IsRequired();
                entity.Property(x => x.Source).HasMaxLength(2000);
                entity.HasIndex(x => new { x.Category, x.RefKey, x.Gas, x.ValidFromYear });
            });

            builder.Entity<CalculationFormula>(entity =>
            {
                entity.Property(x => x.Code).HasMaxLength(100).IsRequired();
                entity.Property(x => x.Name).HasMaxLength(400).IsRequired();
                entity.Property(x => x.FaName).HasMaxLength(400).IsRequired();
                entity.Property(x => x.Expression).HasMaxLength(2000).IsRequired();
                entity.Property(x => x.VariablesJson).HasMaxLength(8000);
                entity.Property(x => x.OutputUnit).HasMaxLength(100).IsRequired();
                entity.Property(x => x.Reference).HasMaxLength(1000);
                entity.Property(x => x.Notes).HasMaxLength(2000);
                entity.HasIndex(x => x.Code).IsUnique();
            });

            builder.Entity<GhgPeriod>(entity =>
            {
                entity.Property(x => x.Title).HasMaxLength(400).IsRequired();
                entity.Property(x => x.Description).HasMaxLength(2000);
                entity.HasIndex(x => x.PersianYear).IsUnique();
            });

            builder.Entity<ActivityDataEntry>(entity =>
            {
                entity.Property(x => x.FactorRefKey).HasMaxLength(200);
                entity.Property(x => x.FactorSubKey).HasMaxLength(100);
                entity.Property(x => x.EmissionSource).HasMaxLength(500);
                entity.Property(x => x.Unit).HasMaxLength(100);
                entity.Property(x => x.Description).HasMaxLength(2000);
                entity.HasIndex(x => new { x.PeriodId, x.CategoryId });
                entity.HasOne(x => x.Period)
                    .WithMany(x => x.ActivityDataEntries)
                    .HasForeignKey(x => x.PeriodId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.Category)
                    .WithMany()
                    .HasForeignKey(x => x.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.Area)
                    .WithMany()
                    .HasForeignKey(x => x.AreaId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.CostCenter)
                    .WithMany()
                    .HasForeignKey(x => x.CostCenterId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.Fuel)
                    .WithMany()
                    .HasForeignKey(x => x.FuelId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<EmissionResult>(entity =>
            {
                entity.Property(x => x.EmissionCode).HasMaxLength(50).IsRequired();
                entity.HasIndex(x => new { x.PeriodId, x.CategoryId });
                entity.HasOne(x => x.Period)
                    .WithMany(x => x.EmissionResults)
                    .HasForeignKey(x => x.PeriodId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.Category)
                    .WithMany()
                    .HasForeignKey(x => x.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.Area)
                    .WithMany()
                    .HasForeignKey(x => x.AreaId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<ProductFootprint>(entity =>
            {
                entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
                entity.Property(x => x.FaName).HasMaxLength(200);
                entity.Property(x => x.AreaName).HasMaxLength(200);
                entity.Property(x => x.Boundary).HasMaxLength(200);
                entity.Property(x => x.Standard).HasMaxLength(200);
                entity.Property(x => x.Description).HasMaxLength(2000);
                entity.HasOne(x => x.Period)
                    .WithMany()
                    .HasForeignKey(x => x.PeriodId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
