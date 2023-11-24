using GymSpotter.ViewModels;

namespace GymSpotter.Views;

public partial class FavouriteListViewPageOnIOS : ContentPage
{
	public FavouriteListViewPageOnIOS(ListViewPageViewModel viewModel)
	{
		InitializeComponent();

        this.BindingContext = viewModel;
    }
}
