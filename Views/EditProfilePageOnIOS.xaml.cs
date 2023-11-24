using GymSpotter.ViewModels;

namespace GymSpotter.Views;

public partial class EditProfilePageOnIOS : ContentPage
{
	public EditProfilePageOnIOS(UserDetailsPageViewModel viewModel)
	{
		InitializeComponent();

		this.BindingContext = viewModel;
	}
}
