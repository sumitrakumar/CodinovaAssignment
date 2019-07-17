using System;
using CodinovaAssignment.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CodinovaAssignment.Entities
{
    public partial class codinovaTestContext : DbContext
    {
        public codinovaTestContext()
        {
        }

        public codinovaTestContext(DbContextOptions<codinovaTestContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Host=localhost;Port=4412;Username=postgres;Password=sumitra;Database=codinovaTest;");  
            }
        }

        public DbSet<UserDetails> Users { get; set; }
        public DbSet<ProductDetails> Products { get; set; }
        public DbSet<SaveOrder> SaveOrders { get; set; }
        public DbSet<EmployeeDetails> Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079");
        }
    }
}
