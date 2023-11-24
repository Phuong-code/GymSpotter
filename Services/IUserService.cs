using GymSpotter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSpotter.Services
{
    public interface IUserService
    {
        Task<List<User>> GetUserList();
        Task<int> AddUser(User user);
        Task<int> DeleteUser(User user);
        Task<int> UpdateUser(User user);
        Task<User> GetUserByEmail(string email);
        Task<User> GetUserById(int Id);
        Task ClearDatabase();
    }
}
