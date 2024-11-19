using Microsoft.EntityFrameworkCore;
using ClassroomBooking.Api.Models;

namespace ClassroomBooking.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<LoginLog> LoginLogs { get; set; }
        public DbSet<Classroom> Classrooms { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.AdUsername)
                .IsUnique();

            modelBuilder.Entity<LoginLog>()
                .HasOne(l => l.User)
                .WithMany()
                .HasForeignKey(l => l.UserId);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Classroom)
                .WithMany()
                .HasForeignKey(r => r.ClassroomId);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId);
        }
    }
} 