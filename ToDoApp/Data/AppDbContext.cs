using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ToDoApp.Models;

namespace ToDoApp.Data
{
    public class AppDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Goal>()
                .HasMany(G => G.SubGoals).WithOne(G => G.ParentGoal)
                .HasForeignKey(G => G.ParentGoalId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Goal>()
                .ToTable(T => T.HasCheckConstraint(
                    "CK_Goal_NoSelfParent",
                    "\"ParentGoalId\" IS NULL OR \"Id\" <> \"ParentGoalId\""));

            modelBuilder.Entity<TaskItem>()
                .HasOne(T => T.Goal).WithMany(G => G.Tasks)
                .HasForeignKey(T => T.GoalId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Activity>()
                .HasOne(A => A.User).WithMany(U => U.Activities)
                .HasForeignKey(A => A.UserId);

            modelBuilder.Entity<Activity>()
                .Property(A => A.ActivityType)
                .HasConversion<string>();

            modelBuilder.Entity<User>()
                .Property(U => U.TimeZoneId)
                .HasDefaultValue("Asia/Kuala_Lumpur");
        }

        public DbSet<TaskItem> Tasks { get; set; }
        public DbSet<Goal> Goals { get; set; }
        public DbSet<Activity> Activities { get; set; }
    }
}