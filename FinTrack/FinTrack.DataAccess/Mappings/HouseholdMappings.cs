using FinTrack.Shared.Entities;
using FinTrack.Shared.Entities.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinTrack.DataAccess.Mappings;

internal static class HouseholdMappings
{
    public static void Map(this EntityTypeBuilder<Household> entity)
    {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
        entity.HasOne<ApplicationUser>().WithMany().HasForeignKey(e => e.OwnerId);
    }

    public static void Map(this EntityTypeBuilder<HouseholdMember> entity)
    {
        entity.HasKey(e => new {e.HouseholdId, e.UserId});
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("timezone('utc', now())").ValueGeneratedOnAdd();
        entity.HasOne(e => e.Household).WithMany().HasForeignKey(e => e.HouseholdId).OnDelete(DeleteBehavior.Cascade);
        entity.HasOne<ApplicationUser>().WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}