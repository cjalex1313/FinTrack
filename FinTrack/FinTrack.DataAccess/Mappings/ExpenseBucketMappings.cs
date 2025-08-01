using FinTrack.Shared.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinTrack.DataAccess.Mappings;

internal static class ExpenseBucketMappings
{
    public static void Map(this EntityTypeBuilder<ExpenseBucket> entity)
    {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Name).HasMaxLength(100);
        entity.Property(e => e.Description).HasMaxLength(500);
        
        entity.HasOne(e => e.Household).WithMany().HasForeignKey(e => e.HouseholdId);
    }
}