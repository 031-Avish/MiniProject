using FlightBookingSystemAPI.Contexts;
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
    public class BookingRepository : IRepository<int, Booking>
    {
        private readonly FlightBookingContext _context;
        private readonly ILogger<BookingRepository> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookingRepository"/> class.
        /// </summary>
        /// <param name="context">The database context to be used by the repository.</param>
        /// <param name="logger">The logger to log messages.</param>
        #region constructor
        public BookingRepository(FlightBookingContext context, ILogger<BookingRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        #endregion

        /// <summary>
        /// Adds a new booking to the database.
        /// </summary>
        /// <param name="item">The booking object that is to be added.</param>
        /// <returns>The added booking object.</returns>
        /// <exception cref="BookingRepositoryException">Thrown when an error occurs while adding the booking.</exception>
        #region Add Booking
        public async Task<Booking> Add(Booking item)
        {
            try
            {
                _logger.LogInformation("Adding new booking.");
                _context.Add(item);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Booking added successfully.");
                return item;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding booking.");
                throw new BookingRepositoryException("Error occurred while adding booking." + ex.Message, ex);
            }
        }
        #endregion

        /// <summary>
        /// Deletes a booking from the database by its key.
        /// </summary>
        /// <param name="key">The key of the booking to be deleted.</param>
        /// <returns>The deleted booking object.</returns>
        /// <exception cref="BookingRepositoryException">Thrown when an error occurs while deleting the booking.</exception>
        #region Delete Booking By Key
        public async Task<Booking> DeleteByKey(int key)
        {
            try
            {
                _logger.LogInformation($"Deleting booking with key: {key}");
                var booking = await GetByKey(key);
                _context.Remove(booking);
                await _context.SaveChangesAsync(true);
                _logger.LogInformation("Booking deleted successfully.");
                return booking;
            }
            catch (NotPresentException ex)
            {
                _logger.LogError(ex, "Error occurred while deleting booking. Booking not found.");
                throw new BookingRepositoryException("Booking not found. " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting booking.");
                throw new BookingRepositoryException("Error occurred while deleting booking." + ex.Message, ex);
            }
        }
        #endregion

        /// <summary>
        /// Retrieves all bookings from the database.
        /// </summary>
        /// <returns>A collection of bookings.</returns>
        /// <exception cref="BookingRepositoryException">Thrown when an error occurs while retrieving the bookings.</exception>
        #region Get All Bookings
        public async Task<IEnumerable<Booking>> GetAll()
        {
            try
            {
                _logger.LogInformation("Retrieving all bookings.");
                var bookings = await _context.Bookings
                    .Include(b => b.FlightDetails) // Include related FlightDetails entity
                    .ToListAsync();

                if (bookings.Count <= 0)
                {
                    _logger.LogWarning("No bookings present.");
                    throw new NotPresentException("No bookings present.");
                }

                _logger.LogInformation("Bookings retrieved successfully.");
                return bookings;
            }
            catch (NotPresentException ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving bookings.");
                throw new BookingRepositoryException("Error occurred while retrieving bookings. " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving bookings.");
                throw new BookingRepositoryException("Error occurred while retrieving bookings." + ex.Message, ex);
            }
        }
        #endregion

        /// <summary>
        /// Retrieves a booking from the database by its key.
        /// </summary>
        /// <param name="key">The key of the booking to be retrieved.</param>
        /// <returns>The retrieved booking object.</returns>
        /// <exception cref="BookingRepositoryException">Thrown when an error occurs while retrieving the booking by given key.</exception>
        #region Get Booking By Key
        public async Task<Booking> GetByKey(int key)
        {
            try
            {
                _logger.LogInformation($"Retrieving booking with key: {key}");
                var booking = await _context.Bookings
                    .Include(b => b.FlightDetails) // Include related FlightDetails entity
                    .FirstOrDefaultAsync(b => b.BookingId == key);

                if (booking == null)
                {
                    _logger.LogWarning("No such booking is present.");
                    throw new NotPresentException("No such booking is present.");
                }

                _logger.LogInformation("Booking retrieved successfully.");
                return booking;
            }
            catch (NotPresentException ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving booking.");
                throw new BookingRepositoryException("Error occurred while retrieving booking. " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving booking.");
                throw new BookingRepositoryException("Error occurred while retrieving booking." + ex.Message, ex);
            }
        }
        #endregion

        /// <summary>
        /// Updates an existing booking in the database.
        /// </summary>
        /// <param name="item">The booking object to be updated.</param>
        /// <returns>The updated booking object.</returns>
        /// <exception cref="BookingRepositoryException">Thrown when an error occurs while updating the booking.</exception>
        #region Update Booking
        public async Task<Booking> Update(Booking item)
        {
            try
            {
                _logger.LogInformation($"Updating booking with ID: {item.BookingId}");
                var booking = await GetByKey(item.BookingId);
                _context.Entry(booking).State = EntityState.Detached;
                _context.Update(item);
                await _context.SaveChangesAsync(true);
                _logger.LogInformation("Booking updated successfully.");
                return item;
            }
            catch (NotPresentException ex)
            {
                _logger.LogError(ex, "Error occurred while updating booking. Booking not found.");
                throw new BookingRepositoryException("Booking not found. " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating booking.");
                throw new BookingRepositoryException("Error occurred while updating booking." + ex.Message, ex);
            }
        }
        #endregion
    }
}
