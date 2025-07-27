using FinTrack.Shared.Entities.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.DataAccess;


public class FinDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public FinDbContext(DbContextOptions<FinDbContext> options) : base(options)
    {
        
    }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}
