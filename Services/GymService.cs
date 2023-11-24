using GymSpotter.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSpotter.Services {

    public class GymService : IGymService {
        private SQLiteAsyncConnection _dbConnection;

        private async Task SetUpDb() {
            if (_dbConnection == null) {
                string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Gym.db3");
                _dbConnection = new SQLiteAsyncConnection(dbPath);
                await _dbConnection.CreateTableAsync<Gym>();
            }
        }

        public async Task<int> AddGym(Gym gym) {
            await SetUpDb();
            return await _dbConnection.InsertAsync(gym);
        }

        public async Task<int> DeleteGym(Gym gym) {
            await SetUpDb();
            return await _dbConnection.DeleteAsync(gym);
        }

        public async Task<List<Gym>> GetGymList() {
            await SetUpDb();
            var gyms = await _dbConnection.Table<Gym>().ToListAsync();
            return gyms;
        }

        public async Task<int> UpdateGym(Gym gym) {
            await SetUpDb();
            return await _dbConnection.UpdateAsync(gym);
        }

        public async Task ClearDatabase() {
            await SetUpDb();
            await _dbConnection.DeleteAllAsync<Gym>();

            // Reset the auto-increment counter for the User table using a raw SQL query
            var resetSql = $"DELETE FROM sqlite_sequence WHERE name = 'Gym'";
            await _dbConnection.ExecuteAsync(resetSql);
        }

        public async Task<Gym> GetGymById(int gymId)
        {
            await SetUpDb();
            var gym = await _dbConnection.Table<Gym>()
                .Where(g => g.Id == gymId)
                .FirstOrDefaultAsync();

            return gym;
        }

        public async Task<List<Gym>> GetGymByOwnerId(int ownerId) {
            await SetUpDb();
            var gyms = await _dbConnection.Table<Gym>()
                .Where(g => g.OwnerId == ownerId)
                .ToListAsync();

            return gyms;
        }

        public async Task<Gym> GetGymByPlaceId(string placeId)
        {
            await SetUpDb();
            var gym = await _dbConnection.Table<Gym>()
                .Where(g => g.PlaceId.Equals(placeId))
                .FirstOrDefaultAsync();

            return gym;
        }

    }
}