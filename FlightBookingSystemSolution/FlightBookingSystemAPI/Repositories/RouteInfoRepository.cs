using FlightBookingSystemAPI.Contexts;
using FlightBookingSystemAPI.Exceptions;
using FlightBookingSystemAPI.Exceptions.RepositoryException;
using FlightBookingSystemAPI.Interfaces;
using FlightBookingSystemAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlightBookingSystemAPI.Repositories
{
    public class RouteInfoRepository : IRepository<int, RouteInfo>
    {
        private readonly FlightBookingContext _context;

        public RouteInfoRepository(FlightBookingContext context)
        {
            _context = context;
        }

        public async Task<RouteInfo> Add(RouteInfo item)
        {
            try
            {
                _context.Add(item);
                await _context.SaveChangesAsync();
                return item;
            }
            catch (Exception ex)
            {
                throw new RouteInfoRepositoryException("Error occurred while adding route info.", ex);
            }
        }

        public async Task<RouteInfo> DeleteByKey(int key)
        {
            try
            {
                var routeInfo = await GetByKey(key);
                _context.Remove(routeInfo);
                await _context.SaveChangesAsync(true);
                return routeInfo;
            }
            catch (NotPresentException ex)
            {
                throw new RouteInfoRepositoryException("Error occurred while deleting route info. Route not found. " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new RouteInfoRepositoryException("Error occurred while deleting route info.", ex);
            }
        }

        public async Task<IEnumerable<RouteInfo>> GetAll()
        {
            try
            {
                var routes = await _context.RouteInfos.ToListAsync();
                if (routes.Count <= 0)
                {
                    throw new NotPresentException("No routes present.");
                }
                return routes;
            }
            catch (NotPresentException ex)
            {
                throw new RouteInfoRepositoryException("Error occurred while retrieving routes. " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new RouteInfoRepositoryException("Error occurred while retrieving routes.", ex);
            }
        }

        public async Task<RouteInfo> GetByKey(int key)
        {
            try
            {
                var routeInfo = await _context.RouteInfos.FirstOrDefaultAsync(r => r.RouteId == key);
                if (routeInfo == null)
                    throw new NotPresentException("No such route is present.");
                return routeInfo;
            }
            catch (NotPresentException ex)
            {
                throw new RouteInfoRepositoryException("Error occurred while retrieving route info. " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new RouteInfoRepositoryException("Error occurred while retrieving route info.", ex);
            }
        }

        public async Task<RouteInfo> Update(RouteInfo item)
        {
            try
            {
                var routeInfo = await GetByKey(item.RouteId);
                _context.Update(routeInfo);
                await _context.SaveChangesAsync(true);
                return routeInfo;
            }
            catch (NotPresentException ex)
            {
                throw new RouteInfoRepositoryException("Error occurred while updating route info. Route not found. " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new RouteInfoRepositoryException("Error occurred while updating route info.", ex);
            }
        }
    }
}
