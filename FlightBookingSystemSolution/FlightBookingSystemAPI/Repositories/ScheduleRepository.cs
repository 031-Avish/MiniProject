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
    public class ScheduleRepository : IRepository<int, Schedule>
    {
        private readonly FlightBookingContext _context;

        public ScheduleRepository(FlightBookingContext context)
        {
            _context = context;
        }

        public async Task<Schedule> Add(Schedule item)
        {
            try
            {
                _context.Add(item);
                await _context.SaveChangesAsync();
                return item;
            }
            catch (Exception ex)
            {
                throw new ScheduleRepositoryException("Error occurred while adding schedule.", ex);
            }
        }

        public async Task<Schedule> DeleteByKey(int key)
        {
            try
            {
                var schedule = await GetByKey(key);
                _context.Remove(schedule);
                await _context.SaveChangesAsync(true);
                return schedule;
            }
            catch (NotPresentException ex)
            {
                throw new ScheduleRepositoryException("Error occurred while deleting schedule. Schedule not found. " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new ScheduleRepositoryException("Error occurred while deleting schedule.", ex);
            }
        }

        public async Task<IEnumerable<Schedule>> GetAll()
        {
            try
            {
                var schedules = await _context.Schedules.ToListAsync();
                if (schedules.Count < 0)
                {
                    throw new NotPresentException("No schedules present.");
                }
                return schedules;
            }
            catch (NotPresentException ex)
            {
                throw new ScheduleRepositoryException("Error occurred while retrieving schedules. " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new ScheduleRepositoryException("Error occurred while retrieving schedules.", ex);
            }
        }

        public async Task<Schedule> GetByKey(int key)
        {
            try
            {
                var schedule = await _context.Schedules.FirstOrDefaultAsync(s => s.ScheduleId == key);
                if (schedule == null)
                    throw new NotPresentException("No such schedule is present.");
                return schedule;
            }
            catch (NotPresentException ex)
            {
                throw new ScheduleRepositoryException("Error occurred while retrieving schedule. " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new ScheduleRepositoryException("Error occurred while retrieving schedule.", ex);
            }
        }

        public async Task<Schedule> Update(Schedule item)
        {
            try
            {
                var schedule = await GetByKey(item.ScheduleId);
                _context.Update(schedule);
                await _context.SaveChangesAsync(true);
                return schedule;
            }
            catch (NotPresentException ex)
            {
                throw new ScheduleRepositoryException("Error occurred while updating schedule. Schedule not found. " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new ScheduleRepositoryException("Error occurred while updating schedule.", ex);
            }
        }
    }
}
