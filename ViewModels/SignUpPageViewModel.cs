using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GymSpotter.Models;
using GymSpotter.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSpotter.ViewModels
{
    public partial class SignUpPageViewModel : ObservableObject
    {
        private readonly IUserService _userService;
        public SignUpPageViewModel(IUserService userService)
        {
            _userService = userService;
        }

        [ObservableProperty]
        private string firstName;

        [ObservableProperty]
        private string lastName;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private string confirmPassword;

        [ObservableProperty]
        private bool isProcessing;

        [RelayCommand]
        public async Task SignUp()
        {
            try
            {

                IsProcessing = true;
                if (Password != ConfirmPassword)
                {
                    await Shell.Current.DisplayAlert("Error", "Password doesn't match", "OK");
                    return;
                }

                var user = await _userService.GetUserByEmail(Email);
                if (user != null)
                {
                    await Shell.Current.DisplayAlert("Error", "Email is already taken", "OK");
                    return;
                }

                var newUser = new User
                {
                    FirstName = FirstName,
                    LastName = LastName,
                    Email = Email,
                    Password = Password
                };

                int result = await _userService.AddUser(newUser);

                if (result > 0)
                {
                    await Shell.Current.DisplayAlert("Successs", "Successful registration", "OK");
                    await Shell.Current.GoToAsync("//Login");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Fail", "Unsucessful registration", "OK");

                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");

            }
            finally
            {
                IsProcessing = false;
            }
        }
    }
}
