using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using Microsoft.Maui.Devices.Sensors;
using Map = Microsoft.Maui.Controls.Maps.Map;
using GymSpotter.Models;
using GymSpotter.Services;
using System.Web;

namespace GymSpotter.Views;

public partial class MapPage : ContentPage, IQueryAttributable
{

    private CancellationTokenSource _cancelTokenSource;
    private Microsoft.Maui.Devices.Sensors.Location currentLocation;
    private int UserId;
    private IGymService _gymService;
    private IUserService _userService;

    public MapPage()
    {
        InitializeComponent();

        this.Appearing += MapPage_Appearing;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query == null || query.Count == 0) return;

        UserId = int.Parse(HttpUtility.UrlDecode(query["userId"].ToString()));

    }

    private async void MapPage_Appearing(object sender, EventArgs e)
    {
        await LoadMapWithDeviceLocationAsync();
    }

    public async Task LoadMapWithDeviceLocationAsync()
    {
        try
        {
            GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
            _cancelTokenSource = new CancellationTokenSource();
            currentLocation = await Geolocation.Default.GetLocationAsync(request, _cancelTokenSource.Token);

            if (currentLocation != null)
            {
                map = new Map
                {
                    IsShowingUser = true,
                };

                Content = map;

                MapSpan mapSpan = new MapSpan(currentLocation, 0.02, 0.02);
                map.MoveToRegion(mapSpan);

                await GetNearbyPlaces(map);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error getting device location: ", $"{ex}", "OK");
        }
    }

    public async Task GetNearbyPlaces(Map map)
    {
        double latitude = currentLocation.Latitude;
        double longitude = currentLocation.Longitude;
        int radius = 1000;

        var placesService = new PlacesApiService();
        List<Place> nearbyPlaces = await placesService.GetNearbyPlacesAsync(latitude, longitude, radius);

        await PopulateDatabase(nearbyPlaces);
        await LoadPinsFromDatabase();
        await AddUserLocation();

        if (nearbyPlaces != null)
        {
            foreach (var place in nearbyPlaces)
            {
                var gym = await _gymService.GetGymByPlaceId(place.PlaceId);
                var pin = new Pin
                {
                    Label = place.Name,
                    Address = place.Vicinity,
                    Type = PinType.Place,
                    Location = new Microsoft.Maui.Devices.Sensors.Location(place.Geometry.Location.Latitude, place.Geometry.Location.Longitude),
                };

#if IOS
                pin.MarkerClicked += async(s, args) => {
                    //var result = await Shell.Current.DisplayAlert("View details of " + pin.Label + "?", pin.Address, "OK", "Cancel");
                    //if (result) {
                    //    await Shell.Current.GoToAsync($"{nameof(GymDetailsPage)}?userId={UserId}&gymId={gym.Id}");
                    //}
                    await Shell.Current.GoToAsync($"{nameof(GymDetailsPage)}?userId={UserId}&gymId={gym.Id}");
                };
#endif
                pin.InfoWindowClicked += async (s, args) =>
                {
                    await Shell.Current.GoToAsync($"{nameof(GymDetailsPage)}?userId={UserId}&gymId={gym.Id}");
                };

                map.Pins.Add(pin);
            }
        }
    }

    public async Task AddUserLocation()
    {
        _userService = new UserService();

        User user = await _userService.GetUserById(UserId);
        user.Longitude = currentLocation.Longitude;
        user.Latitude = currentLocation.Latitude;

        await _userService.UpdateUser(user);
    }

    public async Task PopulateDatabase(List<Place> nearbyPlaces)
    {
        _gymService = new GymService();

        try
        {
            int count = 0;
            foreach (var place in nearbyPlaces)
            {
                var existingGym = await _gymService.GetGymByPlaceId(place.PlaceId);

                if (existingGym == null)
                {
                    var gym = new Gym
                    {
                        Name = place.Name,
                        PlaceId = place.PlaceId,
                        Location = place.Vicinity,
                        Latitude = place.Geometry.Location.Latitude,
                        Longitude = place.Geometry.Location.Longitude,
                        PhotoReferenceURL = $"https://maps.googleapis.com/maps/api/place/photo?maxwidth=1280&photoreference={place.FirstPhotoReference}&key=AIzaSyBflN-k9mo3VWLWeX4KNPiaWDUS8Pt6Hdc",
                    };
                    if (count < 10)
                    {
                        count++;
                        await _gymService.AddGym(gym);
                    }
                    else break;
                }
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error populating Database", $"{ex}", "OK");
        }
    }

    public async Task LoadPinsFromDatabase()
    {
        var gyms = await _gymService.GetGymList();
        foreach (var gym in gyms)
        {
            var pin = new Pin
            {
                Label = gym.Name,
                Address = gym.Location,
                Type = PinType.Place,
                Location = new Microsoft.Maui.Devices.Sensors.Location(gym.Latitude, gym.Longitude),
            };
            pin.InfoWindowClicked += async (s, args) =>
            {
                await Shell.Current.GoToAsync($"{nameof(GymDetailsPage)}?userId={UserId}&gymId={gym.Id}");
            };
            map.Pins.Add(pin);
        }
    }


    public class PlacesApiService
    {
        private const string PlacesApiBaseUrl = "https://maps.googleapis.com/maps/api/place/nearbysearch/json";
        private const string ApiKey = "AIzaSyBflN-k9mo3VWLWeX4KNPiaWDUS8Pt6Hdc";
        private const string Keyword = "gym|fitness|workout|boxing|training|powerhouse|fit|jetts|F45|strength";

        public async Task<List<Place>> GetNearbyPlacesAsync(double latitude, double longitude, int radius)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {

                    string requestUrl = $"{PlacesApiBaseUrl}?keyword={Keyword}&location={latitude},{longitude}&radius={radius}&key={ApiKey}";

                    HttpResponseMessage response = await httpClient.GetAsync(requestUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonContent = await response.Content.ReadAsStringAsync();

                        // Deserialize 
                        var placesResponse = JsonConvert.DeserializeObject<PlacesResponse>(jsonContent);

                        if (placesResponse?.Results != null)
                        {
                            return placesResponse.Results;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return null;
        }
    }

    public class PlacesResponse
    {
        [JsonProperty("results")]
        public List<Place> Results { get; set; }

    }

    public class Place
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("vicinity")]
        public string Vicinity { get; set; }

        [JsonProperty("geometry")]
        public Geometry Geometry { get; set; }

        [JsonProperty("place_id")]
        public string PlaceId { get; set; }

        [JsonProperty("photos")]
        public List<Photo> Photos { get; set; }

        public string FirstPhotoReference => Photos?.FirstOrDefault()?.PhotoReference;
    }

    public class Geometry
    {
        [JsonProperty("location")]
        public Location Location { get; set; }

    }
    public class Photo
    {
        [JsonProperty("photo_reference")]
        public string PhotoReference { get; set; }
    }

    public class Location
    {
        [JsonProperty("lat")]
        public double Latitude { get; set; }

        [JsonProperty("lng")]
        public double Longitude { get; set; }

        public Location(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
    }

}
