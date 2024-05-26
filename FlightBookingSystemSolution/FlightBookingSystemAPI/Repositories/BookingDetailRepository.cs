using FlightBookingSystemAPI.Contexts;
using FlightBookingSystemAPI.Exceptions;
using FlightBookingSystemAPI.Exceptions.RepositoryException;
using FlightBookingSystemAPI.Interfaces;
using FlightBookingSystemAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlightBookingSystemAPI.Repositories
{
    public class BookingDetailRepository : IRepository<int, BookingDetail>
    {
        private readonly FlightBookingContext _context;

        public BookingDetailRepository(FlightBookingContext context)
        {
            _context = context;
        }

        public async Task<BookingDetail> Add(BookingDetail item)
        {
            try
            {
                _context.Add(item);
                await _context.SaveChangesAsync();
                return item;
            }
            catch (Exception ex)
            {
                throw new BookingDetailRepositoryException("Error occurred while adding booking detail.", ex);
            }
        }

        public async Task<BookingDetail> DeleteByKey(int key)
        {
            try
            {
                var bookingDetail = await GetByKey(key);
                _context.Remove(bookingDetail);
                await _context.SaveChangesAsync(true);
                return bookingDetail;
            }
            catch (NotPresentException ex)
            {
                throw new BookingDetailRepositoryException("Error occurred while deleting booking detail. Booking detail not found. " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new BookingDetailRepositoryException("Error occurred while deleting booking detail.", ex);
            }
        }

        public async Task<IEnumerable<BookingDetail>> GetAll()
        {
            try
            {
                var bookingDetails = await _context.BookingDetails.ToListAsync();
                if (bookingDetails.Count <= 0)
                {
                    throw new NotPresentException("No booking details present.");
                }
                return bookingDetails;
            }
            catch (NotPresentException ex)
            {
                throw new BookingDetailRepositoryException("Error occurred while retrieving booking details. " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new BookingDetailRepositoryException("Error occurred while retrieving booking details.", ex);
            }
        }

        public async Task<BookingDetail> GetByKey(int key)
        {
            try
            {
                var bookingDetail = await _context.BookingDetails.FirstOrDefaultAsync(bd => bd.BookingDetailId == key);
                if (bookingDetail == null)
                    throw new NotPresentException("No such booking detail is present.");
                return bookingDetail;
            }
            catch (NotPresentException ex)
            {
                throw new BookingDetailRepositoryException("Error occurred while retrieving booking detail. " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new BookingDetailRepositoryException("Error occurred while retrieving booking detail.", ex);
            }
        }

        public async Task<BookingDetail> Update(BookingDetail item)
        {
            try
            {
                var bookingDetail = await GetByKey(item.BookingDetailId);
                _context.Update(item);
                await _context.SaveChangesAsync(true);
                return bookingDetail;
            }
            catch (NotPresentException ex)
            {
                throw new BookingDetailRepositoryException("Error occurred while updating booking detail. Booking detail not found. " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new BookingDetailRepositoryException("Error occurred while updating booking detail.", ex);
            }
        }
    }
}
