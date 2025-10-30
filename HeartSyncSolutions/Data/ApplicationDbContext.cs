using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HeartSyncSolutions.Models;

namespace HeartSyncSolutions.Data
{
    
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Making a DbSet for every single model we made.
        // This tells EF "Here is your tables".
        public DbSet<Event> Events { get; set; }

        public DbSet<EventType> EventTypes { get; set; }

        public DbSet<EventStatus> EventStatuses { get; set; }

        public DbSet<UserEvent> UserEvents { get; set; }

        public DbSet<AttendanceStatus> AttendanceStatuses { get; set; }

        public DbSet<MonetaryDonations> MonetaryDonations { get; set; }

        public DbSet<PartnerPledge> PartnerPledges { get; set; }

        public DbSet<InKindOffer> InKindOffers { get; set; }

        public DbSet<EventGallery> EventGalleries { get; set; }

        public DbSet<EventReport> EventReports { get; set; }

        // Ensuring the 1-to-1 relationships are setup right.
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // This sets up the 1-to-1 between ApplicationUser and PartnerPledge
            builder.Entity<ApplicationUser>()
                .HasOne(a => a.PartnerPledge)
                .WithOne(p => p.ApplicationUser)
                .HasForeignKey<PartnerPledge>(p => p.ApplicationUserID);

            // This sets up the 1-to-1 between Event and EventReport
            builder.Entity<Event>()
                .HasOne(e => e.EventReport)
                .WithOne(r => r.Event)
                .HasForeignKey<EventReport>(r => r.EventID);
        }
    }
}
