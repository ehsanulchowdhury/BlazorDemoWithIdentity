using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SecureDemoClassLibrary.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public readonly Guid InstanceId = Guid.NewGuid();
        readonly string userId = Guid.NewGuid().ToString();
        readonly string roleId = Guid.NewGuid().ToString();
        public static ApplicationDbContext CreateDbContext(string connectionString, bool useSqlite)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            if (useSqlite)
            {
                var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
                builder.UseSqlite(connectionString);
                return new ApplicationDbContext(builder.Options);
            }
            else
            {
                var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
                builder.UseSqlServer(connectionString);
                return new ApplicationDbContext(builder.Options);
            }
        }

        public static void AddBaseOptions(DbContextOptionsBuilder<ApplicationDbContext> builder, string connectionString, bool useSqlite = true)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("Connection string must be provided", nameof(connectionString));

            if (useSqlite)
            {
                builder.UseSqlite(connectionString);
            }
            else
            {
                builder.UseSqlServer(connectionString, x =>
                {
                    x.EnableRetryOnFailure();
                });
            }
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Employee> Employees { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            //builder.UseSqlServer(@"Server=.;Database=Demo;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            ApplicationUser user = new ApplicationUser()
            {
                Id = userId,
                FirstName = "Admin",
                LastName = "Admin",
                UserName = "admin@gmail.com",
                //NormalizedUserName = "Admin".ToUpper(),
                Email = "admin@gmail.com",
                //NormalizedEmail = "admin@gmail.com".ToUpper(),
                LockoutEnabled = false,
                PhoneNumber = "1234567890",
                UserEntityType = "Admin"
            };

            PasswordHasher<ApplicationUser> passwordHasher = new PasswordHasher<ApplicationUser>();
            user.PasswordHash = passwordHasher.HashPassword(user, "Admin");

            builder.Entity<ApplicationUser>().HasData(user);

            IdentityRole role = new IdentityRole
            {
                Id = roleId,
                Name = "Admin",
                NormalizedName = "Admin".ToUpper()
            };

            builder.Entity<IdentityRole>().HasData(role);

            //if (user != null && role != null)
            //{
            //    builder.Entity<IdentityUserRole<Guid>>().HasData(new IdentityUserRole<Guid>
            //    {
            //        UserId = Guid.Parse(userId),
            //        RoleId = Guid.Parse(roleId)
            //    });
            //}
        }
    }

    public static class EntityExtensions
    {
        public static void Clear<T>(this DbSet<T> dbSet) where T : class
        {
            dbSet.RemoveRange(dbSet);
        }
    }



}
