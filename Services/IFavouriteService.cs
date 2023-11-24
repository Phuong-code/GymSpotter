using GymSpotter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSpotter.Services {

    public interface IFavouriteService {

        Task<int> AddFavourite(Favourite favourite);

        Task<int> UpdateFavourite(Favourite favourite);

        Task<int> DeleteFavourite(Favourite favourite);

        Task<List<Favourite>> GetFavouriteList();

        Task<List<Favourite>> GetFavouriteListByUserId(int userId);

        Task<Boolean> IsFavourite(int userId, int gymId);

        Task ClearDatabase();
    }
}