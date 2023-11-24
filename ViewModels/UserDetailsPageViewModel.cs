using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GymSpotter.Models;
using GymSpotter.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using CommunityToolkit.Maui.Views;
using GymSpotter.Views;

namespace GymSpotter.ViewModels
{
    public partial class UserDetailsPageViewModel : ObservableObject, IQueryAttributable
    {
        [ObservableProperty]
        private User user;

        [ObservableProperty]
        private string confirmPassword;

        [ObservableProperty]
        private List<Gym> gyms = new();

        [ObservableProperty]
        private int totalGymReview;

        [ObservableProperty]
        private List<Review> userAllReviews;

        [ObservableProperty]
        private List<GymReviewWithGymName> gymReviewWithGymNameList;

        [ObservableProperty]
        private List<GymDetails> gymDetailsList;

        [ObservableProperty]
        private EditProfilePage editProfilePage;

        [ObservableProperty]
        private EditGymPage editGymPage;

        [ObservableProperty]
        private GymDetailsPopUp gymDetailsPopUpBinding;

        [ObservableProperty]
        private bool isProcessing;

        [ObservableProperty]
        private bool isVisible;

        private readonly IUserService _userService;

        private readonly IGymService _gymService;

        private readonly IReviewService _reviewService;

        private readonly ListViewPageViewModel _listViewPageViewModel;

        public UserDetailsPageViewModel(IUserService userService, IGymService gymService, IReviewService reviewService, ListViewPageViewModel listViewPageViewModel)
        {
            _userService = userService;
            _gymService = gymService;
            _reviewService = reviewService;
            _listViewPageViewModel = listViewPageViewModel;
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query == null || query.Count == 0) return;

            var userId = int.Parse(HttpUtility.UrlDecode(query["userId"].ToString()));
            if (Gyms.Count == 0)
            {
                LoadInformation(userId);
            }
        }

        public async void LoadInformation(int userId)
        {
            IsProcessing = true;
            IsVisible = false;

            User = await _userService.GetUserById(userId);

            Gyms = await _gymService.GetGymByOwnerId(userId);

            UserAllReviews = await _reviewService.GetAllUserReviewsByUserId(userId);

            GymReviewWithGymNameList = await GetUserAllReviewsWithGymNameList(UserAllReviews);

            GymDetailsList = await GetGymDetailsList(Gyms);

            IsProcessing = false;
            IsVisible = true;
        }

        private async Task<List<GymReviewWithGymName>> GetUserAllReviewsWithGymNameList(List<Review> reviews)
        {
            List<GymReviewWithGymName> userAllReviewsWithGymNameList = new List<GymReviewWithGymName>();

            if (reviews != null)
            {
                foreach (var review in reviews)
                {
                    Gym gym = await _gymService.GetGymById(review.GymId);
                    var gymReview = new GymReviewWithGymName { Gym = gym, Review = review };
                    userAllReviewsWithGymNameList.Add(gymReview);
                }
            }

            return userAllReviewsWithGymNameList;
        }

        public async Task<List<GymDetails>> GetGymDetailsList(List<Gym> gyms)
        {
            List<GymDetails> gymDetailsListResult = new();

            if (gyms != null)
            {
                foreach (var gym in gyms)
                {
                    string typeCount = Math.Ceiling((double)gym.Types.Count() / 2).ToString();
                    var reviewCount = await _reviewService.GetTotalReviewByGymId(gym.Id);
                    var gymWithTypeCount = new GymDetails
                    {
                        Gym = gym,
                        GymTypeCount = typeCount,
                        ReviewCount = reviewCount
                    };
                    gymDetailsListResult.Add(gymWithTypeCount);
                }
            }

            return gymDetailsListResult;
        }

        public class GymDetails
        {
            public string GymTypeCount { get; set; }
            public Gym Gym { get; set; }
            public int ReviewCount { get; set; }
        }

        public class GymReviewWithGymName
        {
            public Gym Gym { get; set; }
            public Review Review { get; set; }
        }

