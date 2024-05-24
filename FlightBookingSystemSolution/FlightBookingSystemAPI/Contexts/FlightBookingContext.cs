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
    }
}
