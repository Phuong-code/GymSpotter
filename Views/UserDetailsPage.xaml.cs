using CommunityToolkit.Maui.Views;
using GymSpotter.ViewModels;

namespace GymSpotter.Views;

public partial class UserDetailsPage : ContentPage
{
    public UserDetailsPage(UserDetailsPageViewModel viewModel)
    {
        InitializeComponent();

        this.BindingContext = viewModel;
    }
}