using Microsoft.EntityFrameworkCore;
using NHT_Marine_BE.Models.Product;
using NHT_Marine_BE.Models.Stock;
using NHT_Marine_BE.Models.Transaction;
using NHT_Marine_BE.Models.User;

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

        // Users related tables
        public DbSet<CustomerAddress> CustomerAddresses { get; set; }
        public DbSet<CustomerCart> CustomerCarts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }

        // Products related tables
        public DbSet<Category> Categories { get; set; }
        public DbSet<RootProduct> RootProducts { get; set; }
        public DbSet<ProductItem> ProductItems { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<VariantOption> VariantOptions { get; set; }
        public DbSet<ProductAttribute> ProductAttributes { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<ProductPromotion> ProductsPromotions { get; set; }

        // Orders related tables
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderStatus> OrderStatuses { get; set; }
        public DbSet<StatusTransition> StatusTransitions { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderStatusUpdateLog> OrderStatusUpdateLogs { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<DeliveryService> DeliveryServices { get; set; }
        public DbSet<OrderDelivery> OrderDeliveries { get; set; }

        // Inventory related tables
        public DbSet<Storage> Storages { get; set; }
        public DbSet<StorageType> StorageTypes { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<ProductImport> ProductImports { get; set; }
        public DbSet<ImportItem> ImportItems { get; set; }
        public DbSet<ProductDamageReport> ProductDamageReports { get; set; }
        public DbSet<DamageType> DamageTypes { get; set; }
        public DbSet<DamageReportItem> DamageReportItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Handle enum conversions
            builder.Entity<CustomerCart>().Property(cc => cc.Status).HasConversion<string>();

            // Handle foreign keys which are not auto-setup
            builder.Entity<Category>().HasOne(c => c.CreatedByStaff).WithMany().HasForeignKey(c => c.CreatedBy);
            builder.Entity<RootProduct>().HasOne(rp => rp.CreatedByStaff).WithMany().HasForeignKey(rp => rp.CreatedBy);
            builder.Entity<Promotion>().HasOne(p => p.CreatedByStaff).WithMany().HasForeignKey(p => p.CreatedBy);
            builder.Entity<Coupon>().HasOne(c => c.CreatedByStaff).WithMany().HasForeignKey(c => c.CreatedBy);
            builder.Entity<ProductImport>().HasOne(pi => pi.TrackedByStaff).WithMany().HasForeignKey(pi => pi.TrackedBy);
            builder.Entity<ProductDamageReport>().HasOne(pdr => pdr.ReportedByStaff).WithMany().HasForeignKey(pdr => pdr.ReportedBy);
            builder.Entity<OrderStatusUpdateLog>().HasOne(ul => ul.UpdatedByStaff).WithMany().HasForeignKey(ul => ul.UpdatedBy);

            // Handle table self-references
            builder.Entity<Staff>().HasOne(s => s.CreatedByStaff).WithMany(s => s.CreatedStaffs).HasForeignKey(s => s.CreatedBy);
            builder.Entity<Category>().HasOne(c => c.ParentCategory).WithMany(c => c.ChildrenCategories).HasForeignKey(c => c.ParentId);

            // Handle many-to-many relations
            builder.Entity<RolePermission>().HasKey(rp => new { rp.RoleId, rp.PermissionId });
            builder.Entity<RolePermission>().HasOne(rp => rp.Role).WithMany(sr => sr.Permissions).HasForeignKey(rp => rp.RoleId);
            builder.Entity<RolePermission>().HasOne(rp => rp.Permission).WithMany(ap => ap.Roles).HasForeignKey(rp => rp.PermissionId);

            builder.Entity<CartItem>().HasKey(ci => new { ci.CartId, ci.ProductItemId });
            builder.Entity<CartItem>().HasOne(ci => ci.Cart).WithMany(cc => cc.Items).HasForeignKey(ci => ci.CartId);
            builder.Entity<CartItem>().HasOne(ci => ci.ProductItem).WithMany(pi => pi.CartItems).HasForeignKey(ci => ci.ProductItemId);

            builder.Entity<ProductAttribute>().HasKey(pa => new { pa.ProductItemId, pa.OptionId });
            builder
                .Entity<ProductAttribute>()
                .HasOne(pa => pa.ProductItem)
                .WithMany(pi => pi.Attributes)
                .HasForeignKey(pa => pa.ProductItemId);
            builder.Entity<ProductAttribute>().HasOne(pa => pa.Option).WithMany(vo => vo.ProductItems).HasForeignKey(pa => pa.OptionId);

            builder.Entity<ProductPromotion>().HasKey(pp => new { pp.PromotionId, pp.ProductId });
            builder.Entity<ProductPromotion>().HasOne(pp => pp.Promotion).WithMany(p => p.Products).HasForeignKey(pp => pp.PromotionId);
            builder.Entity<ProductPromotion>().HasOne(pp => pp.Product).WithMany(p => p.Promotions).HasForeignKey(pp => pp.ProductId);

            builder.Entity<OrderItem>().HasKey(oi => new { oi.OrderId, oi.ProductItemId });
            builder.Entity<OrderItem>().HasOne(oi => oi.Order).WithMany(o => o.Items).HasForeignKey(oi => oi.OrderId);
            builder.Entity<OrderItem>().HasOne(oi => oi.ProductItem).WithMany(pi => pi.Orders).HasForeignKey(oi => oi.ProductItemId);

            builder.Entity<Inventory>().HasKey(i => new { i.StorageId, i.ProductItemId });
            builder.Entity<Inventory>().HasOne(i => i.Storage).WithMany(s => s.ProductItems).HasForeignKey(i => i.StorageId);
            builder.Entity<Inventory>().HasOne(i => i.ProductItem).WithMany(pi => pi.Storages).HasForeignKey(i => i.ProductItemId);

            builder.Entity<ImportItem>().HasKey(ii => new { ii.ImportId, ii.ProductItemId });
            builder.Entity<ImportItem>().HasOne(ii => ii.Import).WithMany(i => i.Items).HasForeignKey(ii => ii.ImportId);
            builder.Entity<ImportItem>().HasOne(ii => ii.ProductItem).WithMany(pi => pi.Imports).HasForeignKey(ii => ii.ProductItemId);

            builder.Entity<DamageReportItem>().HasKey(dri => new { dri.ReportId, dri.ProductItemId });
            builder.Entity<DamageReportItem>().HasOne(dri => dri.Report).WithMany(pdr => pdr.Items).HasForeignKey(dri => dri.ReportId);
            builder
                .Entity<DamageReportItem>()
                .HasOne(dri => dri.ProductItem)
                .WithMany(pi => pi.DamageReports)
                .HasForeignKey(dri => dri.ProductItemId);
        }
    }
}
