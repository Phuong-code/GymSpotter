using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using GymSpotter.Models;
using GymSpotter.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GymSpotter.Views;
using Newtonsoft.Json;

namespace GymSpotter.ViewModels
{

    public partial class ListViewPageViewModel : ObservableObject, IQueryAttributable
    {

        [ObservableProperty]
        private User user;

        [ObservableProperty]
        private Gym gym;

        [ObservableProperty]
        public List<Gym> gyms = new();

        [ObservableProperty]
        private List<Gym> allGyms;

        [ObservableProperty]
        private List<Gym> oGGyms;

        [ObservableProperty]
        private ObservableCollection<GymWithTypeCount> gymsWithTypeCount;

        [ObservableProperty]
        private ObservableCollection<GymWithTypeCount> favouriteGymsWithTypeCounts;

        [ObservableProperty]
        private bool isProcessing;

        private List<string> filterList = new();

        [ObservableProperty]
        private Color filterAllBorderColor = Color.FromArgb("#008000");

        [ObservableProperty]
        private Color filter24_7BorderColor = Color.FromArgb("#D3D3D3");

        [ObservableProperty]
        private Color filterDojoBorderColor = Color.FromArgb("#D3D3D3");

        [ObservableProperty]
        private Color filterMMABorderColor = Color.FromArgb("#D3D3D3");

        [ObservableProperty]
        private Color filterBoxingBorderColor = Color.FromArgb("#D3D3D3");

        [ObservableProperty]
        private Color filterCrossfitBorderColor = Color.FromArgb("#D3D3D3");

        [ObservableProperty]
        private Color filterPowerliftingBorderColor = Color.FromArgb("#D3D3D3");

        [ObservableProperty]
        private Color filterFemaleOnlyBorderColor = Color.FromArgb("#D3D3D3");

        [ObservableProperty]
        private Color filterRockClimbingBorderColor = Color.FromArgb("#D3D3D3");

        [ObservableProperty]
        private string searchEntry = "";

        [ObservableProperty]
        private bool isRefreshing = false;

        private bool gymLoaded = false;

        private readonly IUserService _userService;

        private readonly IGymService _gymService;

        private readonly IReviewService _reviewService;

        private readonly IFavouriteService _favouriteService;

        private CancellationTokenSource _cancelTokenSource;
        private Microsoft.Maui.Devices.Sensors.Location currentLocation;

        public ListViewPageViewModel(IUserService userService, IGymService gymService, IFavouriteService favouriteService, IReviewService reviewService)
        {
            _userService = userService;
            _gymService = gymService;
            _favouriteService = favouriteService;
            _reviewService = reviewService;
        }

        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {

            IsProcessing = true;
            if (query == null || query.Count == 0) return;

            var userId = int.Parse(HttpUtility.UrlDecode(query["userId"].ToString()));
            User = await _userService.GetUserById(userId);
            if (Gyms.Count == 0)
            {
                await LoadMapWithDeviceLocationAsync();
                await LoadGymInformationsFromDatabase();
                OGGyms = Gyms;
                IsProcessing = true;
                //try
                //{
                //    await SortGymsByDistance();
                //    //GymsWithTypeCount = await GetGymTypeCount(Gyms);
                //}
                //catch (Exception ex)
                //{
                //    await Shell.Current.DisplayAlert("Null Exception", $"{ex}", "OK");
                //}
            }

            //await Task.Run(() => GetFavGyms());
            IsProcessing = false;


        }

        public async Task LoadGymInformationsFromDatabase()
        {
            //IsProcessing = true;
            //Gyms = await _gymService.GetGymList();
            Gyms = Gyms.Take(10).ToList();
            await SortGymsByDistance();
            GymsWithTypeCount = await GetGymTypeCount(Gyms);
            AllGyms = Gyms;
            await GetFavGyms();
            //IsProcessing = false;
        }

        private async Task SortGymsByDistance()
        {
            if (User == null || Gyms == null)
                return;

            foreach (var gym in Gyms)
            {
                gym.Distance = CalculateDistance(User.Latitude, User.Longitude, gym.Latitude, gym.Longitude);
            }

            Gyms = Gyms.OrderBy(gym => gym.Distance).ToList();
            //Gyms = Gyms.Take(10).ToList();
            //GymsWithTypeCount = await GetGymTypeCount(Gyms);
        }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371.0; // Radius of the Earth in kilometers

