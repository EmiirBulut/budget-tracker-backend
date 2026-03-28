using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BudgetTracker.Domain.Entities;

namespace BudgetTracker.Infrastructure.Persistence.Configurations;

public class InstallmentPlanConfiguration : IEntityTypeConfiguration<InstallmentPlan>
{
    public void Configure(EntityTypeBuilder<InstallmentPlan> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(p => p.Category)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.TotalAmount)
            .IsRequired()
            .HasColumnType("numeric(18,2)");

        builder.Property(p => p.MonthlyPayment)
            .IsRequired()
            .HasColumnType("numeric(18,2)");

        builder.Property(p => p.NumberOfMonths)
            .IsRequired();

        builder.Property(p => p.StartDate)
            .IsRequired();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Card>()
            .WithMany()
            .HasForeignKey(p => p.CardId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.Payments)
            .WithOne()
            .HasForeignKey(ip => ip.InstallmentPlanId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
