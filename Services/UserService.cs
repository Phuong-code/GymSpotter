using GymSpotter.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSpotter.Services {

    public class UserService : IUserService {
        private SQLiteAsyncConnection _dbConnection;

        private async Task SetUpDb() {
            if (_dbConnection == null) {
                string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "User.db3");
                _dbConnection = new SQLiteAsyncConnection(dbPath);
                await _dbConnection.CreateTableAsync<User>();
            }
        }

        public async Task<int> AddUser(User user) {
            await SetUpDb();
            return await _dbConnection.InsertAsync(user);
        }

        public async Task<int> DeleteUser(User user) {
            await SetUpDb();
            return await _dbConnection.DeleteAsync(user);
        }

        public async Task<List<User>> GetUserList() {
            await SetUpDb();
            var users = await _dbConnection.Table<User>().ToListAsync();
            return users;
        }

        public async Task<int> UpdateUser(User user) {
            await SetUpDb();
            return await _dbConnection.UpdateAsync(user);
        }

        public async Task<User> GetUserByEmail(string email) {
            await SetUpDb();

            var user = await _dbConnection.Table<User>()
                .Where(u => u.Email == email)
                .FirstOrDefaultAsync();

            return user;
        }

        public async Task<User> GetUserById(int Id)
        {
            await SetUpDb();

            var user = await _dbConnection.Table<User>()
                .Where(u => u.Id == Id)
                .FirstOrDefaultAsync();

            return user;
        }

        public async Task ClearDatabase()
        {
            await SetUpDb();
            await _dbConnection.DeleteAllAsync<User>();

            // Reset the auto-increment counter for the User table using a raw SQL query
            var resetSql = $"DELETE FROM sqlite_sequence WHERE name = 'User'";
            await _dbConnection.ExecuteAsync(resetSql);
        }

    }
}