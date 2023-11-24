using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GymSpotter.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSpotter.ViewModels {

    public partial class LoginPageViewModel : ObservableObject {
        private readonly IUserService _userService;

        public LoginPageViewModel(IUserService userService) {
            _userService = userService;
        }

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private bool isProcessing;

        [RelayCommand]
        public async Task Login() {
            IsProcessing = true;
            try {
                if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password)) {
                    await Shell.Current.DisplayAlert("Error", "Please enter both email and password", "OK");
                    return;
                }

                var user = await _userService.GetUserByEmail(Email);

                if (user != null) {
                    if (Password == user.Password) {
                        await Shell.Current.GoToAsync($"//Dashboard");

                        await Shell.Current.GoToAsync($"//FavouriteListViewPage?userId={user.Id}");
                        await Shell.Current.GoToAsync($"//UserDetailsPage?userId={user.Id}");
                        await Shell.Current.GoToAsync($"//MapPage?userId={user.Id}");
                        await Shell.Current.GoToAsync($"//ListViewPage?userId={user.Id}");

                    } else {
                        await Shell.Current.DisplayAlert("Error", "Invalid email or password", "OK");
                    }
                } else {
                    await Shell.Current.DisplayAlert("Error", "Invalid email or password", "OK");
                }
            } catch (Exception ex) {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            } finally {
                IsProcessing = false;
            }
        }
    }
}