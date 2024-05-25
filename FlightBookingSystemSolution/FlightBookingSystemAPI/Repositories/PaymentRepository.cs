using FlightBookingSystemAPI.Contexts;
using FlightBookingSystemAPI.Exceptions;
using FlightBookingSystemAPI.Interfaces;
using FlightBookingSystemAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlightBookingSystemAPI.Repositories
{
    public class PaymentRepository : IRepository<int, Payment>
    {
        private readonly FlightBookingContext _context;

        public PaymentRepository(FlightBookingContext context)
        {
            _context = context;
        }

        public async Task<Payment> Add(Payment item)
        {
            try
            {
                _context.Add(item);
                await _context.SaveChangesAsync();
                return item;
            }
            catch (Exception ex)
            {
                throw new PaymentRepositoryException("Error occurred while adding payment.", ex);
            }
        }

        public async Task<Payment> DeleteByKey(int key)
        {
            try
            {
                var payment = await GetByKey(key);
                _context.Remove(payment);
                await _context.SaveChangesAsync(true);
                return payment;
            }
            catch (NotPresentException ex)
            {
                throw new PaymentRepositoryException("Error occurred while deleting payment. Payment not found. " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new PaymentRepositoryException("Error occurred while deleting payment.", ex);
            }
        }

        public async Task<IEnumerable<Payment>> GetAll()
        {
            try
            {
                var payments = await _context.Payments.ToListAsync();
                if (payments.Count <= 0)
                {
                    throw new NotPresentException("No payments present.");
                }
                return payments;
            }
            catch (NotPresentException ex)
            {
                throw new PaymentRepositoryException("Error occurred while retrieving payments. " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new PaymentRepositoryException("Error occurred while retrieving payments.", ex);
            }
        }

        public async Task<Payment> GetByKey(int key)
        {
            try
            {
                var payment = await _context.Payments.FirstOrDefaultAsync(p => p.PaymentId == key);
                if (payment == null)
                    throw new NotPresentException("No such payment is present.");
                return payment;
            }
            catch (NotPresentException ex)
            {
                throw new PaymentRepositoryException("Error occurred while retrieving payment. " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new PaymentRepositoryException("Error occurred while retrieving payment.", ex);
            }
        }

        public async Task<Payment> Update(Payment item)
        {
            try
            {
                var payment = await GetByKey(item.PaymentId);
                _context.Update(payment);
                await _context.SaveChangesAsync(true);
                return payment;
            }
            catch (NotPresentException ex)
            {
                throw new PaymentRepositoryException("Error occurred while updating payment. Payment not found. " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new PaymentRepositoryException("Error occurred while updating payment.", ex);
            }
        }
    }
}
