using Microsoft.EntityFrameworkCore;
using NewDiplom.Entities;

namespace NewDiplom.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<Role> Roles => Set<Role>();

    public DbSet<PostOffice> PostOffices => Set<PostOffice>();
    public DbSet<ServiceType> ServiceTypes => Set<ServiceType>();

    public DbSet<DeliveryMethod> DeliveryMethods => Set<DeliveryMethod>();

    public DbSet<ShipmentStatus> ShipmentStatuses => Set<ShipmentStatus>();

    public DbSet<Shipment> Shipments => Set<Shipment>();

    public DbSet<ShipmentTracking> ShipmentTrackings => Set<ShipmentTracking>();

    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(x => x.Login)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(x => x.Email)
            .IsUnique();

        modelBuilder.Entity<PostOffice>()
            .HasIndex(x => x.Code)
            .IsUnique();

        modelBuilder.Entity<Shipment>()
            .HasIndex(x => x.TrackingNumber)
            .IsUnique();

        modelBuilder.Entity<Shipment>()
            .HasOne(x => x.Client)
            .WithMany()
            .HasForeignKey(x => x.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Shipment>()
            .HasOne(x => x.SenderEmployee)
            .WithMany()
            .HasForeignKey(x => x.SenderEmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Shipment>()
            .HasOne(x => x.CurrentOffice)
            .WithMany()
            .HasForeignKey(x => x.CurrentOfficeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Shipment>()
            .HasOne(x => x.DestinationOffice)
            .WithMany()
            .HasForeignKey(x => x.DestinationOfficeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ShipmentTracking>()
            .HasOne(x => x.Shipment)
            .WithMany()
            .HasForeignKey(x => x.ShipmentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ShipmentTracking>()
            .HasOne(x => x.Office)
            .WithMany()
            .HasForeignKey(x => x.OfficeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ShipmentTracking>()
            .HasOne(x => x.Status)
            .WithMany()
            .HasForeignKey(x => x.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Shipment>()
            .Property(x => x.WeightKg)
            .HasPrecision(10, 2);

        modelBuilder.Entity<Shipment>()
            .Property(x => x.DeclaredValue)
            .HasPrecision(10, 2);

        modelBuilder.Entity<Shipment>()
            .Property(x => x.TotalPrice)
            .HasPrecision(10, 2);

        modelBuilder.Entity<ServiceType>()
            .Property(x => x.BasePrice)
            .HasPrecision(10, 2);

        modelBuilder.Entity<DeliveryMethod>()
            .Property(x => x.AdditionalPrice)
            .HasPrecision(10, 2);

        //modelBuilder.Entity<Shipment>()
        //.HasOne(x => x.Client)
        //.WithMany()
        //.HasForeignKey(x => x.ClientId)
        //.OnDelete(DeleteBehavior.Restrict);

        //modelBuilder.Entity<Shipment>()
        //    .HasOne(x => x.RecipientClient)
        //    .WithMany()
        //    .HasForeignKey(x => x.RecipientClientId)
        //    .OnDelete(DeleteBehavior.Restrict);
    }
}