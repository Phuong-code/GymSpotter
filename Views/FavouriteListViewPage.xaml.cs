using GymSpotter.ViewModels;

namespace GymSpotter.Views;

public partial class FavouriteListViewPage : ContentPage {

    public FavouriteListViewPage(ListViewPageViewModel viewModel) {
        InitializeComponent();

        this.BindingContext = viewModel;
    }
}