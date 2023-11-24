using CommunityToolkit.Maui.Views;
using GymSpotter.ViewModels;

namespace GymSpotter.Views;

public partial class UserDetailsPageOnIOS : ContentPage
{
	public UserDetailsPageOnIOS(UserDetailsPageViewModel viewModel)
	{
		InitializeComponent();
		this.BindingContext = viewModel;
	}

}
