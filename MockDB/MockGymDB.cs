using GymSpotter.Models;
using GymSpotter.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSpotter.MockDB
{
    public class MockGymDB
    {
        private readonly IGymService _gymService;
        private readonly IReviewService _reviewService;
        public MockGymDB(IGymService gymService, IReviewService reviewService)
        {
            _gymService = gymService;
            _reviewService = reviewService;
        }

        public async Task AddMockGymToDatabase()
        {
            Gym gym1 = new Gym
            {
                Id = 1,
                OwnerId = 1,
                Name = "Phil-Gym",
                PlaceId = "1",
                Rating = 4,
                Utilities = new List<string> { "Treadmills", "Free Weights", "Sauna" },
                Types = new List<string> { "24/7", "Female Only", "Crossfit", "Dojo", "Boxing" },
                Services = new List<string> { "Personal Training", "Group Classes", "Nutrition Counseling" },
                Description = "Join Phil-Gym. Located in the heart of Collins Street, it is the perfect gym for those working and living right in the centre of Melbourne city. The state-of-the-art facilities include a rooftop tennis court, indoor pool, top of the range fitness equipment and a group of committed trainers to help you fulfil your fitness goals.",
                Location = "123 Collin St, Melbourne VIC 3000",
                Latitude = -37.811112,      
                Longitude = 144.968728,
            };

            Gym gym2 = new Gym
            {
                Id = 2,
                OwnerId = 2,
                Name = "FitZone",
                PlaceId = "2",
                Rating = 3,
                Utilities = new List<string> { "Treadmills", "Ellipticals", "Strength Machines" },
                Types = new List<string> { "Dojo", "Crossfit", "Boxing" },
                Services = new List<string> { "Personal Training", "Yoga Classes", "Diet Planning" },
                Description = "Discover a new level of fitness at FitZone. Our modern gym offers a variety of equipment and classes to help you reach your fitness goals. Whether you're into strength training or yoga, we've got you covered.",
                Location = "1/45 Exhibition St, Melbourne VIC 3000",
                Latitude = -37.812660,               
                Longitude = 144.958054,
            };

            Gym gym3 = new Gym
            {
                Id = 3,
                OwnerId = 3,
                Name = "ZenFitness",
                PlaceId = "3",
                Rating = 2,
                Utilities = new List<string> { "Yoga Mats", "Meditation Rooms", "Sauna" },
                Types = new List<string> { "MMA", "Powerlifting" },
                Services = new List<string> { "Yoga Classes", "Meditation Workshops" },
                Description = "Experience tranquility and inner peace at ZenFitness. Our yoga and meditation center provides a serene environment to relax your mind and body. Join us for yoga classes and meditation workshops.",
                Location = "Shop 3/153 Station St, Fairfield VIC 3078",
                Latitude = -37.813385,
                Longitude = 144.961264,
            };

            //Comment this block out after first run
            //await _gymService.ClearDatabase();
            //await _gymService.AddGym(gym1);
            //await _gymService.AddGym(gym2);
            //await _gymService.AddGym(gym3);
            //End block

            await UpdateGymData();
            
        }

        public async Task UpdateGymData()
        {
            List<Gym> existingGyms = await _gymService.GetGymList();
            foreach (var gym in existingGyms)
            {
                if (gym.Id % 2 == 0)
                {
                    gym.OwnerId = new Random().Next(1, 5);                   

                    gym.Rating = 5;

                    gym.Utilities = new List<string> { "New Equipment", "Cardio Machines", "Weights" };

                    gym.Types = new List<string> { "Crossfit", "Boxing" };

                    gym.Services = new List<string> { "Group Zumba Classes", "Personal Training" };

                    gym.Description = "Embark on a fitness journey like never before at this gym. We're not just a gym; we're a community that believes in empowering every individual to reach their peak potential. Welcome to a space where passion, dedication, and transformation converge.\r\n\r\nAt, we've curated an environment where fitness meets innovation. Our cutting-edge facility is equipped with state-of-the-art equipment, offering a diverse range of workout options to suit every fitness goal. Whether you're a seasoned fitness enthusiast or taking your first steps towards a healthier lifestyle, our expert trainers are here to guide and inspire you.";

                    await _gymService.UpdateGym(gym);

                } else if (gym.Id % 3 == 0)
                {
                    gym.OwnerId = new Random().Next(1, 5);

                    gym.Rating = 4;

                    gym.Utilities = new List<string> { "Open Workout Room", "Pull up bars", "Lockers", "Hair Dryers" };

                    gym.Types = new List<string> { "24/7", "Powerlifting" };

                    gym.Services = new List<string> { "Group HIIT Workout", "Personal Training",  "Pilates"};

                    gym.Description = "Welcome to our Gym, where passion meets discipline in the world of Mixed Martial Arts (MMA). At this gym, we redefine fitness, strength, and skill through the dynamic and empowering art of MMA.\r\n\r\nOur state-of-the-art facility is more than just a gym; it's a community of like-minded individuals driven by a shared love for martial arts and personal growth. Whether you're a seasoned fighter or a beginner looking to explore the world of MMA, our experienced trainers and welcoming atmosphere are here to guide you on your journey.";

                    await _gymService.UpdateGym(gym);
                }
                else
                {
                    gym.OwnerId = new Random().Next(1, 5);

                    gym.Rating = 3;

                    gym.Utilities = new List<string> { "Sauna", "Showers", "Cycling Studio" };

                    gym.Types = new List<string> { "24/7", "MMA", "Dojo" };

                    gym.Services = new List<string> { "Group Classes", "Personal Training", "Sparring Sessions"};

                    gym.Description = "What sets us apart is our commitment to creating a supportive and inclusive community. It's not just about lifting weights; it's about fostering connections, celebrating achievements, and creating a positive impact on your overall well-being.\r\n\r\nJoin us at [Your Gym Name] and experience fitness in a whole new light. From invigorating group classes to personalized training sessions, we're here to help you sculpt your ideal self. Unleash your potential, break barriers, and achieve your fitness dreams with us.\r\n\r\nReady to transform your life? Welcome to our gym, where the journey to a stronger, healthier you begins! 💪✨";

                    await _gymService.UpdateGym(gym);
                }
            }

        }

        public async Task UpdateAverageGymRatings()
        {
            List<Gym> existingGyms = await _gymService.GetGymList();
            foreach(var gym in existingGyms)
            {
                List<Review> gymReviews = await _reviewService.GetAllGymReviewsByGymId(gym.Id);
                int averageRating = 0;
                int reviewCount = gymReviews.Count();

                foreach (var review in gymReviews)
                {
                    averageRating += review.Rating;
                }

                averageRating = (int)Math.Floor((double)averageRating / gymReviews.Count());
                gym.Rating = averageRating;
                await _gymService.UpdateGym(gym);
            }
        }

    }
}
