using GymSpotter.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSpotter.Services {

    public class FavouriteService : IFavouriteService {
        private SQLiteAsyncConnection _dbConnection;

        private async Task SetUpDb() {
            if (_dbConnection == null) {
                string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Favourite.db3");
                _dbConnection = new SQLiteAsyncConnection(dbPath);
                await _dbConnection.CreateTableAsync<Favourite>();
            }
        }

        public async Task<int> AddFavourite(Favourite favourite) {
            await SetUpDb();
            return await _dbConnection.InsertAsync(favourite);
        }

        public async Task<int> DeleteFavourite(Favourite favourite) {
            await SetUpDb();
            Favourite favouriteToDelete =
                await _dbConnection.Table<Favourite>().Where(
                    f => f.UserId == favourite.UserId && f.GymId == favourite.GymId)
                    .FirstOrDefaultAsync();
            return await _dbConnection.DeleteAsync(favouriteToDelete);
        }

        public async Task<List<Favourite>> GetFavouriteList() {
            await SetUpDb();
            var favourites = await _dbConnection.Table<Favourite>().ToListAsync();
            return favourites;
        }

        public async Task<List<Favourite>> GetFavouriteListByUserId(int userId) {
            await SetUpDb();
            var favourites = await _dbConnection.Table<Favourite>().Where(f => f.UserId == userId).ToListAsync();
            return favourites;
        }

        public async Task<Boolean> IsFavourite(int userId, int gymId) {
            await SetUpDb();
            var favourite = await _dbConnection.Table<Favourite>().Where(f => f.UserId == userId && f.GymId == gymId).FirstOrDefaultAsync();
            return favourite != null;
        }

        public async Task<int> UpdateFavourite(Favourite favourite) {
            await SetUpDb();
            return await _dbConnection.UpdateAsync(favourite);
        }

        public async Task ClearDatabase() {
            await SetUpDb();
            await _dbConnection.DeleteAllAsync<Favourite>();

            // Reset the auto-increment counter for the User table using a raw SQL query
            var resetSql = $"DELETE FROM sqlite_sequence WHERE name = 'Favourite'";
            await _dbConnection.ExecuteAsync(resetSql);
        }
    }
}