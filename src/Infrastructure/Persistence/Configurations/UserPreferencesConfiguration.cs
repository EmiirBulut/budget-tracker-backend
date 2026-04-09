using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BudgetTracker.Domain.Entities;

namespace BudgetTracker.Infrastructure.Persistence.Configurations;

public class UserPreferencesConfiguration : IEntityTypeConfiguration<UserPreferences>
{
    public void Configure(EntityTypeBuilder<UserPreferences> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.DefaultCurrency)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(p => p.Language)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(p => p.NotificationsEnabled)
            .IsRequired();

        builder.HasIndex(p => p.UserId)
            .IsUnique();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
