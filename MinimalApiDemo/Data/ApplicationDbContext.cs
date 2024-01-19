using Microsoft.EntityFrameworkCore;
using MinimalApiDemo.Models;

namespace MinimalApiDemo.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Coupon> Coupons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Coupon>().HasData(
                new Coupon
                {
                    Id = 1,
                    Name = "10 OFF",
                    Percent = 10,
                    IsActive = true
                },
                new Coupon
                {
                    Id = 2,
                    Name = "20 OFF",
                    Percent = 20,
                    IsActive = true
                }
            );
        }
    }
}
