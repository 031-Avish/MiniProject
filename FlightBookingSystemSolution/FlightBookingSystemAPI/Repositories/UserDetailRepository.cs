using FlightBookingSystemAPI.Contexts;
using FlightBookingSystemAPI.Exceptions.RepositoryException;
using FlightBookingSystemAPI.Interfaces;
using FlightBookingSystemAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightBookingSystemAPI.Repositories
{
    /// <summary>
    /// Repository for managing user details.
    /// </summary>
    public class UserDetailRepository : IRepository<int, UserDetail>
    {
        private readonly FlightBookingContext _context;
        private readonly ILogger<UserDetailRepository> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserDetailRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger.</param>
        public UserDetailRepository(FlightBookingContext context, ILogger<UserDetailRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region Add User Detail
        /// <summary>
        /// Adds a new user detail to the database.
        /// </summary>
        /// <param name="item">The user detail to add.</param>
        /// <returns>The added user detail.</returns>
        public async Task<UserDetail> Add(UserDetail item)
        {
            try
            {
                _logger.LogInformation("Adding user detail...");

                // Check if email already exists
                var existingUserByEmail = await _context.UserDetails.FirstOrDefaultAsync(u => u.Email == item.Email);
                if (existingUserByEmail != null)
                {
                    _logger.LogError("A user with the same email already exists.");
                    throw new DuplicateUserException("A user with the same email already exists.");
                }

                _context.Add(item);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User detail added successfully.");
                return item;
            }
            catch (DuplicateUserException ex)
            {
                _logger.LogError(ex, "Error occurred while adding user detail: " + ex.Message);
                throw new UserDetailRepositoryException("Error: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding user detail.");
                throw new UserDetailRepositoryException("Error occurred while adding user detail." + ex.Message, ex);
            }
        }
        #endregion

        #region Delete User Detail By Key
        /// <summary>
        /// Deletes a user detail from the database by its key.
        /// </summary>
        /// <param name="key">The key of the user detail to delete.</param>
        /// <returns>The deleted user detail.</returns>
        public async Task<UserDetail> DeleteByKey(int key)
        {
            try
            {
                _logger.LogInformation($"Deleting user detail with key: {key}");
                var userDetail = await GetByKey(key);
                _context.Remove(userDetail);
                await _context.SaveChangesAsync(true);
                _logger.LogInformation("User detail deleted successfully.");
                return userDetail;
            }
            catch (NotPresentException ex)
            {
                _logger.LogError(ex, "Error occurred while deleting user detail: " + ex.Message);
                throw new UserDetailRepositoryException("Error occurred while deleting user detail: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting user detail.");
                throw new UserDetailRepositoryException("Error occurred while deleting user detail." + ex.Message, ex);
            }
        }
        #endregion

        #region Get All User Details
        /// <summary>
        /// Retrieves all user details from the database.
        /// </summary>
        /// <returns>A list of all user details.</returns>
        public async Task<IEnumerable<UserDetail>> GetAll()
        {
            try
            {
                _logger.LogInformation("Retrieving all user details...");
                var userDetails = await _context.UserDetails.ToListAsync();
                if (userDetails.Count <= 0)
                {
                    _logger.LogWarning("There are no user details present.");
                    throw new NotPresentException("There are no user details present.");
                }
                _logger.LogInformation("User details retrieved successfully.");
                return userDetails;
            }
            catch (NotPresentException ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving user details.");
                throw new UserDetailRepositoryException("Error occurred while retrieving user details: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving user details.");
                throw new UserDetailRepositoryException("Error occurred while retrieving user details." + ex.Message, ex);
            }
        }
        #endregion

        #region Get User Detail By Key
        /// <summary>
        /// Retrieves a user detail from the database by its key.
        /// </summary>
        /// <param name="key">The key of the user detail to retrieve.</param>
        /// <returns>The retrieved user detail.</returns>
        public async Task<UserDetail> GetByKey(int key)
        {
            try
            {
                _logger.LogInformation($"Retrieving user detail with key: {key}");
                var userDetail = await _context.UserDetails.FirstOrDefaultAsync(u => u.UserId == key);
                if (userDetail == null)
                {
                    _logger.LogWarning("No such user detail is present.");
                    throw new NotPresentException("No such user detail is present.");
                }
                _logger.LogInformation("User detail retrieved successfully.");
                return userDetail;
            }
            catch (NotPresentException ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving user detail.");
                throw new UserDetailRepositoryException("Error occurred while retrieving user detail: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving user detail.");
                throw new UserDetailRepositoryException("Error occurred while retrieving user detail." + ex.Message, ex);
            }
        }
        #endregion

        #region Update User Detail
        /// <summary>
        /// Updates a user detail in the database.
        /// </summary>
        /// <param name="item">The user detail to update.</param>
        /// <returns>The updated user detail.</returns>
        public async Task<UserDetail> Update(UserDetail item)
        {
            try
            {
                _logger.LogInformation($"Updating user detail with key: {item.UserId}");
                var userDetail = await GetByKey(item.UserId);
                _context.Entry(userDetail).State = EntityState.Detached;
                _context.Update(item);
                await _context.SaveChangesAsync(true);
                _logger.LogInformation("User detail updated successfully.");
                return item;
            }
            catch (NotPresentException ex)
            {
                _logger.LogError(ex, "Error occurred while updating user detail: " + ex.Message);
                throw new UserDetailRepositoryException("Error occurred while updating user detail. User detail not found: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating user detail.");
                throw new UserDetailRepositoryException("Error occurred while updating user detail." + ex.Message, ex);
            }
        }


        #endregion
    }
}
