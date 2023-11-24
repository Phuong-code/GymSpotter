using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSpotter.Models
{
    public class Review
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int GymId { get; set; }
        public int UserId { get; set; }
        public int Rating { get; set; }
        public int Cleanliness { get; set; }
        public int Service { get; set; }
        public int Price { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
    }
}
