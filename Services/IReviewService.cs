using GymSpotter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSpotter.Services
{
    public interface IReviewService
    {
        Task<List<Review>> GetReviewList();
        Task<int> AddReview(Review review);
        Task<int> DeleteReview(Review review);
        //Task<int> UpdateReview(Review review);
        //Task<Review> GetReviewById(int reviewId);
        Task ClearDatabase();
        Task<int> GetTotalReviewByGymId(int id);
        Task<List<Review>> GetAllGymReviewsByGymId(int gymId);
        Task<List<Review>> GetAllUserReviewsByUserId(int userId);
        Task<Review> GetReviewByUserIdAndGymId(int userId, int gymId);
        Task<int> UpdateReview(Review userReview);
    }
}