        public class GymDetailsPopUp
        {
            public Gym Gym { get; set; }
            public string UtilitiesString { get; set; } = "";
            public string ServicesString { get; set; } = "";
            public bool Is24_7 { get; set; } = false;
            public bool IsFemaleOnly { get; set; } = false;
            public bool IsCrossfit { get; set; } = false;
            public bool IsDojo { get; set; } = false;
            public bool IsMMA { get; set; } = false;
            public bool IsBoxing { get; set; } = false;
            public bool IsPowerLifting { get; set; } = false;
            public bool IsRockClimbing { get; set; } = false;
        }

        [RelayCommand]
        public async void OpenEditProfilePopUp()
        {
            ConfirmPassword = User.Password;
#if ANDROID
            EditProfilePage = new EditProfilePage();
            Shell.Current.CurrentPage.ShowPopup(EditProfilePage);
#elif IOS
            var editProfilePageOnIOS = new EditProfilePageOnIOS(this);
            await Shell.Current.CurrentPage.Navigation.PushModalAsync(editProfilePageOnIOS);
#endif
        }

        [RelayCommand]
        public async Task ViewDetails(Gym gym)
        {
            if (gym == null) return;
            await Shell.Current.GoToAsync($"{nameof(GymDetailsPage)}?userId={User.Id}&gymId={gym.Id}");

        }

        [RelayCommand]
        public async Task SaveProfile()
        {
            if (User.Password != ConfirmPassword)
            {
                await Shell.Current.DisplayAlert("Error", "Password doesn't match", "OK");
                return;
            }
            var editedUser = new User
            {
                Id = User.Id,
                Email = User.Email,
                Password = User.Password,
                FirstName = User.FirstName,
                LastName = User.LastName,
                PhotoUrl = User.PhotoUrl,
            };

            int rowsAffected = await _userService.UpdateUser(editedUser);
            if (rowsAffected > 0)
            {
                await Shell.Current.DisplayAlert("Success", "Successfully update profile", "OK");
                User = editedUser;
                //LoadInformation(editedUser.Id);
#if ANDROID
                EditProfilePage.Close();
#elif IOS
                await Shell.Current.CurrentPage.Navigation.PopModalAsync();
#endif

            }
            else
            {
                await Shell.Current.DisplayAlert("Fail", "Fail to update profile", "OK");
            }
        }

        [RelayCommand]
        public async void CloseEditProfilePopUp()
        {
#if ANDROID
            EditProfilePage.Close();
#elif IOS
            await Shell.Current.CurrentPage.Navigation.PopModalAsync();
#endif
        }

        [RelayCommand]
        public async Task OpenEditGymPopUp(Gym gym)
        {
            if (gym == null) return;
            GymDetailsPopUpBinding = new GymDetailsPopUp();
            GymDetailsPopUpBinding.Gym = GymDetailsList.FirstOrDefault(gymDetail => gymDetail.Gym == gym).Gym;

            GymDetailsPopUpBinding.Is24_7 = GymDetailsPopUpBinding.Gym.Types.Contains("24/7");
            GymDetailsPopUpBinding.IsDojo = GymDetailsPopUpBinding.Gym.Types.Contains("Dojo");
            GymDetailsPopUpBinding.IsMMA = GymDetailsPopUpBinding.Gym.Types.Contains("MMA");
            GymDetailsPopUpBinding.IsBoxing = GymDetailsPopUpBinding.Gym.Types.Contains("Boxing");
            GymDetailsPopUpBinding.IsCrossfit = GymDetailsPopUpBinding.Gym.Types.Contains("Crossfit");
            GymDetailsPopUpBinding.IsPowerLifting = GymDetailsPopUpBinding.Gym.Types.Contains("Powerlifting");
            GymDetailsPopUpBinding.IsFemaleOnly = GymDetailsPopUpBinding.Gym.Types.Contains("Female Only");
            GymDetailsPopUpBinding.IsRockClimbing = GymDetailsPopUpBinding.Gym.Types.Contains("Rock Climbing");

            GymDetailsPopUpBinding.UtilitiesString = String.Join(", ", GymDetailsPopUpBinding.Gym.Utilities);
            GymDetailsPopUpBinding.ServicesString = String.Join(", ", GymDetailsPopUpBinding.Gym.Services);

#if ANDROID
            EditGymPage = new EditGymPage();
            Shell.Current.CurrentPage.ShowPopup(EditGymPage);
#elif IOS
            var editGymPageOnIOS = new EditGymPageOnIOS(this);
            await Shell.Current.CurrentPage.Navigation.PushModalAsync(editGymPageOnIOS);
#endif
        }

