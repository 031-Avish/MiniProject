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
    public class UserDetailRepository : IRepository<int, UserDetail>
    {
        private readonly FlightBookingContext _context;

        public UserDetailRepository(FlightBookingContext context)
        {
            _context = context;
        }

        public async Task<UserDetail> Add(UserDetail item)
        {
            try
            {
                // Check if email already exists
                var existingUserByEmail = await _context.UserDetails.FirstOrDefaultAsync(u => u.Email == item.Email);
                if (existingUserByEmail != null)
                {
                    throw new DuplicateUserException("A user with the same email already exists.");
                }

                _context.Add(item);
                await _context.SaveChangesAsync();
                return item;
            }
            catch (DuplicateUserException ex)
            {
                throw new UserDetailRepositoryException("Error: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new UserDetailRepositoryException("Error occurred while adding user detail.", ex);
            }
        }

        public async Task<UserDetail> DeleteByKey(int key)
        {
            try
            {
                var userDetail = await GetByKey(key);
                _context.Remove(userDetail);
                await _context.SaveChangesAsync(true);
                return userDetail;
            }
            catch (NotPresentException ex)
            {
                throw new UserDetailRepositoryException("Error occurred while deleting user detail: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new UserDetailRepositoryException("Error occurred while deleting user detail.", ex);
            }
        }

        public async Task<IEnumerable<UserDetail>> GetAll()
        {
            try
            {
                var userDetails = await _context.UserDetails.ToListAsync();
                if (userDetails.Count <=0)
                {
                    throw new NotPresentException("There are no user details present.");
                }
                return userDetails;
            }
            catch (NotPresentException ex)
            {
                throw new UserDetailRepositoryException("Error occurred while retrieving user details: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new UserDetailRepositoryException("Error occurred while retrieving user details.", ex);
            }
        }

        public async Task<UserDetail> GetByKey(int key)
        {
            try
            {
                var userDetail = await _context.UserDetails.FirstOrDefaultAsync(u => u.UserId == key);
                if (userDetail == null)
                    throw new NotPresentException("No such user detail is present.");
                return userDetail;
            }
            catch (NotPresentException ex)
            {
                throw new UserDetailRepositoryException("Error occurred while retrieving user detail: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new UserDetailRepositoryException("Error occurred while retrieving user detail.", ex);
            }
        }

        public async Task<UserDetail> Update(UserDetail item)
        {
            try
            {
                var userDetail = await GetByKey(item.UserId);
                _context.Update(userDetail);
                await _context.SaveChangesAsync(true);
                return userDetail;
            }
            catch (NotPresentException ex)
            {
                throw new UserDetailRepositoryException("Error occurred while updating user detail. User detail not found: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new UserDetailRepositoryException("Error occurred while updating user detail.", ex);
            }
        }
    }
}
