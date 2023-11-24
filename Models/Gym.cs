using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Devices.Sensors;

namespace GymSpotter.Models
{
    public class Gym
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public string Name { get; set; }
        public string PlaceId { get; set; } = string.Empty;
        public int Rating { get; set; } = 0;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string PhotoReferenceURL { get; set; }
        public double Distance { get; set; }
        public string PhoneNumber { get; set; }

        // Serialize the lists of strings to JSON strings
        public string UtilitiesSerialized
        {
            get => JsonConvert.SerializeObject(Utilities);
            set => Utilities = JsonConvert.DeserializeObject<IEnumerable<string>>(value);
        }

        public string TypesSerialized
        {
            get => JsonConvert.SerializeObject(Types);
            set => Types = JsonConvert.DeserializeObject<IEnumerable<string>>(value);
        }

        public string ServicesSerialized
        {
            get => JsonConvert.SerializeObject(Services);
            set => Services = JsonConvert.DeserializeObject<IEnumerable<string>>(value);
        }
        public string OpeningHoursSerialized
        {
            get => JsonConvert.SerializeObject(OpeningHours);
            set => OpeningHours = JsonConvert.DeserializeObject<IEnumerable<string>>(value);
        }

        // Lists of strings to be serialized
        [Ignore]
        public IEnumerable<string> Utilities { get; set; } = new List<string>();

        [Ignore]
        public IEnumerable<string> Types { get; set; } = new List<string>();

        [Ignore]
        public IEnumerable<string> Services { get; set; } = new List<string>();

        [Ignore]
        public IEnumerable<string> OpeningHours { get; set; } = new List<string>();
    }
}
