using FlightBookingSystemAPI.Contexts;
using FlightBookingSystemAPI.Exceptions.RepositoryException;
using FlightBookingSystemAPI.Interfaces;
using FlightBookingSystemAPI.Models;
using Microsoft.EntityFrameworkCore;


namespace FlightBookingSystemAPI.Repositories
{
    public class FlightRepository : IRepository<int, Flight>
    {
        private readonly FlightBookingContext _context;
        private readonly ILogger<FlightRepository> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FlightRepository"/> class.
        /// </summary>
        /// <param name="context">The database context to be used by the repository.</param>
        /// <param name="logger">The logger to log messages.</param>
        #region constructor
        public FlightRepository(FlightBookingContext context, ILogger<FlightRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        #endregion

        /// <summary>
        /// Adds a new flight to the database.
        /// </summary>
        /// <param name="item">The flight object that is to be added.</param>
        /// <returns>The added flight object.</returns>
        /// <exception cref="FlightRepositoryException">Thrown when an error occurs while adding the flight.</exception>
        #region Add Flight
        public async Task<Flight> Add(Flight item)
        {
            try
            {
                _logger.LogInformation("Adding new flight.");
                _context.Add(item);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Flight added successfully.");
                return item;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding flight.");
                throw new FlightRepositoryException("Error occurred while adding flight.", ex);
            }
        }
        #endregion

        /// <summary>
        /// Deletes a flight from the database by its key.
        /// </summary>
        /// <param name="key">The key of the flight to be deleted.</param>
        /// <returns>The deleted flight object.</returns>
        /// <exception cref="FlightRepositoryException">Thrown when an error occurs while deleting the flight.</exception>
        #region Delete Flight By Key
        public async Task<Flight> DeleteByKey(int key)
        {
            try
            {
                _logger.LogInformation($"Deleting flight with key: {key}");
                var flight = await GetByKey(key);
                _context.Remove(flight);
                await _context.SaveChangesAsync(true);
                _logger.LogInformation("Flight deleted successfully.");
                return flight;
            }
            catch (NotPresentException ex)
            {
                _logger.LogError(ex, "Error occurred while deleting flight. Flight not found.");
                throw new FlightRepositoryException("Error occurred while deleting flight. Flight not found. " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting flight.");
                throw new FlightRepositoryException("Error occurred while deleting flight." + ex.Message, ex);
            }
        }
        #endregion

        /// <summary>
        /// Retrieves all flights from the database.
        /// </summary>
        /// <returns>A collection of flights.</returns>
        /// <exception cref="FlightRepositoryException">Thrown when an error occurs while retrieving the flights.</exception>
        #region Get All Flights
        public async Task<IEnumerable<Flight>> GetAll()
        {
            try
            {
                _logger.LogInformation("Retrieving all flights.");
                var flights = await _context.Flights.ToListAsync();
                if (flights.Count <= 0)
                {
                    _logger.LogWarning("No flights present.");
                    throw new NotPresentException("No flights present.");
                }
                _logger.LogInformation("Flights retrieved successfully.");
                return flights;
            }
            catch (NotPresentException ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving flights.");
                throw new FlightRepositoryException("Error occurred while retrieving flights. " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving flights.");
                throw new FlightRepositoryException("Error occurred while retrieving flights." + ex.Message, ex);
            }
        }
        #endregion

        /// <summary>
        /// Retrieves a flight from the database by its key.
        /// </summary>
        /// <param name="key">The key of the flight to be retrieved.</param>
        /// <returns>The retrieved flight object.</returns>
        /// <exception cref="FlightRepositoryException">Thrown when an error occurs while retrieving the flight by given key.</exception>
        #region Get Flight By Key
        public async Task<Flight> GetByKey(int key)
        {
            try
            {
                _logger.LogInformation($"Retrieving flight with key: {key}");
                var flight = await _context.Flights.FirstOrDefaultAsync(f => f.FlightId == key);
                if (flight == null)
                {
                    _logger.LogWarning("No such flight is present.");
                    throw new NotPresentException("No such flight is present.");
                }
                _logger.LogInformation("Flight retrieved successfully.");
                return flight;
            }
            catch (NotPresentException ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving flight.");
                throw new FlightRepositoryException("Error occurred while retrieving flight. " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving flight.");
                throw new FlightRepositoryException("Error occurred while retrieving flight. " + ex.Message, ex);
            }
        }
        #endregion

        /// <summary>
        /// Updates an existing flight in the database.
        /// </summary>
        /// <param name="item">The flight object to be updated.</param>
        /// <returns>The updated flight object.</returns>
        /// <exception cref="FlightRepositoryException">Thrown when an error occurs while updating the flight.</exception>
        #region Update Flight
        public async Task<Flight> Update(Flight item)
        {
            try
            {
                _logger.LogInformation($"Updating flight with ID: {item.FlightId}");
                var flight = await GetByKey(item.FlightId);
                _context.Entry(flight).State = EntityState.Detached;
                _context.Update(item);
                await _context.SaveChangesAsync(true);
                _logger.LogInformation("Flight updated successfully.");
                return item;
            }
            catch (NotPresentException ex)
            {
                _logger.LogError(ex, "Error occurred while updating flight. Flight not found.");
                throw new FlightRepositoryException("Error occurred while updating flight. Flight not found. " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating flight.");
                throw new FlightRepositoryException("Error occurred while updating flight." + ex.Message, ex);
            }
        }
        #endregion
    }
}
