using GymSpotter.Models;
using GymSpotter.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSpotter.MockDB
{
    public class MockUserDB
    {
        private readonly IUserService _userService;
        public MockUserDB(IUserService userService) 
        {
            _userService = userService;
        }


        public async Task AddMockUserToDatabase()
        {
            User user1 = new User
            {
                Id = 1,
                FirstName = "Phil",
                LastName = "Vu",
                Email = "duy.vu@gmail.com",
                Password = "asd",
                PhotoUrl = "phil.jpg",
            };

            User user2 = new User
            {
                Id = 2,
                FirstName = "Cao Vinh",
                LastName = "Lam",
                Email = "cao.lam@gmail.com",
                Password = "asd",
                PhotoUrl = "caovinh.png"
            };

            User user3 = new User
            {
                Id = 3,
                FirstName = "ZhanZhao",
                LastName = "Yang",
                Email = "zhanzhao.yang@gmail.com",
                Password = "asd",
                PhotoUrl = "zhanzhao.png"
            };

            User user4 = new User
            {
                Id = 4,
                FirstName = "Christian",
                LastName = "Lee",
                Email = "christian.lee@gmail.com",
                Password = "asd",
                PhotoUrl = "christian.jpeg"
            };

            await _userService.ClearDatabase();

            await _userService.AddUser(user1);
            await _userService.AddUser(user2);
            await _userService.AddUser(user3);
            await _userService.AddUser(user4);
        }
    }
}
