using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSpotter.Models
{
    public class User
    {
        [PrimaryKey,AutoIncrement]
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;    
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string PhotoUrl { get; set; }
    }
}
