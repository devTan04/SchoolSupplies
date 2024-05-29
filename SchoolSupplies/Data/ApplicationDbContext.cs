using Microsoft.EntityFrameworkCore;
using SchoolSupplies.Models.Entities;

namespace SchoolSupplies.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Brand> Brands  { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Thiết lập khóa chính cho OrderDetail và Feedback
            modelBuilder.Entity<OrderDetail>().HasKey(od => new { od.OrderId, od.ProductId });
            modelBuilder.Entity<Feedback>().HasKey(f => new { f.UserId, f.ProductId });

            // Thiết lập mối quan hệ giữa Product và Feedback
            modelBuilder.Entity<Product>()
                .HasMany(p => p.Feedbacks)
                .WithOne(f => f.Product)
                .HasForeignKey(f => f.ProductId);

            // Thiết lập mối quan hệ giữa User và Feedback
            modelBuilder.Entity<User>()
                .HasMany(u => u.Feedbacks)
                .WithOne(f => f.User)
                .HasForeignKey(f => f.UserId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
