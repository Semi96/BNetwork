using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BnetworkManagement.Models;
using Microsoft.AspNetCore.Identity;

namespace BnetworkManagement.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options)
        {
        }

     
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
            builder.Entity<IdentityUser>().ToTable("BnetUser");
            builder.Entity<ApplicationUser>().ToTable("BnetUser");

            builder.Entity<IdentityRole>().ToTable("BnetRole");
            builder.Entity<IdentityUserRole<string>>().ToTable("BnetUserRole");
            builder.Entity<IdentityUserClaim<string>>().ToTable("BnetUserClaim");
            builder.Entity<IdentityUserLogin<string>>().ToTable("BnetUserLogin");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("BnetRoleClaim");
            builder.Entity<IdentityUserToken<string>>().ToTable("BnetUserToken");
        }

        public DbSet<ApplicationUser> AppUsers { get; set; }
        public DbSet<Capacity> AvailableCapacity { get; set; }
        public DbSet<MiningInventory> MiningInventory { get; set; }
        public DbSet<RentalPurchaseContract> RentalPurchaseContract { get; set; }
        public DbSet<UserTransaction> UserTransactions { get; set; }
        public DbSet<MiningContractProgress> MiningContractProgress { get; set; }
        public DbSet<DiscountCode> DiscountCodes { get; set; }
        public DbSet<Messages> Messages { get; set; }

    }
}
