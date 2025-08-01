using FinTrack.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinTrack.DataAccess.Mappings;

internal static class IncomeMappings
{
    public static void Map(this EntityTypeBuilder<OneTimeIncome> entity)
    {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Description).HasMaxLength(500);
        entity.Property(e => e.Date).HasColumnType("date");
        
        entity.HasOne(e => e.Household).WithMany().HasForeignKey(e => e.HouseholdId);
    }

    public static void Map(this EntityTypeBuilder<RecurringIncome> entity)
    {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.StartDate).HasColumnType("date");
        entity.Property(e => e.EndDate).HasColumnType("date");
        entity.Property(e => e.Description).HasMaxLength(500);
        
        entity.HasOne(e => e.Household).WithMany().HasForeignKey(e => e.HouseholdId);
    }
}