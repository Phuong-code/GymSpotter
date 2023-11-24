using GymSpotter.Models;
using GymSpotter.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSpotter.MockDB
{
    public class MockReviewDB
    {
        private readonly IReviewService _reviewService;
        public MockReviewDB(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        public async Task AddMockReviewToDatabase()
        {
            Review review1 = new Review
            {
                Id = 1,
                GymId = 1,
                UserId = 2,
                Rating = 1,
                Cleanliness = 2,
                Price = 3,
                Service = 5,
                Description = "One of the best places to excercise and relax in the city.",
                Date = new DateTime(2023, 10, 2),
            };

            Review review2 = new Review
            {
                Id = 2,
                GymId = 3,
                UserId = 3,
                Rating = 3,
                Cleanliness = 5,
                Price = 4,
                Service = 4,
                Description = "Decent but it was overhyped. Won't come again",
                Date = new DateTime(2023, 2, 6),
            };

            Review review3 = new Review
            {
                Id = 3,
                GymId = 1,
                UserId = 4,
                Rating = 2,
                Cleanliness = 3,
                Price = 3,
                Service = 3,
                Description = "Worst customer service",
                Date = new DateTime(2023, 3, 13),
            };

            Review review4 = new Review
            {
                Id = 4,
                GymId = 2,
                UserId = 1,
                Rating = 5,
                Cleanliness = 5,
                Price = 5,
                Service = 5,
                Description = "Amazing place and amazing staff",
                Date = new DateTime(2023, 5, 23),
            };

            await _reviewService.ClearDatabase();
            await _reviewService.AddReview(review1);
            await _reviewService.AddReview(review2);
            await _reviewService.AddReview(review3);
            await _reviewService.AddReview(review4);
            await AddDummyReviewData();
        }

        public async Task AddDummyReviewData()
        {
            List<int> userAdded = new();
            int randomUser;
            for (int gymId = 4; gymId <= 23; gymId++)
            {
                userAdded.Clear();
                while(userAdded.Count < 4)
                {
                    if (userAdded.Count == 0)
                    {
                        userAdded.Add(new Random().Next(1, 5));
                    }
                    else
                    {
                        randomUser = new Random().Next(1, 5);
                        if (!userAdded.Contains(randomUser))
                        {
                            userAdded.Add(randomUser);
                        }
                    }
                }
                for (int i = 1; i <= 3; i++)
                {
                    DateTime date = DateTime.Now.Subtract(TimeSpan.FromDays(new Random().Next(1, 160)));
                    if (i == 1)
                    {
                        Review review = new Review
                        {
                            GymId = gymId,
                            //UserId = new Random().Next(1, 5),
                            UserId = userAdded[i],
                            Rating = 5,
                            Cleanliness = 5,
                            Price = 4,
                            Service = 4,
                            Description = "Awesome gym to start of in if you are just a beginner like myself. Would like better availability of staff though.",
                            Date = date
                        };
                        await _reviewService.AddReview(review);
                    } else if (i == 2)
                    {
                        Review review = new Review
                        {
                            GymId = gymId,
                            //UserId = new Random().Next(1, 5),
                            UserId = userAdded[i],
                            Rating = 3,
                            Cleanliness = 4,
                            Price = 3,
                            Service = 4,
                            Description = "The gym is well serviced but the price is a little steep and the gym is not too spacious.",
                            Date = date
                        };
                        await _reviewService.AddReview(review);
                    } else
                    {
                        Review review = new Review
                        {
                            GymId = gymId,
                            //UserId = new Random().Next(1, 5),
                            UserId = userAdded[i],
                            Rating = 4,
                            Cleanliness = 3,
                            Price = 4,
                            Service = 5,
                            Description = "I absolutely love the great equipment here and fascilities, but the gym is not maintained very well and the changing rooms are always wet.",
                            Date = date
                        };
                        await _reviewService.AddReview(review);
                    }
                    
                }
            }
        }

    }
}
