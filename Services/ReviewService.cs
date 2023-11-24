using AlohaKit.Controls;
using GymSpotter.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSpotter.Services
{
    public class ReviewService : IReviewService
    {
        private SQLiteAsyncConnection _dbConnection;

        private async Task SetUpDb()
        {
            if (_dbConnection == null)
            {
                string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Review.db3");
                _dbConnection = new SQLiteAsyncConnection(dbPath);
                await _dbConnection.CreateTableAsync<Review>();
            }
        }

        public async Task<int> AddReview(Review review)
        {
            await SetUpDb();
            return await _dbConnection.InsertAsync(review);
        }

        public async Task<int> UpdateReview(Review review)
        {
            await SetUpDb();
            return await _dbConnection.UpdateAsync(review);
        }
        public async Task<int> DeleteReview(Review reivew)
        {
            await SetUpDb();
            return await _dbConnection.DeleteAsync(reivew);
        }

        public async Task<List<Review>> GetReviewList()
        {
            await SetUpDb();
            var reivews = await _dbConnection.Table<Review>().ToListAsync();
            return reivews;
        }

        public async Task ClearDatabase()
        {
            await SetUpDb();
            await _dbConnection.DeleteAllAsync<Review>();

            // Reset the auto-increment counter for the User table using a raw SQL query
            var resetSql = $"DELETE FROM sqlite_sequence WHERE name = 'Review'";
            await _dbConnection.ExecuteAsync(resetSql);
        }

        public async Task<int> GetTotalReviewByGymId(int gymId)
        {
            await SetUpDb();
            int totalReviewCount = await _dbConnection.Table<Review>()
                .Where(r => r.GymId == gymId)
                .CountAsync();

            return totalReviewCount;
        }

        public async Task<List<Review>> GetAllGymReviewsByGymId(int gymId)
        {
            await SetUpDb();
            List<Review> reviews = await _dbConnection.Table<Review>()
                .Where(r => r.GymId == gymId)
                .ToListAsync();

            return reviews;
        }

        public async Task<List<Review>> GetAllUserReviewsByUserId(int userId)
        {
            await SetUpDb();
            List<Review> reviews = await _dbConnection.Table<Review>()
                .Where(r => r.UserId == userId)
                .ToListAsync();

            return reviews;
        }

        public async Task<Review> GetReviewByUserIdAndGymId(int userId, int gymId)
        {
            await SetUpDb();
            var review = await _dbConnection.Table<Review>()
                .Where(r => r.UserId == userId && r.GymId == gymId)
                .FirstOrDefaultAsync();

            return review;
        }
    }
}
