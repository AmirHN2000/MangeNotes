using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManageNotes.Data;
using Microsoft.EntityFrameworkCore;

namespace ManageNotes.Services
{
    public class UserServices
    {
        private ApplicationContext _applicationContext;

        public UserServices(ApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public async Task<User> GetUserWithUserNameAsync(String username)
        {
            return await _applicationContext.Users
                .FirstOrDefaultAsync(x => x.UserName == username);
        }

        public async Task<bool> IsExistUserWithUsernameAsync(String username)
        {
            return await _applicationContext
                .Users
                .AnyAsync(x => x.UserName == username);
        }

        public async Task AddUserAsync(User user)
        {
            await _applicationContext
                .Users
                .AddAsync(user);
        }

        public async Task<User> FindAsync(int? userId)
        {
            return await _applicationContext
                .Users
                .FindAsync(userId);
        }

        public async Task<List<User>> GetAllUsers(TypeAccountEnum role)
        {
            var list = await _applicationContext
                .Users
                .AsNoTracking()
                .Where(x => x.Role == role)
                .ToListAsync();
            return list;
        }

        public async Task<bool> RemoveUser(int id)
        {
            var user = await FindAsync(id);
            if (user is null)
            {
                return false;
            }

            _applicationContext.Users.Remove(user);
            return true;
        }
    }
}