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
    public class PassengerRepository : IRepository<int, Passenger>
    {
        private readonly FlightBookingContext _context;

        public PassengerRepository(FlightBookingContext context)
        {
            _context = context;
        }

        public async Task<Passenger> Add(Passenger item)
        {
            try
            {
                _context.Add(item);
                await _context.SaveChangesAsync();
                return item;
            }
            catch (Exception ex)
            {
                throw new PassengerRepositoryException("Error occurred while adding passenger.", ex);
            }
        }

        public async Task<Passenger> DeleteByKey(int key)
        {
            try
            {
                var passenger = await GetByKey(key);
                _context.Remove(passenger);
                await _context.SaveChangesAsync(true);
                return passenger;
            }
            catch (NotPresentException ex)
            {
                throw new PassengerRepositoryException("Error occurred while deleting passenger. Passenger not found. " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new PassengerRepositoryException("Error occurred while deleting passenger.", ex);
            }
        }

        public async Task<IEnumerable<Passenger>> GetAll()
        {
            try
            {
                var passengers = await _context.Passengers.ToListAsync();
                if (passengers.Count < 0)
                {
                    throw new NotPresentException("No passengers present.");
                }
                return passengers;
            }
            catch (NotPresentException ex)
            {
                throw new PassengerRepositoryException("Error occurred while retrieving passengers. " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new PassengerRepositoryException("Error occurred while retrieving passengers.", ex);
            }
        }

        public async Task<Passenger> GetByKey(int key)
        {
            try
            {
                var passenger = await _context.Passengers.FirstOrDefaultAsync(p => p.PassengerId == key);
                if (passenger == null)
                    throw new NotPresentException("No such passenger is present.");
                return passenger;
            }
            catch (NotPresentException ex)
            {
                throw new PassengerRepositoryException("Error occurred while retrieving passenger. " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new PassengerRepositoryException("Error occurred while retrieving passenger.", ex);
            }
        }

        public async Task<Passenger> Update(Passenger item)
        {
            try
            {
                var passenger = await GetByKey(item.PassengerId);
                _context.Update(passenger);
                await _context.SaveChangesAsync(true);
                return passenger;
            }
            catch (NotPresentException ex)
            {
                throw new PassengerRepositoryException("Error occurred while updating passenger. Passenger not found. " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new PassengerRepositoryException("Error occurred while updating passenger.", ex);
            }
        }
    }
}
