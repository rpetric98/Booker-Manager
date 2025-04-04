using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Data.Models;

public partial class ProjektRwaContext : DbContext
{
    public ProjektRwaContext()
    {
    }

    public ProjektRwaContext(DbContextOptions<ProjektRwaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Amenity> Amenities { get; set; }

    public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<Property> Properties { get; set; }

    public virtual DbSet<PropertyAmenity> PropertyAmenities { get; set; }

    public virtual DbSet<PropertyType> PropertyTypes { get; set; }

    public virtual DbSet<Reservation> Reservations { get; set; }

    public virtual DbSet<UserDetail> UserDetails { get; set; }

    public virtual DbSet<UserMessage> UserMessages { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("server=LAPTOPRP\\SQLEXPRESSDB;Database=ProjektRWA;User=sa;Password=SQL;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Amenity>(entity =>
        {
            entity.HasKey(e => e.AmenityId).HasName("PK__Amenity__842AF52BEB3B5E29");

            entity.ToTable("Amenity");

            entity.Property(e => e.AmenityId).HasColumnName("AmenityID");
            entity.Property(e => e.AmenityName).HasMaxLength(255);
        });

        modelBuilder.Entity<Log>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__Log__5E54864853E7F0D9");

            entity.ToTable("Log");

            entity.Property(e => e.Level).HasMaxLength(100);
            entity.Property(e => e.Message).HasMaxLength(100);
            entity.Property(e => e.Timestamp).HasColumnType("datetime");
        });

        modelBuilder.Entity<Property>(entity =>
        {
            entity.HasKey(e => e.PropertyId).HasName("PK__Property__70C9A7553F5328ED");

            entity.ToTable("Property");

            entity.Property(e => e.PropertyId).HasColumnName("PropertyID");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.Country).HasMaxLength(100);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.PropertyTypeId).HasColumnName("PropertyTypeID");
            entity.Property(e => e.ZipCode).HasMaxLength(100);

            entity.HasOne(d => d.PropertyType).WithMany(p => p.Properties)
                .HasForeignKey(d => d.PropertyTypeId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Property_PropertyType");
        });

        modelBuilder.Entity<PropertyAmenity>(entity =>
        {
            entity.HasKey(e => e.PropertyAmenityId).HasName("PK__Property__0D6BB43EB062D99E");

            entity.ToTable("PropertyAmenity");

            entity.Property(e => e.PropertyAmenityId).HasColumnName("PropertyAmenityID");
            entity.Property(e => e.AmenityId).HasColumnName("AmenityID");
            entity.Property(e => e.PropertyId).HasColumnName("PropertyID");

            entity.HasOne(d => d.Amenity).WithMany(p => p.PropertyAmenities)
                .HasForeignKey(d => d.AmenityId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_PropertyAmenity_Amenity");

            entity.HasOne(d => d.Property).WithMany(p => p.PropertyAmenities)
                .HasForeignKey(d => d.PropertyId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_PropertyAmenity_Property");
        });

        modelBuilder.Entity<PropertyType>(entity =>
        {
            entity.HasKey(e => e.PropertyTypeId).HasName("PK__Property__BDE14DD44AFFCDDC");

            entity.ToTable("PropertyType");

            entity.Property(e => e.PropertyTypeId).HasColumnName("PropertyTypeID");
            entity.Property(e => e.PropertyTypeName).HasMaxLength(100);
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.ReservationId).HasName("PK__Reservat__B7EE5F24464AA389");

            entity.ToTable("Reservation");

            entity.Property(e => e.CheckInDate).HasColumnType("date");
            entity.Property(e => e.CheckOutDate).HasColumnType("date");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Username).HasMaxLength(100);

            entity.HasOne(d => d.Property).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.PropertyId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Reservation_Property");
        });

        modelBuilder.Entity<UserDetail>(entity =>
        {
            entity.HasKey(e => e.UserDetailId).HasName("PK__UserDeta__564F56D26CEBEDD1");

            entity.ToTable("UserDetail");

            entity.Property(e => e.UserDetailId).HasColumnName("UserDetailID");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(256);
            entity.Property(e => e.PasswordSalt).HasMaxLength(256);
            entity.Property(e => e.Phone).HasMaxLength(100);
            entity.Property(e => e.UserRoleId).HasColumnName("UserRoleID");
            entity.Property(e => e.Username).HasMaxLength(100);

            entity.HasOne(d => d.UserRole).WithMany(p => p.UserDetails)
                .HasForeignKey(d => d.UserRoleId)
                .HasConstraintName("FK_UserDetail_UserRole");
        });

        modelBuilder.Entity<UserMessage>(entity =>
        {
            entity.HasKey(e => e.UserMessageId).HasName("PK__UserMess__F6F57F83DF3C685F");

            entity.ToTable("UserMessage");

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Username).HasMaxLength(100);
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.UserRoleId).HasName("PK__UserRole__3D978A55D335FED5");

            entity.ToTable("UserRole");

            entity.Property(e => e.UserRoleId).HasColumnName("UserRoleID");
            entity.Property(e => e.UserRoleName).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
