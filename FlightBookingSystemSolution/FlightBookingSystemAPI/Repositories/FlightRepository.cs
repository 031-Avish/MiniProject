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
    public class FlightRepository : IRepository<int, Flight>
    {
        private readonly FlightBookingContext _context;

        public FlightRepository(FlightBookingContext context)
        {
            _context = context;
        }

        public async Task<Flight> Add(Flight item)
        {
            try
            {
                _context.Add(item);
                await _context.SaveChangesAsync();
                return item;
            }
            catch (Exception ex)
            {
                throw new FlightRepositoryException("Error occurred while adding flight.", ex);
            }
        }

        public async Task<Flight> DeleteByKey(int key)
        {
            try
            {
                var flight = await GetByKey(key);
                _context.Remove(flight);
                await _context.SaveChangesAsync(true);
                return flight;
            }
            catch (NotPresentException ex)
            {
                throw new FlightRepositoryException("Error occurred while deleting flight. Flight not found. " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new FlightRepositoryException("Error occurred while deleting flight.", ex);
            }
        }

        public async Task<IEnumerable<Flight>> GetAll()
        {
            try
            {
                var flights = await _context.Flights.ToListAsync();
                if (flights.Count <= 0)
                {
                    throw new NotPresentException("No flights present.");
                }
                return flights;
            }
            catch (NotPresentException ex)
            {
                throw new FlightRepositoryException("Error occurred while retrieving flights. " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new FlightRepositoryException("Error occurred while retrieving flights.", ex);
            }
        }

        public async Task<Flight> GetByKey(int key)
        {
            try
            {
                var flight = await _context.Flights.FirstOrDefaultAsync(f => f.FlightId == key);
                if (flight == null)
                    throw new NotPresentException("No such flight is present.");
                return flight;
            }
            catch (NotPresentException ex)
            {
                throw new FlightRepositoryException("Error occurred while retrieving flight. " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new FlightRepositoryException("Error occurred while retrieving flight.", ex);
            }
        }

        public async Task<Flight> Update(Flight item)
        {
            try
            {
                var flight = await GetByKey(item.FlightId);
                _context.Update(flight);
                await _context.SaveChangesAsync(true);
                return flight;
            }
            catch (NotPresentException ex)
            {
                throw new FlightRepositoryException("Error occurred while updating flight. Flight not found. " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new FlightRepositoryException("Error occurred while updating flight.", ex);
            }
        }
    }
}

