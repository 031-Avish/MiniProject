using FlightBookingSystemAPI.Contexts;
using FlightBookingSystemAPI.Exceptions.RepositoryException;
using FlightBookingSystemAPI.Interfaces;
using FlightBookingSystemAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlightBookingSystemAPI.Repositories
{
    public class BookingRepository : IRepository<int, Booking>
    {
        private readonly FlightBookingContext _context;

        public BookingRepository(FlightBookingContext context)
        {
            _context = context;
        }

        public async Task<Booking> Add(Booking item)
        {
            try
            {
                _context.Add(item);
                await _context.SaveChangesAsync();
                return item;
            }
            catch (Exception ex)
            {
                throw new BookingRepositoryException("Error occurred while adding booking.", ex);
            }
        }

        public async Task<Booking> DeleteByKey(int key)
        {
            try
            {
                var booking = await GetByKey(key);
                _context.Remove(booking);
                await _context.SaveChangesAsync(true);
                return booking;
            }
            catch (NotPresentException ex)
            {
                throw new BookingRepositoryException("Error occurred while deleting booking. Booking not found. " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new BookingRepositoryException("Error occurred while deleting booking.", ex);
            }
        }

        public async Task<IEnumerable<Booking>> GetAll()
        {
            try
            {
                var bookings = await _context.Bookings.ToListAsync();
                if (bookings.Count <= 0)
                {
                    throw new NotPresentException("No bookings present.");
                }
                return bookings;
            }
            catch (NotPresentException ex)
            {
                throw new BookingRepositoryException("Error occurred while retrieving bookings. " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new BookingRepositoryException("Error occurred while retrieving bookings.", ex);
            }
        }

        public async Task<Booking> GetByKey(int key)
        {
            try
            {
                var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.BookingId == key);
                if (booking == null)
                    throw new NotPresentException("No such booking is present.");
                return booking;
            }
            catch (NotPresentException ex)
            {
                throw new BookingRepositoryException("Error occurred while retrieving booking. " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new BookingRepositoryException("Error occurred while retrieving booking.", ex);
            }
        }

        public async Task<Booking> Update(Booking item)
        {
            try
            {
                var booking = await GetByKey(item.BookingId);
                _context.Update(item);
                await _context.SaveChangesAsync(true);
                return booking;
            }
            catch (NotPresentException ex)
            {
                throw new BookingRepositoryException("Error occurred while updating booking. Booking not found. " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new BookingRepositoryException("Error occurred while updating booking.", ex);
            }
        }
    }
}
