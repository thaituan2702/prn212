﻿using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.Extensions.Configuration;
using VehicleEmissionManagement.Core.Modelss;

namespace VehicleEmissionManagement.Data.Contextt
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<InspectionStation> InspectionStations { get; set; }
        public DbSet<InspectionRecord> InspectionRecords { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        // Thêm constructor cho dependency injection
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Constructor này giữ lại để hỗ trợ các nơi cũ vẫn tạo context trực tiếp
        public ApplicationDbContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json")
                    .Build();

                optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnectionStringDB"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Giữ nguyên phần này
            modelBuilder.Entity<User>()
                .Property(u => u.CreatedAt)
                .HasColumnType("datetime2");

            modelBuilder.Entity<User>()
                .Property(u => u.UpdatedAt)
                .HasColumnType("datetime2");

            modelBuilder.Entity<Vehicle>()
           .Property(v => v.CreatedAt)
           .HasColumnType("datetime2");

            modelBuilder.Entity<Vehicle>()
                .Property(v => v.UpdatedAt)
                .HasColumnType("datetime2");

            modelBuilder.Entity<Vehicle>()
                .HasOne(v => v.Owner)
                .WithMany()
                .HasForeignKey(v => v.OwnerID);


            modelBuilder.Entity<InspectionStation>()
        .HasKey(s => s.StationID);

            modelBuilder.Entity<InspectionStation>()
                .Property(s => s.CreatedAt)
                .HasColumnType("datetime2");

            modelBuilder.Entity<InspectionStation>()
                .Property(s => s.UpdatedAt)
                .HasColumnType("datetime2");

            modelBuilder.Entity<InspectionRecord>()
           .HasKey(r => r.RecordID);

            modelBuilder.Entity<InspectionRecord>()
                .Property(r => r.CreatedAt)
                .HasColumnType("datetime2");

            modelBuilder.Entity<InspectionRecord>()
                .Property(r => r.UpdatedAt)
                .HasColumnType("datetime2");

            modelBuilder.Entity<InspectionRecord>()
                .Property(r => r.ExpiryDate)
                .HasColumnType("datetime2");

            modelBuilder.Entity<InspectionRecord>()
                .Property(r => r.InspectionDate)
                .HasColumnType("datetime2");

            modelBuilder.Entity<InspectionRecord>()
                .HasOne(r => r.Vehicle)
                .WithMany()
                .HasForeignKey(r => r.VehicleID);

            modelBuilder.Entity<InspectionRecord>()
                .HasOne(r => r.Station)
                .WithMany()
                .HasForeignKey(r => r.StationID);

            modelBuilder.Entity<InspectionRecord>()
                .HasOne(r => r.Inspector)
                .WithMany()
                .HasForeignKey(r => r.InspectorID);

            modelBuilder.Entity<Notification>()
            .HasKey(n => n.NotificationID);

            modelBuilder.Entity<Notification>()
                .Property(n => n.CreatedAt)
                .HasColumnType("datetime2");

            modelBuilder.Entity<Notification>()
                .Property(n => n.SentDate)
                .HasColumnType("datetime2");

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserID);
        }

    }
    }
