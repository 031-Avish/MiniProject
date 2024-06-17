using FlightBookingSystemAPI.Contexts;
using FlightBookingSystemAPI.Exceptions.RepositoryException;
using FlightBookingSystemAPI.Interfaces;
using FlightBookingSystemAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightBookingSystemAPI.Repositories
{
    public class RouteInfoRepository : IRepository<int, RouteInfo>
    {
        private readonly FlightBookingContext _context;
        private readonly ILogger<RouteInfoRepository> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RouteInfoRepository"/> class.
        /// </summary>
        /// <param name="context">The database context to be used by the repository.</param>
        /// <param name="logger">The logger to log messages.</param>
        #region constructor
        public RouteInfoRepository(FlightBookingContext context, ILogger<RouteInfoRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        #endregion

        /// <summary>
        /// Adds a new route info to the database.
        /// </summary>
        /// <param name="item">The route info object that is to be added.</param>
        /// <returns>The added route info object.</returns>
        /// <exception cref="RouteInfoRepositoryException">Thrown when an error occurs while adding the route info.</exception>
        #region Add RouteInfo
        public async Task<RouteInfo> Add(RouteInfo item)
        {
            try
            {
                _logger.LogInformation("Adding new route info.");
                _context.Add(item);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Route info added successfully.");
                return item;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding route info.");
                throw new RouteInfoRepositoryException("Error occurred while adding route info.", ex);
            }
        }
        #endregion

        /// <summary>
        /// Deletes a route info from the database by its key.
        /// </summary>
        /// <param name="key">The key of the route info to be deleted.</param>
        /// <returns>The deleted route info object.</returns>
        /// <exception cref="RouteInfoRepositoryException">Thrown when an error occurs while deleting the route info.</exception>
        #region Delete RouteInfo By Key
        public async Task<RouteInfo> DeleteByKey(int key)
        {
            try
            {
                _logger.LogInformation($"Deleting route info with key: {key}");
                var routeInfo = await GetByKey(key);
                _context.Remove(routeInfo);
                await _context.SaveChangesAsync(true);
                _logger.LogInformation("Route info deleted successfully.");
                return routeInfo;
            }
            
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting route info.");
                throw new RouteInfoRepositoryException("Error occurred while deleting route info. " + ex.Message, ex);
            }
        }
        #endregion

        /// <summary>
        /// Retrieves all route infos from the database.
        /// </summary>
        /// <returns>A collection of route infos.</returns>
        /// <exception cref="RouteInfoRepositoryException">Thrown when an error occurs while retrieving the route infos.</exception>
        #region Get All RouteInfos
        public async Task<IEnumerable<RouteInfo>> GetAll()
        {
            try
            {
                _logger.LogInformation("Retrieving all route infos.");
                var routes = await _context.RouteInfos.ToListAsync();
                if (routes.Count <= 0)
                {
                    _logger.LogWarning("No routes present.");
                    throw new NotPresentException("No routes present.");
                }
                _logger.LogInformation("Route infos retrieved successfully.");
                return routes;
            }
            catch (NotPresentException ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving route infos.");
                throw new RouteInfoRepositoryException("Error occurred while retrieving routes. " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving route infos.");
                throw new RouteInfoRepositoryException("Error occurred while retrieving routes. " + ex.Message, ex);
            }
        }
        #endregion

        /// <summary>
        /// Retrieves a route info from the database by its key.
        /// </summary>
        /// <param name="key">The key of the route info to be retrieved.</param>
        /// <returns>The retrieved route info object.</returns>
        /// <exception cref="RouteInfoRepositoryException">Thrown when an error occurs while retrieving the route info by given key.</exception>
        #region Get RouteInfo By Key
        public async Task<RouteInfo> GetByKey(int key)
        {
            try
            {
                _logger.LogInformation($"Retrieving route info with key: {key}");
                var routeInfo = await _context.RouteInfos.FirstOrDefaultAsync(r => r.RouteId == key);
                if (routeInfo == null)
                {
                    _logger.LogWarning("No such route is present.");
                    throw new NotPresentException("No such route is present.");
                }
                _logger.LogInformation("Route info retrieved successfully.");
                return routeInfo;
            }
            catch (NotPresentException ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving route info.");
                throw new RouteInfoRepositoryException("Error occurred while retrieving route info. " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving route info.");
                throw new RouteInfoRepositoryException("Error occurred while retrieving route info. " + ex.Message, ex);
            }
        }
        #endregion

        /// <summary>
        /// Updates an existing route info in the database.
        /// </summary>
        /// <param name="item">The route info object to be updated.</param>
        /// <returns>The updated route info object.</returns>
        /// <exception cref="RouteInfoRepositoryException">Thrown when an error occurs while updating the route info.</exception>
        #region Update RouteInfo
        public async Task<RouteInfo> Update(RouteInfo item)
        {
            try
            {
                _logger.LogInformation($"Updating route info with ID: {item.RouteId}");
                var routeInfo = await GetByKey(item.RouteId);
                _context.Entry(routeInfo).State = EntityState.Detached;
                _context.Update(item);
                await _context.SaveChangesAsync(true);
                _logger.LogInformation("Route info updated successfully.");
                return item;
            }
            
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating route info.");
                throw new RouteInfoRepositoryException("Error occurred while updating route info. " + ex.Message, ex);
            }
        }
        #endregion
    }
}
