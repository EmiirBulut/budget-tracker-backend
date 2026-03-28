using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BudgetTracker.Domain.Entities;

namespace BudgetTracker.Infrastructure.Persistence.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.Type)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(a => a.Currency)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(a => a.Balance)
            .IsRequired()
            .HasPrecision(18, 6);

        builder.Property(a => a.IsArchived)
            .IsRequired();

        builder.Property(a => a.CreatedAt)
            .IsRequired();

        // Index for fast retrieval of all accounts belonging to a user.
        builder.HasIndex(a => a.UserId);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
