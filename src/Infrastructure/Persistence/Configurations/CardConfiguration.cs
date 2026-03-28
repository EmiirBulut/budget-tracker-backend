using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Enums;

namespace BudgetTracker.Infrastructure.Persistence.Configurations;

public class CardConfiguration : IEntityTypeConfiguration<Card>
{
    public void Configure(EntityTypeBuilder<Card> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.CardCategory)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(c => c.CardType)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(c => c.Last4Digits)
            .IsRequired()
            .HasMaxLength(4);

        builder.Property(c => c.ExpiryDate)
            .IsRequired()
            .HasMaxLength(7);

        builder.Property(c => c.Currency)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(c => c.Color)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(c => c.CreditLimit)
            .HasColumnType("numeric(18,2)");

        builder.Property(c => c.IsArchived)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Account>()
            .WithMany()
            .HasForeignKey(c => c.LinkedAccountId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .IsRequired(false);
    }
}
