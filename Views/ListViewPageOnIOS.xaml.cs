using GymSpotter.ViewModels;

namespace GymSpotter.Views;

public partial class ListViewPageOnIOS : ContentPage
{
	public ListViewPageOnIOS(ListViewPageViewModel viewModel)
	{
		InitializeComponent();
		this.BindingContext = viewModel;
	}
}
