using GymSpotter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSpotter.Services {

    public interface IGymService {

        Task<int> AddGym(Gym gym);

        Task<int> UpdateGym(Gym gym);

        Task<int> DeleteGym(Gym gym);

        Task<List<Gym>> GetGymList();

        Task<Gym> GetGymById(int id);

        Task<List<Gym>> GetGymByOwnerId(int ownerId);

        Task<Gym> GetGymByPlaceId(string placeId);

        Task ClearDatabase();
    }
}