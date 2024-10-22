using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Lab_3.Models;

public partial class DatabaseContext : DbContext
{
    public DatabaseContext()
    {
    }

    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ArchiveShow> ArchiveShows { get; set; }

    public virtual DbSet<Broadcasting> Broadcastings { get; set; }

    public virtual DbSet<CitizenAppeal> CitizenAppeals { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<Tvshow> Tvshows { get; set; }

    public virtual DbSet<WeeklySchedule> WeeklySchedules { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var builder = new ConfigurationBuilder();
        builder.AddJsonFile("D:\\Learning\\Курсач АСП\\appsettings.json");
        var config = builder.Build();
        string connectionString = config.GetConnectionString("DefaultConnection");         
        optionsBuilder.UseSqlServer(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ArchiveShow>(entity =>
        {
            entity.HasKey(e => e.ArchiveId).HasName("PK__ArchiveS__33A73E77707EF574");

            entity.Property(e => e.ArchiveId).HasColumnName("ArchiveID");
            entity.Property(e => e.AirDate).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.GenreId).HasColumnName("GenreID");
            entity.Property(e => e.ShowId).HasColumnName("ShowID");
            entity.Property(e => e.ShowName).HasMaxLength(100);

            entity.HasOne(d => d.Show).WithMany(p => p.ArchiveShows)
                .HasForeignKey(d => d.ShowId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ArchiveSh__ShowI__440B1D61");
        });

        modelBuilder.Entity<Broadcasting>(entity =>
        {
            entity.HasKey(e => e.BroadcastingId).HasName("PK__Broadcas__AFB829B873E8CCD1");

            entity.ToTable("Broadcasting");

            entity.Property(e => e.BroadcastingId).HasColumnName("BroadcastingID");
            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.ShowId).HasColumnName("ShowID");

            entity.HasOne(d => d.Employee).WithMany(p => p.Broadcastings)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Broadcast__Emplo__46E78A0C");

            entity.HasOne(d => d.Show).WithMany(p => p.Broadcastings)
                .HasForeignKey(d => d.ShowId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Broadcast__ShowI__47DBAE45");
        });

        modelBuilder.Entity<CitizenAppeal>(entity =>
        {
            entity.HasKey(e => e.AppealId).HasName("PK__CitizenA__BB684E100BA68C13");

            entity.Property(e => e.AppealId).HasColumnName("AppealID");
            entity.Property(e => e.AppealPurpose).HasMaxLength(255);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Organization).HasMaxLength(100);
            entity.Property(e => e.ShowId).HasColumnName("ShowID");

            entity.HasOne(d => d.Show).WithMany(p => p.CitizenAppeals)
                .HasForeignKey(d => d.ShowId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CitizenAp__ShowI__3F466844");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__7AD04FF1F2A8C208");

            entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Position).HasMaxLength(100);
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(e => e.GenreId).HasName("PK__Genres__0385055E20ECC719");

            entity.Property(e => e.GenreId).HasColumnName("GenreID");
            entity.Property(e => e.GenreDescription).HasMaxLength(255);
            entity.Property(e => e.GenreName).HasMaxLength(100);
        });

        modelBuilder.Entity<Tvshow>(entity =>
        {
            entity.HasKey(e => e.ShowId).HasName("PK__TVShows__6DE3E0D2DF1E74FA");

            entity.ToTable("TVShows");

            entity.Property(e => e.ShowId).HasColumnName("ShowID");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Employees).HasMaxLength(255);
            entity.Property(e => e.GenreId).HasColumnName("GenreID");
            entity.Property(e => e.Rating).HasColumnType("decimal(3, 2)");
            entity.Property(e => e.ShowName).HasMaxLength(100);

            entity.HasOne(d => d.Genre).WithMany(p => p.Tvshows)
                .HasForeignKey(d => d.GenreId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TVShows__GenreID__398D8EEE");
        });

        modelBuilder.Entity<WeeklySchedule>(entity =>
        {
            entity.HasKey(e => e.ScheduleId).HasName("PK__WeeklySc__9C8A5B694F9B5FE7");

            entity.ToTable("WeeklySchedule");

            entity.Property(e => e.ScheduleId).HasColumnName("ScheduleID");
            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.Guests).HasMaxLength(255);
            entity.Property(e => e.ShowId).HasColumnName("ShowID");
            entity.Property(e => e.StartTime).HasColumnType("datetime");

            entity.HasOne(d => d.Show).WithMany(p => p.WeeklySchedules)
                .HasForeignKey(d => d.ShowId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__WeeklySch__ShowI__3C69FB99");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
