using FlightBookingSystemAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightBookingSystemAPI.Contexts
{
    public class FlightBookingContext:DbContext
    {
        public FlightBookingContext(DbContextOptions options) : base(options) { 

        }
        public DbSet<User> Users { get; set; }
        public DbSet<UserDetail> UserDetails { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingDetail> BookingDetails { get; set; }
        public DbSet<Flight> Flights { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Passenger> Passengers { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<RouteInfo> RouteInfos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure foreign key relationships

            // Booking to Schedule
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.FlightDetails)
                .WithMany()
                .HasForeignKey(b => b.ScheduleId)
                .OnDelete(DeleteBehavior.Restrict);

            // BookingDetail to Booking
            modelBuilder.Entity<BookingDetail>()
                .HasOne(bd => bd.Bookings)
                .WithMany()
                .HasForeignKey(bd => bd.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            // Payment to Booking
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.BookingInfo)
                .WithMany()
                .HasForeignKey(p => p.BookingId)
                .OnDelete(DeleteBehavior.Restrict);

            // Schedule to RouteInfo and Flight
            modelBuilder.Entity<Schedule>()
                .HasOne(s => s.RouteInfo)
                .WithMany()
                .HasForeignKey(s => s.RouteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Schedule>()
                .HasOne(s => s.FlightInfo)
                .WithMany()
                .HasForeignKey(s => s.FlightId)
                .OnDelete(DeleteBehavior.Restrict);

            // Other entity configurations

            // BookingDetail to Passenger
            modelBuilder.Entity<BookingDetail>()
                .HasOne(bd => bd.PassengerDetail)
                .WithMany()
                .HasForeignKey(bd => bd.PassengerId)
                .OnDelete(DeleteBehavior.Cascade);

            // UserDetail to User
            modelBuilder.Entity<UserDetail>()
                .HasOne(ud => ud.User)
                .WithOne()
                .HasForeignKey<UserDetail>(ud => ud.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Add similar configurations for other entities...


        }
    }
    
}
