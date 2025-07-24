using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace G3_PerfumeShop.Models
{
    public partial class G3_PerfumeShopDB_Iter3Context : DbContext
    {
        public G3_PerfumeShopDB_Iter3Context()
        {
        }

        public G3_PerfumeShopDB_Iter3Context(DbContextOptions<G3_PerfumeShopDB_Iter3Context> options)
            : base(options)
        {
        }

        public virtual DbSet<Blog> Blogs { get; set; } = null!;
        public virtual DbSet<BlogCategory> BlogCategories { get; set; } = null!;
        public virtual DbSet<BlogStatus> BlogStatuses { get; set; } = null!;
        public virtual DbSet<Brand> Brands { get; set; } = null!;
        public virtual DbSet<CustomDashboard> CustomDashboards { get; set; } = null!;
        public virtual DbSet<DashBoardUserOption> DashBoardUserOptions { get; set; } = null!;
        public virtual DbSet<DashboardSection> DashboardSections { get; set; } = null!;
        public virtual DbSet<DefaultDashboard> DefaultDashboards { get; set; } = null!;
        public virtual DbSet<Feedback> Feedbacks { get; set; } = null!;
        public virtual DbSet<FeedbackResponse> FeedbackResponses { get; set; } = null!;
        public virtual DbSet<FilterDashboardParameter> FilterDashboardParameters { get; set; } = null!;
        public virtual DbSet<Gender> Genders { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<OrderDetail> OrderDetails { get; set; } = null!;
        public virtual DbSet<Origin> Origins { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<ProductCategory> ProductCategories { get; set; } = null!;
        public virtual DbSet<ProductSizePricing> ProductSizePricings { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public DbSet<Slider> SliderDetail { get; set; } = null!; // Đảm bảo bảng này kết nối với DbSet<Slider>
        public virtual DbSet<Source> Sources { get; set; } = null!;
        public virtual DbSet<SourcesFeedbackRespone> SourcesFeedbackRespones { get; set; } = null!;
        public virtual DbSet<Status> Statuses { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<UserRolesAudit> UserRolesAudits { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                String ConnectionStr = config.GetConnectionString("G3_PerfumeShopDB_Test_TestConnection");

                optionsBuilder.UseSqlServer(ConnectionStr);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Blog>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Author).HasMaxLength(50);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.ImageUrl)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Title).HasMaxLength(100);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.BlogCategory)
                    .WithMany(p => p.Blogs)
                    .HasForeignKey(d => d.BlogCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Blogs_BlogCategories");

                entity.HasOne(d => d.BlogStatus)
                    .WithMany(p => p.Blogs)
                    .HasForeignKey(d => d.BlogStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Blogs_BlogStatuses");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Blogs)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Blogs_Users");
            });

            modelBuilder.Entity<BlogCategory>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<BlogStatus>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<Brand>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.ImageUrl)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Name).HasMaxLength(100);
            });

            modelBuilder.Entity<CustomDashboard>(entity =>
            {
                entity.HasOne(d => d.FilterParam)
                    .WithMany(p => p.CustomDashboards)
                    .HasForeignKey(d => d.FilterParamId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CustomDashboards_FilterDashboardParameters");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.CustomDashboards)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CustomDashboards_Users");
            });

            modelBuilder.Entity<DashBoardUserOption>(entity =>
            {
                entity.HasOne(d => d.User)
                    .WithMany(p => p.DashBoardUserOptions)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DashBoardUserOptions_Users");
            });

            modelBuilder.Entity<DashboardSection>(entity =>
            {
                entity.Property(e => e.Description).HasMaxLength(100);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.DashboardSections)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DashboardSections_Roles");
            });

            modelBuilder.Entity<DefaultDashboard>(entity =>
            {
                entity.HasOne(d => d.FilterParam)
                    .WithMany(p => p.DefaultDashboards)
                    .HasForeignKey(d => d.FilterParamId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DefaultDashboards_FilterDashboardParameters");
            });

            modelBuilder.Entity<Feedback>(entity =>
            {
                entity.Property(e => e.Content).HasMaxLength(200);

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.Feedbacks)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Feedbacks_Orders");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Feedbacks)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Feedbacks_Products");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Feedbacks)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Feedbacks_Users");
            });

            modelBuilder.Entity<FeedbackResponse>(entity =>
            {
                entity.Property(e => e.ResponseContent).HasMaxLength(200);

                entity.Property(e => e.ResponseDate).HasColumnType("datetime");

                entity.HasOne(d => d.Feedback)
                    .WithMany(p => p.FeedbackResponses)
                    .HasForeignKey(d => d.FeedbackId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FeedbackResponses_Feedbacks");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.FeedbackResponses)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FeedbackResponses_Users");
            });

            modelBuilder.Entity<FilterDashboardParameter>(entity =>
            {
                entity.Property(e => e.Code).HasMaxLength(20);

                entity.Property(e => e.Description).HasMaxLength(100);

                entity.HasOne(d => d.DashboardSection)
                    .WithMany(p => p.FilterDashboardParameters)
                    .HasForeignKey(d => d.DashboardSectionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FilterDashboardParameters_DashboardSections");
            });

            modelBuilder.Entity<Gender>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.FirstName).HasMaxLength(200);

                entity.Property(e => e.LastName).HasMaxLength(200);

                entity.Property(e => e.PaymentMethod).HasMaxLength(50);

                entity.Property(e => e.Phone).HasMaxLength(50);

                entity.Property(e => e.ShippingAddress).HasMaxLength(254);

                entity.Property(e => e.ShippingEmail).HasMaxLength(200);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Orders_Users");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.ImageUrl).HasMaxLength(200);

                entity.Property(e => e.Title).HasMaxLength(200);

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderDetails_Orders");

                entity.HasOne(d => d.ProductSizePricing)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.ProductSizePricingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderDetails_ProductSizePricing");
            });

            modelBuilder.Entity<Origin>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CountryCode)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Name).HasMaxLength(100);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.ImageUrl)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Name).HasMaxLength(50);

                entity.Property(e => e.ReleaseYear).HasColumnType("date");

                entity.Property(e => e.Title).HasMaxLength(200);

                entity.HasOne(d => d.Brand)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.BrandId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Products_Brands");

                entity.HasOne(d => d.Gender)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.GenderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Products_Genders");

                entity.HasOne(d => d.Origin)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.OriginId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Products_Origins");

                entity.HasOne(d => d.ProductCategory)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.ProductCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Products_ProductCategories");
            });

            modelBuilder.Entity<ProductCategory>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Description).HasMaxLength(200);

                entity.Property(e => e.Name).HasMaxLength(150);
            });

            modelBuilder.Entity<ProductSizePricing>(entity =>
            {
                entity.ToTable("ProductSizePricing");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductSizePricings)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProductSizePricing_Products");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Description).HasMaxLength(50);

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<Slider>(entity =>
            {
                entity.Property(e => e.Title).HasMaxLength(255);
                entity.Property(e => e.ImagePath).HasMaxLength(500);
                entity.Property(e => e.Notes).HasMaxLength(1000);
            });

            modelBuilder.Entity<Source>(entity =>
            {
                entity.Property(e => e.Caption).HasMaxLength(255);

                entity.Property(e => e.FeedbackId).HasColumnName("FeedbackID");

                entity.Property(e => e.Url)
                    .HasMaxLength(256)
                    .IsUnicode(false);

                entity.HasOne(d => d.Feedback)
                    .WithMany(p => p.Sources)
                    .HasForeignKey(d => d.FeedbackId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Sources_Feedbacks");
            });

            modelBuilder.Entity<SourcesFeedbackRespone>(entity =>
            {
                entity.ToTable("SourcesFeedbackRespone");

                entity.Property(e => e.Caption).HasMaxLength(255);

                entity.Property(e => e.Url)
                    .HasMaxLength(256)
                    .IsUnicode(false);

                entity.HasOne(d => d.FeedbackResponse)
                    .WithMany(p => p.SourcesFeedbackRespones)
                    .HasForeignKey(d => d.FeedbackResponseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SourcesFeedbackRespone_FeedbackResponses");
            });

            modelBuilder.Entity<Status>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Description).HasMaxLength(200);

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Address).HasMaxLength(50);

                entity.Property(e => e.Birthdate).HasColumnType("date");

                entity.Property(e => e.Email).HasMaxLength(254);

                entity.Property(e => e.FirstName).HasMaxLength(50);

                entity.Property(e => e.ImageUrl)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.LastName).HasMaxLength(50);

                entity.Property(e => e.OtpExpiration).HasColumnType("datetime");

                entity.Property(e => e.Password).HasMaxLength(50);

                entity.Property(e => e.PasswordResetOtp).HasMaxLength(50);

                entity.Property(e => e.Phone).HasMaxLength(50);

                entity.Property(e => e.Username)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Users_Roles");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Users_Statuses");
            });

            modelBuilder.Entity<UserRolesAudit>(entity =>
            {
                entity.ToTable("UserRolesAudit");

                entity.Property(e => e.ChangeDate).HasColumnType("datetime");

                entity.Property(e => e.Note).HasMaxLength(200);

                entity.HasOne(d => d.ChangedByNavigation)
                    .WithMany(p => p.UserRolesAuditChangedByNavigations)
                    .HasForeignKey(d => d.ChangedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserRolesAudit_Users1");

                entity.HasOne(d => d.NewRole)
                    .WithMany(p => p.UserRolesAuditNewRoles)
                    .HasForeignKey(d => d.NewRoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserRolesAudit_Roles1");

                entity.HasOne(d => d.OldRole)
                    .WithMany(p => p.UserRolesAuditOldRoles)
                    .HasForeignKey(d => d.OldRoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserRolesAudit_Roles");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserRolesAuditUsers)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserRolesAudit_Users");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}