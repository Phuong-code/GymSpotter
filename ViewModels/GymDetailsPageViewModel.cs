using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using GymSpotter.Services;
using GymSpotter.Models;
using GymSpotter.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace GymSpotter.ViewModels {

    public partial class GymDetailsPageViewModel : ObservableObject, IQueryAttributable {

        [ObservableProperty]
        private User user;

        [ObservableProperty]
        private Gym gym;

        [ObservableProperty]
        private string utilities;

        [ObservableProperty]
        private string services;

        [ObservableProperty]
        private string openingHours;

        [ObservableProperty]
        private List<Review> gymReviews;

        [ObservableProperty]
        private List<ReviewWithUserName> reviewWithUserNames;

        [ObservableProperty]
        private string gymTypeCount = "1";

        [ObservableProperty]
        private AddReviewPage addReviewPage;

        [ObservableProperty]
        private Review userReview;

        [ObservableProperty]
        private Boolean reviewExist = true;

        [ObservableProperty]
        private AverageReview avgReview;

        [ObservableProperty]
        private string photoReferenceURL;

        [ObservableProperty]
        private bool isFavourite;

        private readonly IUserService _userService;

        private readonly IGymService _gymService;

        private readonly IReviewService _reviewService;

        private readonly IFavouriteService _favouriteService;

        private readonly ListViewPageViewModel _listViewPageViewModel;

        private ICommand _openDirectionsCommand;

        public ICommand OpenDirectionsCommand {
            get {
                if (_openDirectionsCommand == null) {
                    _openDirectionsCommand = new Command(async () => await ExecuteOpenDirectionsCommand());
                }
                return _openDirectionsCommand;
            }
        }

        public GymDetailsPageViewModel(IUserService userService, IGymService gymService, IReviewService reviewService, IFavouriteService favouriteService, ListViewPageViewModel listViewPageViewModel) {
            _userService = userService;
            _gymService = gymService;
            _reviewService = reviewService;
            _favouriteService = favouriteService;
            _listViewPageViewModel = listViewPageViewModel;
        }

        public async void ApplyQueryAttributes(IDictionary<string, object> query) {
            if (query == null || query.Count == 0) return;

            var userId = int.Parse(HttpUtility.UrlDecode(query["userId"].ToString()));
            var gymId = int.Parse(HttpUtility.UrlDecode(query["gymId"].ToString()));

            User = await _userService.GetUserById(userId);
            Gym = await _gymService.GetGymById(gymId);

            if (Gym.Types.Any()) {
                GymTypeCount = Math.Ceiling((double)Gym.Types.Count() / 2).ToString();
            }
            Utilities = string.Join(", ", Gym.Utilities);
            Services = string.Join(", ", Gym.Services);
            OpeningHours = string.Join("\n", Gym.OpeningHours);
            GymReviews = await _reviewService.GetAllGymReviewsByGymId(gymId);
            ReviewWithUserNames = await GetUserNameForAllReviews(GymReviews);

            PhotoReferenceURL = Gym.PhotoReferenceURL;

            var favourite = await _favouriteService.IsFavourite(userId, gymId);
            IsFavourite = favourite;

            
            if (GymReviews.Count() != 0)
            {
                AvgReview = await CalculateAverageReviews(GymReviews);
            }
            else
            {
                AvgReview = new AverageReview
                {
                    AverageRating = 0,
                    AverageCleanliness = 0,
                    AveragePrice = 0,
                    AverageService = 0
                };
            }
            UserReview = await _reviewService.GetReviewByUserIdAndGymId(userId, gymId);
            if (UserReview == null)
            {
                UserReview = new Review
                {
                    GymId = gymId,
                    UserId = userId,
                    Rating = 0,
                    Cleanliness = 0,
                    Service = 0,
                    Price = 0,
                    Description = "",
                    Date = DateTime.Now,
                };
                ReviewExist = false;
            }
            //await Shell.Current.DisplayAlert("Clicked", $"GymType: {GymTypeCount}", "OK");
            //TotalGymReview = await _reviewService.GetTotalReviewByGymId(Gym.Id);
        }

        private async Task ExecuteOpenDirectionsCommand() {
            if (Gym != null) {
                var location = new Microsoft.Maui.Devices.Sensors.Location(Gym.Latitude, Gym.Longitude);
                var options = new MapLaunchOptions { Name = Gym.Name };

                await Map.OpenAsync(location, options);
            } else {
                await Shell.Current.DisplayAlert("Error", "Gym location not available", "OK");
            }
        }

        [RelayCommand]
        public void OpenAddReviewPopup()
        {
            AddReviewPage = new AddReviewPage();
            Shell.Current.CurrentPage.ShowPopup(AddReviewPage);
        }

        [RelayCommand]
        public async Task AddOrUpdateReview()
        {

            int reviewAddedOrUpdated;

            if (ReviewExist)
            {
                reviewAddedOrUpdated = await _reviewService.UpdateReview(UserReview);
                // User is editing review
                if (reviewAddedOrUpdated > 0)
                {
#if ANDROID
                    await Shell.Current.DisplayAlert("Success", "Review updated!", "OK");
#endif
                    GymReviews = await _reviewService.GetAllGymReviewsByGymId(Gym.Id);
                    ReviewWithUserNames = await GetUserNameForAllReviews(GymReviews);
                    AvgReview = await CalculateAverageReviews(GymReviews);
                    var gymFound = _listViewPageViewModel.Gyms.Where(g => g.Id == Gym.Id).FirstOrDefault();
                    var gymIndex = _listViewPageViewModel.Gyms.IndexOf(gymFound);
                    Gym.Rating = AvgReview.AverageRating;
                    await _gymService.UpdateGym(Gym);
                    _listViewPageViewModel.Gyms[gymIndex] = Gym;
                    await _listViewPageViewModel.LoadGymInformationsFromDatabase();
                    //_listViewPageViewModel.Gyms.Clear();
                    AddReviewPage.Close();
                }
                else
                {
                    await Shell.Current.DisplayAlert("Fail", "Failed to edit review", "OK");
                }
            }
            // User adding a new review
            else
            {
                reviewAddedOrUpdated = await _reviewService.AddReview(UserReview);
                if (reviewAddedOrUpdated > 0)
                {
#if ANDROID
                    await Shell.Current.DisplayAlert("Success", "Review added!", "OK");
#endif
                    GymReviews = await _reviewService.GetAllGymReviewsByGymId(Gym.Id);
                    ReviewWithUserNames = await GetUserNameForAllReviews(GymReviews);
                    ReviewExist = true;
                    AvgReview = await CalculateAverageReviews(GymReviews);
                    var gymFound = _listViewPageViewModel.Gyms.Where(g => g.Id == Gym.Id).FirstOrDefault();
                    var gymIndex = _listViewPageViewModel.Gyms.IndexOf(gymFound);
                    Gym.Rating = AvgReview.AverageRating;
                    await _gymService.UpdateGym(Gym);
                    _listViewPageViewModel.Gyms[gymIndex] = Gym;
                    await _listViewPageViewModel.LoadGymInformationsFromDatabase();
                    //_listViewPageViewModel.Gyms.Clear();
                    AddReviewPage.Close();
                }
                else
                {
                    await Shell.Current.DisplayAlert("Fail", "Failed to add review", "OK");
                }
            }

        }

        public class ReviewWithUserName
        {
            public string UserName { get; set; }
            public Review Review { get; set; }
        }

        private async Task<List<ReviewWithUserName>> GetUserNameForAllReviews(List<Review> reviews) {
            List<ReviewWithUserName> reviewWithUserName = new List<ReviewWithUserName>();

            if (reviews != null) {
                foreach (var review in reviews) {
                    User user = await _userService.GetUserById(review.UserId);
                    string userName = user.FirstName + " " + user.LastName;
                    var userGymReview = new ReviewWithUserName { UserName = userName, Review = review };
                    reviewWithUserName.Add(userGymReview);
                }
            }

            return reviewWithUserName;
        }

        public class AverageReview
        {
            public int AverageRating { get; set; }
            public int AverageCleanliness { get; set; }
            public int AveragePrice { get; set; }
            public int AverageService { get; set; }
            public int ReviewCount {  get; set; }
        }

        private async Task<AverageReview> CalculateAverageReviews(List<Review> reviews)
        {
            int averageRating = 0;
            int averageCleanliness = 0;
            int averagePrice = 0;
            int averageService = 0;
            int reviewCount = reviews.Count();

            foreach (var review in reviews)
            {
                averageRating += review.Rating;
                averageCleanliness += review.Cleanliness;
                averagePrice += review.Price;
                averageService += review.Service;
            }

            averageRating = (int)Math.Floor((double)averageRating / reviews.Count());
            averageCleanliness = (int)Math.Floor((double)averageCleanliness / reviews.Count());
            averagePrice = (int)Math.Floor((double)averagePrice / reviews.Count());
            averageService = (int)Math.Floor((double)averageService / reviews.Count());

            AverageReview averageReviews = new AverageReview
            {
                AverageRating = averageRating,
                AverageCleanliness = averageCleanliness,
                AveragePrice = averagePrice,
                AverageService = averageService,
                ReviewCount = reviewCount
            };

            return averageReviews;
        }
        [RelayCommand]
        private async Task UnfavGym(int gymId) {
            bool result = await Shell.Current.DisplayAlert("Unfavourite", $"Are you sure you want to unfavourite this gym?", "OK", "Cancel");
            if (!result) {
                return;
            }

            try {
                var favourite = new Favourite {
                    UserId = User.Id,
                    GymId = gymId
                };
                await _favouriteService.DeleteFavourite(favourite);
                await _listViewPageViewModel.LoadGymInformationsFromDatabase();

                IsFavourite = false;
            } catch (Exception ex) {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        [RelayCommand]
        private async Task FavoriteGym(int gymId) {
            try {
                var favourite = new Favourite {
                    UserId = User.Id,
                    GymId = gymId
                };
                bool isFav = await _favouriteService.IsFavourite(User.Id, gymId);
                if (isFav) {
                    return;
                }
                await _favouriteService.AddFavourite(favourite);
                await _listViewPageViewModel.LoadGymInformationsFromDatabase();

                IsFavourite = true;
            } catch (Exception ex) {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}