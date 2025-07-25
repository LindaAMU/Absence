using Absence.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Absence.Domain.Context
{
    public class AbsenceContext : DbContext
    {
        public AbsenceContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<AbsenceRequest> Requests { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Session> Sessions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<AbsenceRequest>()
                .HasOne(r => r.User)
                .WithMany(u => u.AbsenceRequests)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Role>()
                .HasOne(r => r.User)
                .WithMany(u => u.Roles)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Session)
                .WithOne(u => u.User)
                .HasForeignKey<Session>(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Session>()
                .HasIndex(s => s.UserId)
                .IsUnique();
        }
    }
}
