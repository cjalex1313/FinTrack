using FinTrack.Shared.Entities;
using FinTrack.Shared.Entities.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinTrack.DataAccess.Mappings;

public static class HouseholdMappings
{
    public static void Map(this EntityTypeBuilder<Household> entity)
    {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
        entity.HasOne<ApplicationUser>().WithMany().HasForeignKey(e => e.OwnerId);
    }
}