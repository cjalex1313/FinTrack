using FinTrack.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinTrack.DataAccess.Mappings;

internal static class ExpenseMappings
{
    public static void Map(this EntityTypeBuilder<ExpenseBucket> entity)
    {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Name).HasMaxLength(100);
        entity.Property(e => e.Description).HasMaxLength(500);
        
        entity.HasOne(e => e.Household).WithMany().HasForeignKey(e => e.HouseholdId);
    }

    public static void Map(this EntityTypeBuilder<Expense> entity)
    {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Description).HasMaxLength(500);
        
        entity.HasOne(e => e.Household).WithMany().HasForeignKey(e => e.HouseholdId);
        entity.HasOne(e => e.ExpenseBucket).WithMany().HasForeignKey(e => e.ExpenseBucketId).OnDelete(DeleteBehavior.SetNull);

        entity.HasIndex(e => new {e.HouseholdId, e.Date});
    }
}