using FlightBookingSystemAPI.Contexts;
using FlightBookingSystemAPI.Exceptions.RepositoryException;
using FlightBookingSystemAPI.Interfaces;
using FlightBookingSystemAPI.Models;
using Microsoft.EntityFrameworkCore;


namespace FlightBookingSystemAPI.Repositories
{
    public class PaymentRepository : IRepository<int, Payment>
    {
        private readonly FlightBookingContext _context;
        private readonly ILogger<PaymentRepository> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentRepository"/> class.
        /// </summary>
        /// <param name="context">The database context to be used by the repository.</param>
        /// <param name="logger">The logger to log messages.</param>
        #region constructor
        public PaymentRepository(FlightBookingContext context, ILogger<PaymentRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        #endregion

        /// <summary>
        /// Adds a new payment to the database.
        /// </summary>
        /// <param name="item">The payment object that is to be added.</param>
        /// <returns>The added payment object.</returns>
        /// <exception cref="PaymentRepositoryException">Thrown when an error occurs while adding the payment.</exception>
        #region Add Payment
        public async Task<Payment> Add(Payment item)
        {
            try
            {
                _logger.LogInformation("Adding new payment.");
                _context.Add(item);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Payment added successfully.");
                return item;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding payment.");
                throw new PaymentRepositoryException("Error occurred while adding payment.", ex);
            }
        }
        #endregion

        /// <summary>
        /// Deletes a payment from the database by its key.
        /// </summary>
        /// <param name="key">The key of the payment to be deleted.</param>
        /// <returns>The deleted payment object.</returns>
        /// <exception cref="PaymentRepositoryException">Thrown when an error occurs while deleting the payment.</exception>
        #region Delete Payment By Key
        public async Task<Payment> DeleteByKey(int key)
        {
            try
            {
                _logger.LogInformation($"Deleting payment with key: {key}");
                var payment = await GetByKey(key);
                _context.Remove(payment);
                await _context.SaveChangesAsync(true);
                _logger.LogInformation("Payment deleted successfully.");
                return payment;
            }
            
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting payment.");
                throw new PaymentRepositoryException("Error occurred while deleting payment. " + ex.Message, ex);
            }
        }
        #endregion

        /// <summary>
        /// Retrieves all payments from the database.
        /// </summary>
        /// <returns>A collection of payments.</returns>
        /// <exception cref="PaymentRepositoryException">Thrown when an error occurs while retrieving the payments.</exception>
        #region Get All Payments
        public async Task<IEnumerable<Payment>> GetAll()
        {
            try
            {
                _logger.LogInformation("Retrieving all payments.");
                var payments = await _context.Payments.ToListAsync();
                if (payments.Count <= 0)
                {
                    _logger.LogWarning("No payments present.");
                    throw new NotPresentException("No payments present.");
                }
                _logger.LogInformation("Payments retrieved successfully.");
                return payments;
            }
            catch (NotPresentException ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving payments.");
                throw new PaymentRepositoryException("Error occurred while retrieving payments. " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving payments.");
                throw new PaymentRepositoryException("Error occurred while retrieving payments. " + ex.Message, ex);
            }
        }
        #endregion

        /// <summary>
        /// Retrieves a payment from the database by its key.
        /// </summary>
        /// <param name="key">The key of the payment to be retrieved.</param>
        /// <returns>The retrieved payment object.</returns>
        /// <exception cref="PaymentRepositoryException">Thrown when an error occurs while retrieving the payment by given key.</exception>
        #region Get Payment By Key
        public async Task<Payment> GetByKey(int key)
        {
            try
            {
                _logger.LogInformation($"Retrieving payment with key: {key}");
                var payment = await _context.Payments.FirstOrDefaultAsync(p => p.PaymentId == key);
                if (payment == null)
                {
                    _logger.LogWarning("No such payment is present.");
                    throw new NotPresentException("No such payment is present.");
                }
                _logger.LogInformation("Payment retrieved successfully.");
                return payment;
            }
            catch (NotPresentException ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving payment.");
                throw new PaymentRepositoryException("Error occurred while retrieving payment. " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving payment.");
                throw new PaymentRepositoryException("Error occurred while retrieving payment. " + ex.Message, ex);
            }
        }
        #endregion

        /// <summary>
        /// Updates an existing payment in the database.
        /// </summary>
        /// <param name="item">The payment object to be updated.</param>
        /// <returns>The updated payment object.</returns>
        /// <exception cref="PaymentRepositoryException">Thrown when an error occurs while updating the payment.</exception>
        #region Update Payment
        public async Task<Payment> Update(Payment item)
        {
            try
            {
                _logger.LogInformation($"Updating payment with ID: {item.PaymentId}");
                var payment = await GetByKey(item.PaymentId);
                _context.Entry(payment).State = EntityState.Detached;
                _context.Update(item);
                await _context.SaveChangesAsync(true);
                _logger.LogInformation("Payment updated successfully.");
                return item;
            }
            
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating payment.");
                throw new PaymentRepositoryException("Error occurred while updating payment. " + ex.Message, ex);
            }
        }
        #endregion
    }
}
