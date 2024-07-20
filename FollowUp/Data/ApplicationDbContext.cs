using FollowUp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace FollowUp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<ApplicationRole> ApplicationRoles { get; set; }
        public DbSet<Department> Departments { get; set; } 
        public DbSet<Build> Builds { get; set; } 
        public DbSet<Course> Courses { get; set; } 
        public DbSet<Table> Tables { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Activation> Activations { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            string ADMIN_ID = "ecc07b18-f55e-4f6b-95bd-0e84f556135f";
            string ADMIN_ROLE_ID = "ba51b8f7-2a1d-45c6-9c00-68099eebd485";

            string TRAINER_ROLE_ID = "68099eeb-4f6b-45c6-9c00-68099eebd485";

            string TRAINEE_ROLE_ID = "9eebd485-2a1d-45c6-9c00-68099eebd485";

            builder.Entity<ApplicationRole>().HasData(new ApplicationRole
            {
                Name = "Admin",
                NormalizedName = "ADMIN",
                Id = ADMIN_ROLE_ID,
                ConcurrencyStamp = ADMIN_ROLE_ID,
                ArabicRoleName = "الادمن"
            });

            builder.Entity<ApplicationRole>().HasData(new ApplicationRole
            {
                Name = "Supervisor",
                NormalizedName = "SUPERVISOR",
                Id = TRAINEE_ROLE_ID,
                ConcurrencyStamp = TRAINEE_ROLE_ID,
                ArabicRoleName = "المشرف"
            });

            builder.Entity<ApplicationRole>().HasData(new ApplicationRole
            {
                Name = "Trainer",
                NormalizedName = "TRAINER",
                Id = TRAINER_ROLE_ID,
                ConcurrencyStamp = TRAINER_ROLE_ID,
                ArabicRoleName = "المدرب"
            });

            var appUser = new ApplicationUser
            {
                Id = ADMIN_ID,
                EmailConfirmed = true,
                UserFullName = "الادمن",
                UserName = "Admin",
                NormalizedUserName = "ADMIN",
            };

            PasswordHasher<ApplicationUser> ph = new PasswordHasher<ApplicationUser>();
            appUser.PasswordHash = ph.HashPassword(appUser, "P@ssw0rd");

            builder.Entity<ApplicationUser>().HasData(appUser);

            builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                RoleId = ADMIN_ROLE_ID,
                UserId = ADMIN_ID
            });

            base.OnModelCreating(builder);
        }
    }
}