using FlightBookingSystemAPI.Contexts;
using FlightBookingSystemAPI.Interfaces;
using FlightBookingSystemAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlightBookingSystemAPI.Repositories
{
    public class UserRepository : IRepository<int, User>
    {
        private readonly FlightBookingContext _context;

        public UserRepository(FlightBookingContext context)
        {
            _context = context;
        }

        public async Task<User> Add(User item)
        {
            try
            {
                _context.Add(item);
                await _context.SaveChangesAsync();
                return item;
            }
            catch (Exception ex)
            {
                throw new UserRepositoryException("Error occurred while adding user." , ex);
            }
        }

        public async Task<User> DeleteByKey(int key)
        {
            try
            {
                var user = await GetByKey(key);
                _context.Remove(user);
                await _context.SaveChangesAsync(true);
                return user;
            }
            catch (NotPresentException ex)
            {
                throw new UserRepositoryException("Error occurred while deleting user " + ex.Message , ex);
            }
            catch (Exception ex)
            {
                throw new UserRepositoryException("Error occurred while deleting user.", ex);
            }
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            try
            {
                var users = await _context.Users.ToListAsync();
                if(users.Count<0)
                {
                    throw new NotPresentException("There is not user Present");
                }
                return users;
            }
            catch(NotPresentException ex)
            {
                throw new UserRepositoryException("Error occured while retrieving users" + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new UserRepositoryException("Error occurred while retrieving users." , ex);
            }
        }

        public async Task<User> GetByKey(int key)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == key);
                if (user == null)
                    throw new NotPresentException("No such user is present wi");
                return user;
            }
            catch (NotPresentException ex)
            {
                throw new UserRepositoryException("Error occurred while rerieving user" + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new UserRepositoryException("Error occurred while retrieving user." , ex );
            }
        }

        public async Task<User> Update(User item)
        {
            try
            {
                var user = await GetByKey(item.UserId);
                _context.Update(user);
                await _context.SaveChangesAsync(true);
                return user;
            }
            catch (NotPresentException ex)
            {
                throw new UserRepositoryException("Error occurred while updating user. User not found."+ ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new UserRepositoryException("Error occurred while updating user.", ex);
            }
        }
    }
}
