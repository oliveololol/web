using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace web.Models
{
    public partial class userContext : DbContext
    {
        public userContext()
        {
        }

        public userContext(DbContextOptions<userContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Event> Event { get; set; }
        public virtual DbSet<Log> Log { get; set; }
        public virtual DbSet<Users> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

                optionsBuilder.UseSqlServer("Data Source=.\\SQLEXPRESS;Initial Catalog=user;uid=sqluser;pwd=123");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Event>(entity =>
            {
                entity.ToTable("event");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Nameevent)
                    .HasColumnName("nameevent")
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Log>(entity =>
            {
                entity.ToTable("log");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.IdEvent).HasColumnName("id_event");

                entity.Property(e => e.IdUsers).HasColumnName("id_users");

                entity.HasOne(d => d.IdEventNavigation)
                    .WithMany(p => p.Log)
                    .HasForeignKey(d => d.IdEvent)
                    .HasConstraintName("FK_log_event");

                entity.HasOne(d => d.IdUsersNavigation)
                    .WithMany(p => p.Log)
                    .HasForeignKey(d => d.IdUsers)
                    .HasConstraintName("FK_log_users");
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.ToTable("users");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Login)
                    .HasColumnName("login")
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(50);

                entity.Property(e => e.Parol)
                    .HasColumnName("parol")
                    .HasMaxLength(150);

                entity.Property(e => e.Surname)
                    .HasColumnName("surname")
                    .HasMaxLength(50);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
