using FinTrack.Shared.Entities.Auth;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinTrack.DataAccess.Mappings;

internal static class UserMappings
{
    public static void Map(this EntityTypeBuilder<ApplicationUser> entity)
    {
        entity.Property(e => e.FirstName).HasMaxLength(50);
        entity.Property(e => e.LastName).HasMaxLength(50);
    }
}