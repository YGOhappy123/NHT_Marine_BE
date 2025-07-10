using Microsoft.EntityFrameworkCore;
using NHT_Marine_BE.Models.Product;
using NHT_Marine_BE.Models.User;

// using NHT_Marine_BE.Models;

namespace NHT_Marine_BE.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions dbContextOptions)
            : base(dbContextOptions) { }

        // Authentication related tables
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<StaffRole> StaffRoles { get; set; }
        public DbSet<AppPermission> AppPermissions { get; set; }
        public DbSet<RolePermission> RolesPermissions { get; set; }

        // User related tables
        public DbSet<CustomerAddress> CustomerAddresses { get; set; }
        public DbSet<CustomerCart> CustomerCarts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }

        // Product related tables
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<RootProduct> RootProducts { get; set; }
        public DbSet<ProductItem> ProductItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Handle enum conversions
            builder.Entity<CustomerCart>().Property(cc => cc.Status).HasConversion<string>();

            // Handle table self-references
            builder.Entity<Staff>().HasOne(s => s.CreatedByStaff).WithMany(s => s.CreatedStaffs).HasForeignKey(s => s.CreatedBy);

            // Handle many-to-many relations
            builder.Entity<RolePermission>().HasKey(rp => new { rp.RoleId, rp.PermissionId });
            builder.Entity<RolePermission>().HasOne(rp => rp.Role).WithMany(sr => sr.Permissions).HasForeignKey(rp => rp.RoleId);
            builder.Entity<RolePermission>().HasOne(rp => rp.Permission).WithMany(ap => ap.Roles).HasForeignKey(rp => rp.PermissionId);

            builder.Entity<CartItem>().HasKey(ci => new { ci.CartId, ci.ProductItemId });
            builder.Entity<CartItem>().HasOne(ci => ci.Cart).WithMany(cc => cc.Items).HasForeignKey(ci => ci.CartId);
            builder.Entity<CartItem>().HasOne(ci => ci.ProductItem).WithMany(pi => pi.CartItems).HasForeignKey(ci => ci.ProductItemId);
        }
    }
}
