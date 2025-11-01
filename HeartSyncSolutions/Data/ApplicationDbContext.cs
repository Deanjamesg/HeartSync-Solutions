using HeartSyncSolutions.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HeartSyncSolutions.Data
{

    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        //DbSet properties for all entities
        public DbSet<Event> Events { get; set; }
        public DbSet<EventType> EventTypes { get; set; }
        public DbSet<EventStatus> EventStatuses { get; set; }
        public DbSet<UserEvent> UserEvents { get; set; }
        public DbSet<AttendanceStatus> AttendanceStatuses { get; set; }
        public DbSet<MonetaryDonation> MonetaryDonations { get; set; }
        public DbSet<MonetaryDonationStatus> MonetaryDonationStatuses { get; set; }
        public DbSet<InKindDonation> InKindDonations { get; set; }
        public DbSet<InKindStatus> InKindStatuses { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Event entity
            modelBuilder.Entity<Event>(entity =>
            {
                entity.HasKey(e => e.EventID);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Location)
                    .HasMaxLength(300);

                entity.Property(e => e.Date)
                    .IsRequired();

                // Relationship with EventType
                entity.HasOne(e => e.EventType)
                    .WithMany(et => et.Events)
                    .HasForeignKey(e => e.EventTypeID)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relationship with EventStatus
                entity.HasOne(e => e.EventStatus)
                    .WithMany(es => es.Events)
                    .HasForeignKey(e => e.EventStatusID)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure EventType entity
            modelBuilder.Entity<EventType>(entity =>
            {
                entity.HasKey(et => et.EventTypeID);

                entity.Property(et => et.Title)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            // Configure EventStatus entity
            modelBuilder.Entity<EventStatus>(entity =>
            {
                entity.HasKey(es => es.EventStatusID);

                entity.Property(es => es.Status)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            // Configure UserEvent entity
            modelBuilder.Entity<UserEvent>(entity =>
            {
                entity.HasKey(ue => ue.UserEventID);

                // Relationship with User
                entity.HasOne(ue => ue.User)
                    .WithMany(u => u.UserEvents)
                    .HasForeignKey(ue => ue.UserID)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relationship with Event
                entity.HasOne(ue => ue.Event)
                    .WithMany(e => e.UserEvents)
                    .HasForeignKey(ue => ue.EventID)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relationship with AttendanceStatus
                entity.HasOne(ue => ue.AttendanceStatus)
                    .WithMany(a => a.UserEvents)
                    .HasForeignKey(ue => ue.AttendanceStatusID)
                    .OnDelete(DeleteBehavior.Restrict);

                // Create a unique index to prevent duplicate sign-ups
                entity.HasIndex(ue => new { ue.UserID, ue.EventID })
                    .IsUnique();
            });

            // Configure AttendanceStatus entity
            modelBuilder.Entity<AttendanceStatus>(entity =>
            {
                entity.HasKey(a => a.AttendanceStatusID);

                entity.Property(a => a.Status)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            // Configure User entity (extends IdentityUser)
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(u => u.FirstName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(u => u.LastName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(u => u.ContactNumber)
                    .HasMaxLength(20);

                entity.Property(u => u.IsVolunteer)
                    .HasDefaultValue(false);

                entity.Property(u => u.IsDonor)
                    .HasDefaultValue(false);
            });

            // Configure MonetaryDonation entity
            modelBuilder.Entity<MonetaryDonation>(entity =>
            {
                entity.HasKey(md => md.MonetaryDonationID);

                entity.Property(md => md.DonationAmount)
                    .IsRequired()
                    .HasPrecision(18, 2);

                entity.Property(md => md.Date)
                    .IsRequired();

                // Relationship with User
                entity.HasOne(md => md.User)
                    .WithMany(u => u.MonetaryDonations)
                    .HasForeignKey(md => md.UserID)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relationship with MonetaryDonationStatus
                entity.HasOne(md => md.MonetaryDonationStatus)
                    .WithMany(mds => mds.MonetaryDonations)
                    .HasForeignKey(md => md.MonetaryDonationStatusID)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure MonetaryDonationStatus entity
            modelBuilder.Entity<MonetaryDonationStatus>(entity =>
            {
                entity.HasKey(mds => mds.MonetaryDonationStatusID);

                entity.Property(mds => mds.Status)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            // Configure InKindDonation entity
            modelBuilder.Entity<InKindDonation>(entity =>
            {
                entity.HasKey(ikd => ikd.InKindDonationID);

                entity.Property(ikd => ikd.ItemDescription)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(ikd => ikd.DeliveryDate)
                    .IsRequired();

                // Relationship with User
                entity.HasOne(ikd => ikd.User)
                    .WithMany(u => u.InKindDonations)
                    .HasForeignKey(ikd => ikd.UserID)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relationship with InKindStatus
                entity.HasOne(ikd => ikd.InKindStatus)
                    .WithMany(iks => iks.InKindDonation)
                    .HasForeignKey(ikd => ikd.InKindStatusID)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure InKindStatus entity
            modelBuilder.Entity<InKindStatus>(entity =>
            {
                entity.HasKey(iks => iks.InKindStatusID);

                entity.Property(iks => iks.Status)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            // Seed initial data for lookup tables
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed EventStatuses
            modelBuilder.Entity<EventStatus>().HasData(
                new EventStatus { EventStatusID = "es-11111111-1111-1111-1111-111111111111", Status = "Upcoming" },
                new EventStatus { EventStatusID = "es-33333333-3333-3333-3333-333333333333", Status = "Completed" },
                new EventStatus { EventStatusID = "es-44444444-4444-4444-4444-444444444444", Status = "Cancelled" }
                );

            // Seed EventTypes
            modelBuilder.Entity<EventType>().HasData(
                new EventType { EventTypeID = "et-11111111-1111-1111-1111-111111111111", Title = "Food Drive" },
                new EventType { EventTypeID = "et-22222222-2222-2222-2222-222222222222", Title = "Clothing Drive" },
                new EventType { EventTypeID = "et-33333333-3333-3333-3333-333333333333", Title = "Outing" }
                );

            // Seed AttendanceStatuses
            modelBuilder.Entity<AttendanceStatus>().HasData(
                new AttendanceStatus { AttendanceStatusID = "as-11111111-1111-1111-1111-111111111111", Status = "Signed Up" },
                new AttendanceStatus { AttendanceStatusID = "as-22222222-2222-2222-2222-222222222222", Status = "Attended" },
                new AttendanceStatus { AttendanceStatusID = "as-33333333-3333-3333-3333-333333333333", Status = "No Show" },
                new AttendanceStatus { AttendanceStatusID = "as-44444444-4444-4444-4444-444444444444", Status = "Cancelled" }
                );

            // Seed MonetaryDonationStatuses
            modelBuilder.Entity<MonetaryDonationStatus>().HasData(
                new MonetaryDonationStatus { MonetaryDonationStatusID = "mds-11111111-1111-1111-1111-111111111111", Status = "Pending" },
                new MonetaryDonationStatus { MonetaryDonationStatusID = "mds-22222222-2222-2222-2222-222222222222", Status = "Completed" },
                new MonetaryDonationStatus { MonetaryDonationStatusID = "mds-33333333-3333-3333-3333-333333333333", Status = "Cancelled" }
                );

            // Seed InKindStatuses
            modelBuilder.Entity<InKindStatus>().HasData(
                new InKindStatus { InKindStatusID = "iks-11111111-1111-1111-1111-111111111111", Status = "Pending" },
                new InKindStatus { InKindStatusID = "iks-22222222-2222-2222-2222-222222222222", Status = "Approved" },
                new InKindStatus { InKindStatusID = "iks-33333333-3333-3333-3333-333333333333", Status = "Completed" },
                new InKindStatus { InKindStatusID = "iks-44444444-4444-4444-4444-444444444444", Status = "Cancelled" }
                );
        }

    }
}
