using FlightBookingSystemAPI.Contexts;

using FlightBookingSystemAPI.Exceptions.RepositoryException;
using FlightBookingSystemAPI.Interfaces;
using FlightBookingSystemAPI.Models;
using Microsoft.EntityFrameworkCore;


namespace FlightBookingSystemAPI.Repositories
{
    public class ScheduleRepository : IRepository<int, Schedule>
    {
        private readonly FlightBookingContext _context;
        private readonly ILogger<ScheduleRepository> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleRepository"/> class.
        /// </summary>
        /// <param name="context">The database context to be used by the repository.</param>
        /// <param name="logger">The logger to log messages.</param>
        #region constructor
        public ScheduleRepository(FlightBookingContext context, ILogger<ScheduleRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        #endregion

        /// <summary>
        /// Adds a new schedule to the database.
        /// </summary>
        /// <param name="item">The schedule object that is to be added.</param>
        /// <returns>The added schedule object.</returns>
        /// <exception cref="ScheduleRepositoryException">Thrown when an error occurs while adding the schedule.</exception>
        #region Add Schedule
        public async Task<Schedule> Add(Schedule item)
        {
            try
            {
                _logger.LogInformation("Adding new schedule.");
                _context.Add(item);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Schedule added successfully.");
                return item;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding schedule.");
                throw new ScheduleRepositoryException("Error occurred while adding schedule. " + ex.Message, ex);
            }
        }
        #endregion

        /// <summary>
        /// Deletes a schedule from the database by its key.
        /// </summary>
        /// <param name="key">The key of the schedule to be deleted.</param>
        /// <returns>The deleted schedule object.</returns>
        /// <exception cref="ScheduleRepositoryException">Thrown when an error occurs while deleting the schedule.</exception>
        #region Delete Schedule By Key
        public async Task<Schedule> DeleteByKey(int key)
        {
            try
            {
                _logger.LogInformation($"Deleting schedule with key: {key}");
                var schedule = await GetByKey(key);
                _context.Remove(schedule);
                await _context.SaveChangesAsync(true);
                _logger.LogInformation("Schedule deleted successfully.");
                return schedule;
            }
            catch (NotPresentException ex)
            {
                _logger.LogError(ex, "Error occurred while deleting schedule. Schedule not found.");
                throw new ScheduleRepositoryException("Error occurred while deleting schedule. Schedule not found. " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting schedule.");
                throw new ScheduleRepositoryException("Error occurred while deleting schedule. " + ex.Message, ex);
            }
        }
        #endregion

        /// <summary>
        /// Retrieves all schedules from the database.
        /// </summary>
        /// <returns>A collection of schedules.</returns>
        /// <exception cref="ScheduleRepositoryException">Thrown when an error occurs while retrieving the schedules.</exception>
        #region Get All Schedules
        public async Task<IEnumerable<Schedule>> GetAll()
        {
            try
            {
                _logger.LogInformation("Retrieving all schedules.");
                var schedules = await _context.Schedules
                    .Include(s => s.RouteInfo)
                    .Include(s => s.FlightInfo)
                    .ToListAsync();

                if (schedules.Count <= 0)
                {
                    _logger.LogWarning("No schedules present.");
                    throw new NotPresentException("No schedules present.");
                }

                _logger.LogInformation("Schedules retrieved successfully.");
                return schedules;
            }
            catch (NotPresentException ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving schedules.");
                throw new ScheduleRepositoryException("Error occurred while retrieving schedules. " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving schedules.");
                throw new ScheduleRepositoryException("Error occurred while retrieving schedules. " + ex.Message, ex);
            }
        }
        #endregion

        /// <summary>
        /// Retrieves a schedule from the database by its key.
        /// </summary>
        /// <param name="key">The key of the schedule to be retrieved.</param>
        /// <returns>The retrieved schedule object.</returns>
        /// <exception cref="ScheduleRepositoryException">Thrown when an error occurs while retrieving the schedule by given Key.</exception>
        #region Get Schedule By Key
        public async Task<Schedule> GetByKey(int key)
        {
            try
            {
                _logger.LogInformation($"Retrieving schedule with key: {key}");
                var schedule = await _context.Schedules
                    .Include(s => s.RouteInfo)
                    .Include(s => s.FlightInfo)
                    .FirstOrDefaultAsync(s => s.ScheduleId == key);

                if (schedule == null)
                {
                    _logger.LogWarning("No such schedule is present.");
                    throw new NotPresentException("No such schedule is present.");
                }

                _logger.LogInformation("Schedule retrieved successfully.");
                return schedule;
            }
            catch (NotPresentException ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving schedule.");
                throw new ScheduleRepositoryException("Error occurred while retrieving schedule. " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving schedule.");
                throw new ScheduleRepositoryException("Error occurred while retrieving schedule. " + ex.Message, ex);
            }
        }
        #endregion

        /// <summary>
        /// Updates an existing schedule in the database.
        /// </summary>
        /// <param name="item">The schedule object to be updated.</param>
        /// <returns>The updated schedule Object.</returns>
        /// <exception cref="ScheduleRepositoryException">Thrown when an error occurs while updating the schedule.</exception>
        #region Update Schedule
        public async Task<Schedule> Update(Schedule item)
        {
            try
            {
                _logger.LogInformation($"Updating schedule with ID: {item.ScheduleId}");
                var schedule = await GetByKey(item.ScheduleId);
                _context.Entry(schedule).State = EntityState.Detached;
                _context.Update(item);
                await _context.SaveChangesAsync(true);
                _logger.LogInformation("Schedule updated successfully.");
                return item;
            }
            catch (NotPresentException ex)
            {
                _logger.LogError(ex, "Error occurred while updating schedule. Schedule not found.");
                throw new ScheduleRepositoryException("Error occurred while updating schedule. Schedule not found. " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating schedule.");
                throw new ScheduleRepositoryException("Error occurred while updating schedule. " + ex.Message, ex);
            }
        }
        #endregion
    }
}
