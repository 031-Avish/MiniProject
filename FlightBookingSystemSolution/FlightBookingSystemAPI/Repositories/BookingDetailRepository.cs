using FlightBookingSystemAPI.Contexts;
using FlightBookingSystemAPI.Exceptions;
using FlightBookingSystemAPI.Exceptions.RepositoryException;
using FlightBookingSystemAPI.Interfaces;
using FlightBookingSystemAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlightBookingSystemAPI.Repositories
{
    public class BookingDetailRepository : IRepository<int, BookingDetail>
    {
        private readonly FlightBookingContext _context;
        private readonly ILogger<BookingDetailRepository> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookingDetailRepository"/> class.
        /// </summary>
        /// <param name="context">The database context to be used by the repository.</param>
        /// <param name="logger">The logger to log messages.</param>
        #region constructor
        public BookingDetailRepository(FlightBookingContext context, ILogger<BookingDetailRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        #endregion

        /// <summary>
        /// Adds a new booking detail to the database.
        /// </summary>
        /// <param name="item">The booking detail object that is to be added.</param>
        /// <returns>The added booking detail object.</returns>
        /// <exception cref="BookingDetailRepositoryException">Thrown when an error occurs while adding the booking detail.</exception>
        #region Add Booking Detail
        public async Task<BookingDetail> Add(BookingDetail item)
        {
            try
            {
                _logger.LogInformation("Adding new booking detail.");
                _context.Add(item);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Booking detail added successfully.");
                return item;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding booking detail.");
                throw new BookingDetailRepositoryException("Error occurred while adding booking detail." + ex.Message, ex);
            }
        }
        #endregion

        /// <summary>
        /// Deletes a booking detail from the database by its key.
        /// </summary>
        /// <param name="key">The key of the booking detail to be deleted.</param>
        /// <returns>The deleted booking detail object.</returns>
        /// <exception cref="BookingDetailRepositoryException">Thrown when an error occurs while deleting the booking detail.</exception>
        #region Delete Booking Detail By Key
        public async Task<BookingDetail> DeleteByKey(int key)
        {
            try
            {
                _logger.LogInformation($"Deleting booking detail with key: {key}");
                var bookingDetail = await GetByKey(key);
                _context.Remove(bookingDetail);
                await _context.SaveChangesAsync(true);
                _logger.LogInformation("Booking detail deleted successfully.");
                return bookingDetail;
            }
            catch (NotPresentException ex)
            {
                _logger.LogError(ex, "Error occurred while deleting booking detail. Booking detail not found.");
                throw new BookingDetailRepositoryException("Booking detail not found. " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting booking detail.");
                throw new BookingDetailRepositoryException("Error occurred while deleting booking detail." + ex.Message, ex);
            }
        }
        #endregion

        /// <summary>
        /// Retrieves all booking details from the database.
        /// </summary>
        /// <returns>A collection of booking details.</returns>
        /// <exception cref="BookingDetailRepositoryException">Thrown when an error occurs while retrieving the booking details.</exception>
        #region Get All Booking Details
        public async Task<IEnumerable<BookingDetail>> GetAll()
        {
            try
            {
                _logger.LogInformation("Retrieving all booking details.");
                var bookingDetails = await _context.BookingDetails
                    .Include(bd => bd.PassengerDetail) // Include related Passenger entity
                    .Include(bd => bd.Bookings) // Include related Booking entity
                    .ToListAsync();

                if (bookingDetails.Count <= 0)
                {
                    _logger.LogWarning("No booking details present.");
                    throw new NotPresentException("No booking details present.");
                }

                _logger.LogInformation("Booking details retrieved successfully.");
                return bookingDetails;
            }
            catch (NotPresentException ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving booking details.");
                throw new BookingDetailRepositoryException("Error occurred while retrieving booking details. " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving booking details.");
                throw new BookingDetailRepositoryException("Error occurred while retrieving booking details." + ex.Message, ex);
            }
        }
        #endregion

        /// <summary>
        /// Retrieves a booking detail from the database by its key.
        /// </summary>
        /// <param name="key">The key of the booking detail to be retrieved.</param>
        /// <returns>The retrieved booking detail object.</returns>
        /// <exception cref="BookingDetailRepositoryException">Thrown when an error occurs while retrieving the booking detail by given key.</exception>
        #region Get Booking Detail By Key
        public async Task<BookingDetail> GetByKey(int key)
        {
            try
            {
                _logger.LogInformation($"Retrieving booking detail with key: {key}");
                var bookingDetail = await _context.BookingDetails
                .Include(bd => bd.PassengerDetail) // Include related Passenger entity
                .Include(bd => bd.Bookings) // Include related Booking entity
                .FirstOrDefaultAsync(bd => bd.BookingDetailId == key);
                if (bookingDetail == null)
                {
                    _logger.LogWarning("No such booking detail is present.");
                    throw new NotPresentException("No such booking detail is present.");
                }

                _logger.LogInformation("Booking detail retrieved successfully.");
                return bookingDetail;
            }
            catch (NotPresentException ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving booking detail.");
                throw new BookingDetailRepositoryException("Error occurred while retrieving booking detail. " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving booking detail.");
                throw new BookingDetailRepositoryException("Error occurred while retrieving booking detail." + ex.Message, ex);
            }
        }
        #endregion

        /// <summary>
        /// Updates an existing booking detail in the database.
        /// </summary>
        /// <param name="item">The booking detail object to be updated.</param>
        /// <returns>The updated booking detail Object.</returns>
        /// <exception cref="BookingDetailRepositoryException">Thrown when an error occurs while updating the booking detail.</exception>
        #region Update Booking Detail
        public async Task<BookingDetail> Update(BookingDetail item)
        {
            try
            {
                _logger.LogInformation($"Updating booking detail with key: {item.BookingDetailId}");
                var bookingDetail = await GetByKey(item.BookingDetailId);
                _context.Entry(bookingDetail).State = EntityState.Detached;
                _context.Update(item);
                await _context.SaveChangesAsync(true);
                _logger.LogInformation("Booking detail updated successfully.");
                return item;
            }
            catch (NotPresentException ex)
            {
                _logger.LogError(ex, "Error occurred while updating booking detail. Booking detail not found.");
                throw new BookingDetailRepositoryException("Booking detail not found. " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating booking detail.");
                throw new BookingDetailRepositoryException("Error occurred while updating booking detail." + ex.Message, ex);
            }
        }
        #endregion
    }
}
