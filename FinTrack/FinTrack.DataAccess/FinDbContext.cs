using FinTrack.DataAccess.Mappings;
using FinTrack.Shared.Entities;
using FinTrack.Shared.Entities.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.DataAccess;


public class FinDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public DbSet<Household> Households { get; set; }
    
    public FinDbContext(DbContextOptions<FinDbContext> options) : base(options)
    {
        
    }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<Household>(e => e.Map());
    }
}
