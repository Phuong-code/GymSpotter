using GymSpotter.Models;
using GymSpotter.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSpotter.MockDB {

    public class MockFavouriteDB {
        private readonly IFavouriteService _favouriteService;

        public MockFavouriteDB(IFavouriteService favouriteService) {
            _favouriteService = favouriteService;
        }

        public async Task AddMockFavouriteToDatabase() {
            Favourite favourite1 = new Favourite {
                Id = 1,
                UserId = 1,
                GymId = 2,
            };
            Favourite favourite2 = new Favourite {
                Id = 2,
                UserId = 1,
                GymId = 3,
            };
            Favourite favourite3 = new Favourite {
                Id = 4,
                UserId = 2,
                GymId = 1,
            };
            Favourite favourite4 = new Favourite {
                Id = 5,
                UserId = 2,
                GymId = 3,
            };
            Favourite favourite5 = new Favourite {
                Id = 6,
                UserId = 3,
                GymId = 1,
            };

            await _favouriteService.ClearDatabase();
            await _favouriteService.AddFavourite(favourite1);
            await _favouriteService.AddFavourite(favourite2);
            await _favouriteService.AddFavourite(favourite3);
            await _favouriteService.AddFavourite(favourite4);
            await _favouriteService.AddFavourite(favourite5);
        }
    }
}