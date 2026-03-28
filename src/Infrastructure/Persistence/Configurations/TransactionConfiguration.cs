using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Enums;

namespace BudgetTracker.Infrastructure.Persistence.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Type)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(t => t.Amount)
            .IsRequired()
            .HasColumnType("numeric(18,2)");

        builder.Property(t => t.Category)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.Description)
            .HasMaxLength(500);

        builder.Property(t => t.Date)
            .IsRequired();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Account>()
            .WithMany()
            .HasForeignKey(t => t.AccountId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .IsRequired(false);

        builder.HasOne<Card>()
            .WithMany()
            .HasForeignKey(t => t.CardId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .IsRequired(false);

        builder.HasOne<InstallmentPlan>()
            .WithMany()
            .HasForeignKey(t => t.InstallmentPlanId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .IsRequired(false);
    }
}
