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
    public DbSet<OneTimeIncome> OneTimeIncomes { get; set; }
    public DbSet<RecurringIncome> RecurringIncomes { get; set; }
    public DbSet<ExpenseBucket> ExpenseBuckets { get; set; }
    public DbSet<Expense> Expenses { get; set; }
    public DbSet<RecurringExpense> RecurringExpenses { get; set; }
    public DbSet<HouseholdMember> HouseholdMembers { get; set; }
    
    public FinDbContext(DbContextOptions<FinDbContext> options) : base(options)
    {
        
    }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<Household>(e => e.Map());
        builder.Entity<OneTimeIncome>(e => e.Map());
        builder.Entity<RecurringIncome>(e => e.Map());
        builder.Entity<ExpenseBucket>(e => e.Map());
        builder.Entity<Expense>(e => e.Map());
        builder.Entity<ApplicationUser>(e => e.Map());
        builder.Entity<RecurringExpense>(e => e.Map());
        builder.Entity<HouseholdMember>(e => e.Map());
    }
}
