using MeetSlot.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MeetSlot.Data
{
    public class MeetSlotDbContext : DbContext // DbContext-klassen for MeetSlot-applikasjonen, som representerer en session med databasen og gir tilgang til databasetabellene gjennom DbSet-egenskapene. Den konfigurerer også modellene og deres relasjoner i OnModelCreating-metoden.
    {
        public MeetSlotDbContext(DbContextOptions<MeetSlotDbContext> options)
            : base(options)
        {
        }

        public DbSet<MeetingRoom> MeetingRooms { get; set; } // DbSet for MeetingRoom-modellen, representerer tabellen i databasen for møterom.
        public DbSet<AppUser> AppUsers { get; set; } // DbSet for AppUser-modellen, representerer tabellen i databasen for brukere.
        public DbSet<Booking> Bookings { get; set; } // DbSet for Booking-modellen, representerer tabellen i databasen for bookinger.

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Normaliserer booking-tid til UTC ved lagring/lesing for konsistent tidslogikk.
            var utcDateTimeConverter = new ValueConverter<DateTime, DateTime>(
                value => NormalizeToUtc(value),
                value => DateTime.SpecifyKind(value, DateTimeKind.Utc));

            modelBuilder.Entity<MeetingRoom>(entity => // Konfigurerer MeetingRoom-modellen.
            {
                entity.Property(r => r.Name) // Navn er påkrevd og har en maksimal lengde på 100 tegn.
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(r => r.Capacity) // Kapasitet er påkrevd og må være positiv.
                    .IsRequired();

                entity.HasIndex(r => r.Name) // Navn må være unikt.
                    .IsUnique(); // Legger til en unik indeks på Name-kolonnen for å sikre at ingen møterom har samme navn.

                entity.ToTable(t => t.HasCheckConstraint(// Legger til en sjekk-konstraint for å sikre at Capacity er større enn 0.
                    "CK_MeetingRooms_Capacity_Positive",
                    "\"Capacity\" > 0"));
            });

            modelBuilder.Entity<AppUser>(entity => // Konfigurerer AppUser-modellen.
            {
                entity.Property(u => u.Email) // E-post er påkrevd, må være en gyldig e-postadresse og har en maksimal lengde på 256 tegn.
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(u => u.PasswordHash) // Passord-hash er påkrevd og har en maksimal lengde på 512 tegn.
                    .IsRequired()
                    .HasMaxLength(512);

                entity.Property(u => u.Role) // Roller er påkrevd, lagres som int i databasen og har en standardverdi på User.
                    .HasConversion<int>()
                    .HasDefaultValue(UserRole.User);

                entity.HasIndex(u => u.Email)
                    .IsUnique();
            });

            modelBuilder.Entity<Booking>(entity => // Konfigurerer Booking-modellen.
            {
                entity.Property(b => b.StartTime) // StartTime lagres som timestamptz for å sikre konsistent UTC-håndtering i PostgreSQL.
                    .HasConversion(utcDateTimeConverter)
                    .HasColumnType("timestamp with time zone");

                entity.Property(b => b.EndTime) // EndTime lagres også som timestamptz for samme UTC-strategi.
                    .HasConversion(utcDateTimeConverter)
                    .HasColumnType("timestamp with time zone");

                entity.ToTable(t => t.HasCheckConstraint(// Legger til en sjekk-konstraint for å sikre at EndTime er etter StartTime.
                    "CK_Bookings_EndTime_After_StartTime",
                    "\"EndTime\" > \"StartTime\""));

                entity.HasOne(b => b.MeetingRoom) // Konfigurerer relasjonen mellom Booking og MeetingRoom, hvor en booking har ett møterom, og et møterom kan ha mange bookinger. Sletting av et møterom vil ikke slette tilhørende bookinger.
                    .WithMany(r => r.Bookings)
                    .HasForeignKey(b => b.MeetingRoomId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(b => b.AppUser) // Konfigurerer relasjonen mellom Booking og AppUser, hvor en booking har en bruker, og en bruker kan ha mange bookinger. Sletting av en bruker vil ikke slette tilhørende bookinger.
                    .WithMany(u => u.Bookings)
                    .HasForeignKey(b => b.AppUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        private static DateTime NormalizeToUtc(DateTime value) // Hjelpemetode for å normalisere DateTime-verdier til UTC før lagring i databasen, og sikre at alle tidsverdier håndteres konsistent som UTC.
        {
            if (value.Kind == DateTimeKind.Utc)
            {
                return value;
            }

            if (value.Kind == DateTimeKind.Local)
            {
                return value.ToUniversalTime();
            }

            return DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }
    }
}