            var dLat = Deg2Rad(lat2 - lat1);
            var dLon = Deg2Rad(lon2 - lon1);

            var a =
                Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(Deg2Rad(lat1)) * Math.Cos(Deg2Rad(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var distance = R * c;

            return distance;
        }

        private double Deg2Rad(double deg)
        {
            return deg * (Math.PI / 180);
        }

        [RelayCommand]
        private async Task DisplayGymDetails(Gym gym)
        {
            if (gym == null) return;

            await Shell.Current.GoToAsync($"{nameof(GymDetailsPage)}?userId={User.Id}&gymId={gym.Id}");
        }

        [RelayCommand]
        private async Task UnfavGym(int gymId)
        {
            bool result = await Shell.Current.DisplayAlert("Unfavourite", $"Are you sure you want to unfavourite this gym?", "OK", "Cancel");
            if (!result)
            {
                return;
            }

            try
            {
                var favourite = new Favourite
                {
                    UserId = User.Id,
                    GymId = gymId
                };
                await _favouriteService.DeleteFavourite(favourite);

                UpdateGymsFavStatus(gymId, false);

                // GymsWithTypeCount = await GetGymTypeCount(Gyms);
                // await Task.Run(() => GetFavGyms());
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        [RelayCommand]
        private async Task FavoriteGym(int gymId)
        {
            try
            {
                var favourite = new Favourite
                {
                    UserId = User.Id,
                    GymId = gymId
                };
                bool isFav = await _favouriteService.IsFavourite(User.Id, gymId);
                if (isFav)
                {
                    return;
                }
                await _favouriteService.AddFavourite(favourite);

                UpdateGymsFavStatus(gymId, true);

                // GymsWithTypeCount = await GetGymTypeCount(Gyms);
                // await Task.Run(() => GetFavGyms());
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        public async Task<ObservableCollection<GymWithTypeCount>> GetGymTypeCount(List<Gym> gyms)
        {
            ObservableCollection<GymWithTypeCount> gymsWithTypeCount = new();

            if (gyms != null)
            {
                foreach (var gym in gyms)
                {
                    string typeCount = "1";
                    if (gym.Types.Any())
                    {
                        typeCount = Math.Ceiling((double)gym.Types.Count() / 2).ToString();
                    }

                    var reviewCount = await _reviewService.GetTotalReviewByGymId(gym.Id);
                    var isFavorite = await _favouriteService.IsFavourite(User.Id, gym.Id);

                    var gymWithTypeCount = new GymWithTypeCount
                    {
                        Gym = gym,
                        GymTypeCount = typeCount,
                        ReviewCount = reviewCount,
                        IsFavorite = isFavorite
                    };
                    gymsWithTypeCount.Add(gymWithTypeCount);
                }
            }

            return gymsWithTypeCount;
        }

        public class GymWithTypeCount
        {
            public string GymTypeCount { get; set; }
            public Gym Gym { get; set; }
            public int ReviewCount { get; set; }
            public bool IsFavorite { get; set; }
        }

        private async Task GetFavGyms()
        {
            var favouriteGymsWithTypeCounts = new ObservableCollection<GymWithTypeCount>();

            foreach (var gym in GymsWithTypeCount)
            {
                if (gym.IsFavorite)
                {
                    favouriteGymsWithTypeCounts.Add(gym);
                }
            }

            FavouriteGymsWithTypeCounts = favouriteGymsWithTypeCounts;
        }

        private void UpdateGymsFavStatus(int gymId, bool newStatus)
        {
            var targetGymIndex = -1;
            var targetFavGymIndex = -1;
            foreach (var gym in GymsWithTypeCount)
            {
                if (gym.Gym.Id == gymId)
                {
                    targetGymIndex = GymsWithTypeCount.IndexOf(gym);
                    break;
                }
            }
            foreach (var gym in FavouriteGymsWithTypeCounts)
            {
                if (gym.Gym.Id == gymId)
                {
                    targetFavGymIndex = FavouriteGymsWithTypeCounts.IndexOf(gym);
                    break;
                }
            }

            if (targetGymIndex == -1) return;
            if (targetFavGymIndex == -1 && !newStatus) return;
            if (targetFavGymIndex != -1 && newStatus) return;

            if (newStatus)
            {
                FavouriteGymsWithTypeCounts.Add(GymsWithTypeCount[targetGymIndex]);
                var gymWithTypeCount = GymsWithTypeCount[targetGymIndex];
                gymWithTypeCount.IsFavorite = true;
                GymsWithTypeCount[targetGymIndex] = gymWithTypeCount;
            }
            else
            {
                FavouriteGymsWithTypeCounts.RemoveAt(targetFavGymIndex);
                var gymWithTypeCount = GymsWithTypeCount[targetGymIndex];
                gymWithTypeCount.IsFavorite = false;
                GymsWithTypeCount[targetGymIndex] = gymWithTypeCount;
            }
        }



        [RelayCommand]
        public async Task FilterGym(string filterCondition)
        {
            IsProcessing = true;
            if (filterCondition == "24/7")
            {
                if (!Filter24_7BorderColor.Equals(Color.FromArgb("#008000")))
                {
                    Filter24_7BorderColor = Color.FromArgb("#008000");
                    FilterAllBorderColor = Color.FromArgb("#D3D3D3");
                }
                else
                {
                    Filter24_7BorderColor = Color.FromArgb("#D3D3D3");
                }
            }
            if (filterCondition == "Dojo")
            {
                if (!FilterDojoBorderColor.Equals(Color.FromArgb("#008000")))
                {
                    FilterDojoBorderColor = Color.FromArgb("#008000");
                    FilterAllBorderColor = Color.FromArgb("#D3D3D3");
                }
                else
                {
                    FilterDojoBorderColor = Color.FromArgb("#D3D3D3");
                }
            }
            if (filterCondition == "MMA")
            {
                if (!FilterMMABorderColor.Equals(Color.FromArgb("#008000")))
                {
                    FilterMMABorderColor = Color.FromArgb("#008000");
                    FilterAllBorderColor = Color.FromArgb("#D3D3D3");
                }
                else
                {
                    FilterMMABorderColor = Color.FromArgb("#D3D3D3");
                }
            }
            if (filterCondition == "Boxing")
            {
                if (!FilterBoxingBorderColor.Equals(Color.FromArgb("#008000")))
                {
                    FilterBoxingBorderColor = Color.FromArgb("#008000");
                    FilterAllBorderColor = Color.FromArgb("#D3D3D3");
                }
                else
                {
                    FilterBoxingBorderColor = Color.FromArgb("#D3D3D3");
                }
            }
            if (filterCondition == "Crossfit")
            {
                if (!FilterCrossfitBorderColor.Equals(Color.FromArgb("#008000")))
                {
                    FilterCrossfitBorderColor = Color.FromArgb("#008000");
                    FilterAllBorderColor = Color.FromArgb("#D3D3D3");
                }
                else
                {
                    FilterCrossfitBorderColor = Color.FromArgb("#D3D3D3");
                }
            }
            if (filterCondition == "Powerlifting")
            {
                if (!FilterPowerliftingBorderColor.Equals(Color.FromArgb("#008000")))
                {
                    FilterPowerliftingBorderColor = Color.FromArgb("#008000");
                    FilterAllBorderColor = Color.FromArgb("#D3D3D3");
                }
                else
                {
                    FilterPowerliftingBorderColor = Color.FromArgb("#D3D3D3");
                }
            }
            if (filterCondition == "Female Only")
            {
                if (!FilterFemaleOnlyBorderColor.Equals(Color.FromArgb("#008000")))
                {
                    FilterFemaleOnlyBorderColor = Color.FromArgb("#008000");
                    FilterAllBorderColor = Color.FromArgb("#D3D3D3");
                }
                else
                {
                    FilterFemaleOnlyBorderColor = Color.FromArgb("#D3D3D3");
                }
            }
            if (filterCondition == "Rock Climbing")
            {
                if (!FilterRockClimbingBorderColor.Equals(Color.FromArgb("#008000")))
                {
                    FilterRockClimbingBorderColor = Color.FromArgb("#008000");
                    FilterAllBorderColor = Color.FromArgb("#D3D3D3");
                }
                else
                {
                    FilterRockClimbingBorderColor = Color.FromArgb("#D3D3D3");
                }
            }
            if (filterCondition == "All")
            {
                if (!FilterAllBorderColor.Equals(Color.FromArgb("#008000")))
                {
                    FilterAllBorderColor = Color.FromArgb("#008000");
                    Filter24_7BorderColor = Color.FromArgb("#D3D3D3");
                    FilterDojoBorderColor = Color.FromArgb("#D3D3D3");
                    FilterMMABorderColor = Color.FromArgb("#D3D3D3");
                    FilterBoxingBorderColor = Color.FromArgb("#D3D3D3");
                    FilterCrossfitBorderColor = Color.FromArgb("#D3D3D3");
                    FilterPowerliftingBorderColor = Color.FromArgb("#D3D3D3");
                    FilterFemaleOnlyBorderColor = Color.FromArgb("#D3D3D3");
                    FilterRockClimbingBorderColor = Color.FromArgb("#D3D3D3");
                    filterList.Clear();
                    Gyms = AllGyms;
                    await LoadGymInformationsFromDatabase();
                }
                else
                {
                    IsProcessing = false;
                    return;
                }
            }
            else
            {
                if (!filterList.Contains(filterCondition))
                {
                    filterList.Add(filterCondition);
                    Gyms = Gyms.Where(gym => gym.Types.Any(type => type.ToLower().Contains(filterCondition.ToLower()))).ToList();
                    GymsWithTypeCount = await GetGymTypeCount(Gyms);
                    await GetFavGyms();
                }
                else
                {
                    filterList.Remove(filterCondition);
                    if (filterList.Count == 0)
                    {
                        FilterAllBorderColor = Color.FromArgb("#008000");
                        Gyms = AllGyms;
                        await LoadGymInformationsFromDatabase();
                    }
                    else
                    {
                        Gyms = AllGyms;
                        foreach (var filter in filterList)
                        {
                            Gyms = Gyms.Where(gym => gym.Types.Any(type => type.ToLower().Contains(filter.ToLower()))).ToList();
                        }
                        await LoadGymInformationsFromDatabase();
                    }
                }

                IsProcessing = false;
            }

            IsProcessing = false;
        }

        [RelayCommand]
        public async Task SearchAsync()
        {
            if (SearchEntry != "")
            {
                IsProcessing = true;
                Gyms = OGGyms;
                Gyms = Gyms.Where(gym =>
                                    gym.Name.ToLower().Contains(SearchEntry.ToLower()) ||
                                    gym.Utilities.Any(utility => utility.ToLower().Contains(SearchEntry.ToLower())) ||
                                    gym.Types.Any(type => type.ToLower().Contains(SearchEntry.ToLower())) ||
                                    gym.Services.Any(service => service.ToLower().Contains(SearchEntry.ToLower())) ||
                                    gym.Location.ToLower().Contains(SearchEntry.ToLower())
                                 ).ToList();
                GymsWithTypeCount = await GetGymTypeCount(Gyms);
                await GetFavGyms();
                IsProcessing = false;
            }
            return;
        }

        [RelayCommand]
        public async Task Refresh()
        {
            IsRefreshing = false;
            IsProcessing = true;
            FilterAllBorderColor = Color.FromArgb("#008000");
            Filter24_7BorderColor = Color.FromArgb("#D3D3D3");
            FilterDojoBorderColor = Color.FromArgb("#D3D3D3");
            FilterMMABorderColor = Color.FromArgb("#D3D3D3");
            FilterBoxingBorderColor = Color.FromArgb("#D3D3D3");
            FilterCrossfitBorderColor = Color.FromArgb("#D3D3D3");
            FilterPowerliftingBorderColor = Color.FromArgb("#D3D3D3");
            FilterFemaleOnlyBorderColor = Color.FromArgb("#D3D3D3");
            FilterRockClimbingBorderColor = Color.FromArgb("#D3D3D3");
            //Gyms = await _gymService.GetGymList();
            Gyms.Clear();
            await LoadMapWithDeviceLocationAsync();
            await LoadGymInformationsFromDatabase();
            filterList.Clear();
            IsProcessing = false;

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
                    await GetNearbyPlaces();
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error getting device location: ", $"{ex}", "OK");
            }
        }

        public async Task GetNearbyPlaces()
        {
            double latitude = currentLocation.Latitude;
            double longitude = currentLocation.Longitude;
            int radius = 1000;

            var placesService = new PlacesApiService();
            List<Place> nearbyPlaces = await placesService.GetNearbyPlacesAsync(latitude, longitude, radius);
            List<PlaceDetails> placeDetails = await placesService.GetNearbyPlaceDetailsAsync(nearbyPlaces);
            await PopulateDatabase(nearbyPlaces, placeDetails);
            await AddUserLocation();

        }

        public async Task AddUserLocation()
        {
            User.Longitude = currentLocation.Longitude;
            User.Latitude = currentLocation.Latitude;

            await _userService.UpdateUser(User);
        }

        public async Task PopulateDatabase(List<Place> nearbyPlaces, List<PlaceDetails> placeDetails)
        {

            try
            {
                int count = 0;
                foreach (var place in nearbyPlaces)
                {
                    var existingGym = await _gymService.GetGymByPlaceId(place.PlaceId);

                    if (existingGym == null)
                    {
                        if (placeDetails[count].OpeningHours == null)
                        {
                            placeDetails[count].OpeningHours = new OpeningHours(new List<string>());
                        }
                        var gym = new Gym
                        {
                            Name = place.Name,
                            PlaceId = place.PlaceId,
                            Location = place.Vicinity,
                            Latitude = place.Geometry.Location.Latitude,
                            Longitude = place.Geometry.Location.Longitude,
                            PhotoReferenceURL = $"https://maps.googleapis.com/maps/api/place/photo?maxwidth=1280&photoreference={place.FirstPhotoReference}&key=AIzaSyBflN-k9mo3VWLWeX4KNPiaWDUS8Pt6Hdc",
                            PhoneNumber = placeDetails[count].PhoneNumber,
                            OpeningHours = placeDetails[count].OpeningHours.Weekdays,
                        };
                        count++;
                        await _gymService.AddGym(gym);
                        Gyms.Add(gym);
                    }
                    else
                    {
                        existingGym = await UpdateAverageGymRating(existingGym);
                        Gyms.Add(existingGym);
                    }
                }
                
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error populating Database", $"{ex}", "OK");
            }
        }

        public async Task<Gym> UpdateAverageGymRating(Gym gym)
        {

            List<Review> gymReviews = await _reviewService.GetAllGymReviewsByGymId(gym.Id);
            int averageRating = 0;

            foreach (var review in gymReviews)
            {
                averageRating += review.Rating;
            }

            averageRating = (int)Math.Floor((double)averageRating / gymReviews.Count());
            gym.Rating = averageRating;
            await _gymService.UpdateGym(gym);

            return gym;
        }
    }

    public class PlacesApiService
    {
        private const string PlacesApiBaseUrl = "https://maps.googleapis.com/maps/api/place/nearbysearch/json";
        private const string PlaceDetailsApiBaseUrl = "https://maps.googleapis.com/maps/api/place/details/json";
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

        public async Task<List<PlaceDetails>> GetNearbyPlaceDetailsAsync(List<Place> nearbyPlaces)
        {
            List<PlaceDetails> placeDetails = new();
            try
            {
                using (var httpClient = new HttpClient())
                {
                    foreach (var place in nearbyPlaces)
                    {
                        string requestUrl = $"{PlaceDetailsApiBaseUrl}?place_id={place.PlaceId}&key={ApiKey}";

                        HttpResponseMessage response = await httpClient.GetAsync(requestUrl);

                        if (response.IsSuccessStatusCode)
                        {
                            string jsonContent = await response.Content.ReadAsStringAsync();

                            // Deserialize 
                            var placeDetailsResponse = JsonConvert.DeserializeObject<PlaceDetailsResponse>(jsonContent);

                            if (placeDetailsResponse?.Result != null)
                            {
                                //if (placeDetailsResponse.Result.OpeningHours == null)
                                //{
                                //    placeDetailsResponse.Result.OpeningHours.Weekdays = new List<string>();
                                //}
                                placeDetails.Add(placeDetailsResponse.Result);
                            }
                        }
                    }

                }
                return placeDetails;
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

    public class PlaceDetailsResponse
    {
        [JsonProperty("result")]
        public PlaceDetails Result { get; set; }

    }

    public class PlaceDetails
    {
        [JsonProperty("opening_hours")]
        public OpeningHours OpeningHours { get; set; }

        [JsonProperty("international_phone_number")]
        public string PhoneNumber { get; set; }
    }

    public class OpeningHours
    {
        [JsonProperty("weekday_text")]
        public List<string> Weekdays { get; set; }
        public OpeningHours(List<string> weekdays)
        {
            Weekdays = weekdays;
        }
    }
}