        [RelayCommand]
        public async Task SaveEditedGym()
        {
            List<string> editedTypes = new List<string>();


            if (GymDetailsPopUpBinding.Is24_7 == true)
            {
                editedTypes.Add("24/7");
            }
            if (GymDetailsPopUpBinding.IsDojo == true)
            {
                editedTypes.Add("Dojo");
            }
            if (GymDetailsPopUpBinding.IsMMA == true)
            {
                editedTypes.Add("MMA");
            }
            if (GymDetailsPopUpBinding.IsBoxing == true)
            {
                editedTypes.Add("Boxing");
            }
            if (GymDetailsPopUpBinding.IsCrossfit == true)
            {
                editedTypes.Add("Crossfit");
            }
            if (GymDetailsPopUpBinding.IsPowerLifting == true)
            {
                editedTypes.Add("Powerlifting");
            }
            if (GymDetailsPopUpBinding.IsFemaleOnly == true)
            {
                editedTypes.Add("Female Only");
            }
            if (GymDetailsPopUpBinding.IsRockClimbing == true)
            {
                editedTypes.Add("Rock Climbing");
            }

            List<string> editUtilitiesList = GymDetailsPopUpBinding.UtilitiesString
                                                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                    .Select(item => item.Trim())
                                                    .ToList();

            List<string> editedServicesList = GymDetailsPopUpBinding.ServicesString
                                                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                    .Select(item => item.Trim())
                                                    .ToList();

            Gym editedGym = new Gym
            {
                Id = GymDetailsPopUpBinding.Gym.Id,
                OwnerId = GymDetailsPopUpBinding.Gym.OwnerId,
                Name = GymDetailsPopUpBinding.Gym.Name,
                PlaceId = GymDetailsPopUpBinding.Gym.PlaceId,
                Rating = GymDetailsPopUpBinding.Gym.Rating,
                Description = GymDetailsPopUpBinding.Gym.Description,
                Location = GymDetailsPopUpBinding.Gym.Location,
                Types = editedTypes,
                Utilities = editUtilitiesList,
                Services = editedServicesList,
                PhotoReferenceURL = GymDetailsPopUpBinding.Gym.PhotoReferenceURL,
            };
            int rowsAffected = await _gymService.UpdateGym(editedGym);

            if (rowsAffected > 0)
            {
#if ANDROID
                await Shell.Current.DisplayAlert("Success", "Successfully update gym details", "OK");
#endif
                var gymFound = Gyms.Where(g => g.Id == editedGym.Id).FirstOrDefault();
                var gymIndex = Gyms.IndexOf(gymFound);
                Gyms[gymIndex] = editedGym;
                GymDetailsList = await GetGymDetailsList(Gyms);
                //LoadInformation(User.Id);

#if ANDROID
                EditGymPage.Close();
#elif IOS
                await Shell.Current.CurrentPage.Navigation.PopModalAsync();
#endif
            }
            else
            {
                await Shell.Current.DisplayAlert("Fail", "Fail to update gym details", "OK");
            }
        }

        [RelayCommand]
        public async Task CloseEditGymPopUp()
        {
#if ANDROID
            EditGymPage.Close();
#elif IOS
            await Shell.Current.CurrentPage.Navigation.PopModalAsync();
#endif
        }

        [RelayCommand]
        public async Task Logout()
        {
            Gyms.Clear();
            _listViewPageViewModel.Gyms.Clear();
            await Shell.Current.GoToAsync($"//Login");
        }

        [RelayCommand]
        public async Task DisplayGymDetails(Gym gym)
        {
            if (gym == null) return;

            await Shell.Current.GoToAsync($"{nameof(GymDetailsPage)}?userId={User.Id}&gymId={gym.Id}");
        }
    }
}
