using Microsoft.EntityFrameworkCore;
using Stationery_Management_System.Models;

namespace Stationery_Management_System.db_context
{
    public class sqldb : DbContext
    {
        public sqldb(DbContextOptions<sqldb>options) : base(options)
        {


        }

        public DbSet<users> users { get; set; }

        public DbSet<Stationery> Stationeries { get; set; }
        public DbSet<Stationery_Management_System.Models.UserRoles> UserRoles { get; set; } = default!;
        public DbSet<Request> Requests { get; set; }

        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserRoles>().HasData(
                new UserRoles() { UserRoleId = 1, UserRoleName ="Admin"}
                );

            modelBuilder.Entity<users>().HasData(
               new users() { userId = 1, UserName = "Admin",
                   UserEmail = "Admin@gmail.com",
                   UserPhone = "03123456789",
                   UserPassword = "123456789", // Hashed password hona chahiye agar authentication use kar rahe ho
                   UserRole = 1, // Admin Role
                   UserLimits = 100000,
                   Add_By = 1,
               }
               );
        }

    }
}
