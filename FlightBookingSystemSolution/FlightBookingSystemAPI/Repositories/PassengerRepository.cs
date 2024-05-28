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
    public class PassengerRepository : IRepository<int, Passenger>
    {
        private readonly FlightBookingContext _context;
        private readonly ILogger<PassengerRepository> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PassengerRepository"/> class.
        /// </summary>
        /// <param name="context">The database context to be used by the repository.</param>
        /// <param name="logger">The logger to log messages.</param>
        #region constructor
        public PassengerRepository(FlightBookingContext context, ILogger<PassengerRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        #endregion

        /// <summary>
        /// Adds a new passenger to the database.
        /// </summary>
        /// <param name="item">The passenger object that is to be added.</param>
        /// <returns>The added passenger object.</returns>
        /// <exception cref="PassengerRepositoryException">Thrown when an error occurs while adding the passenger.</exception>
        #region Add Passenger
        public async Task<Passenger> Add(Passenger item)
        {
            try
            {
                _logger.LogInformation("Adding new passenger.");
                _context.Add(item);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Passenger added successfully.");
                return item;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding passenger.");
                throw new PassengerRepositoryException("Error occurred while adding passenger.", ex);
            }
        }
        #endregion

        /// <summary>
        /// Deletes a passenger from the database by its key.
        /// </summary>
        /// <param name="key">The key of the passenger to be deleted.</param>
        /// <returns>The deleted passenger object.</returns>
        /// <exception cref="PassengerRepositoryException">Thrown when an error occurs while deleting the passenger.</exception>
        #region Delete Passenger By Key
        public async Task<Passenger> DeleteByKey(int key)
        {
            try
            {
                _logger.LogInformation($"Deleting passenger with key: {key}");
                var passenger = await GetByKey(key);
                _context.Remove(passenger);
                await _context.SaveChangesAsync(true);
                _logger.LogInformation("Passenger deleted successfully.");
                return passenger;
            }
            catch (NotPresentException ex)
            {
                _logger.LogError(ex, "Error occurred while deleting passenger. Passenger not found.");
                throw new PassengerRepositoryException("Error occurred while deleting passenger. Passenger not found. " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting passenger.");
                throw new PassengerRepositoryException("Error occurred while deleting passenger. " + ex.Message, ex);
            }
        }
        #endregion

        /// <summary>
        /// Retrieves all passengers from the database.
        /// </summary>
        /// <returns>A collection of passengers.</returns>
        /// <exception cref="PassengerRepositoryException">Thrown when an error occurs while retrieving the passengers.</exception>
        #region Get All Passengers
        public async Task<IEnumerable<Passenger>> GetAll()
        {
            try
            {
                _logger.LogInformation("Retrieving all passengers.");
                var passengers = await _context.Passengers.ToListAsync();
                if (passengers.Count <= 0)
                {
                    _logger.LogWarning("No passengers present.");
                    throw new NotPresentException("No passengers present.");
                }
                _logger.LogInformation("Passengers retrieved successfully.");
                return passengers;
            }
            catch (NotPresentException ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving passengers.");
                throw new PassengerRepositoryException("Error occurred while retrieving passengers. " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving passengers.");
                throw new PassengerRepositoryException("Error occurred while retrieving passengers. " + ex.Message, ex);
            }
        }
        #endregion

        /// <summary>
        /// Retrieves a passenger from the database by its key.
        /// </summary>
        /// <param name="key">The key of the passenger to be retrieved.</param>
        /// <returns>The retrieved passenger object.</returns>
        /// <exception cref="PassengerRepositoryException">Thrown when an error occurs while retrieving the passenger by given key.</exception>
        #region Get Passenger By Key
        public async Task<Passenger> GetByKey(int key)
        {
            try
            {
                _logger.LogInformation($"Retrieving passenger with key: {key}");
                var passenger = await _context.Passengers.FirstOrDefaultAsync(p => p.PassengerId == key);
                if (passenger == null)
                {
                    _logger.LogWarning("No such passenger is present.");
                    throw new NotPresentException("No such passenger is present.");
                }
                _logger.LogInformation("Passenger retrieved successfully.");
                return passenger;
            }
            catch (NotPresentException ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving passenger.");
                throw new PassengerRepositoryException("Error occurred while retrieving passenger. " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving passenger.");
                throw new PassengerRepositoryException("Error occurred while retrieving passenger. " + ex.Message, ex);
            }
        }
        #endregion

        /// <summary>
        /// Updates an existing passenger in the database.
        /// </summary>
        /// <param name="item">The passenger object to be updated.</param>
        /// <returns>The updated passenger object.</returns>
        /// <exception cref="PassengerRepositoryException">Thrown when an error occurs while updating the passenger.</exception>
        #region Update Passenger
        public async Task<Passenger> Update(Passenger item)
        {
            try
            {
                _logger.LogInformation($"Updating passenger with ID: {item.PassengerId}");
                var passenger = await GetByKey(item.PassengerId);
                _context.Entry(passenger).State = EntityState.Detached;
                _context.Update(item);
                await _context.SaveChangesAsync(true);
                _logger.LogInformation("Passenger updated successfully.");
                return item;
            }
            catch (NotPresentException ex)
            {
                _logger.LogError(ex, "Error occurred while updating passenger. Passenger not found.");
                throw new PassengerRepositoryException("Error occurred while updating passenger. Passenger not found. " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating passenger.");
                throw new PassengerRepositoryException("Error occurred while updating passenger. " + ex.Message, ex);
            }
        }
        #endregion
    }
}
