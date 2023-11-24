using GymSpotter.Models;
using GymSpotter.Services;
using GymSpotter.ViewModels;

namespace GymSpotter.Views;

public partial class ListViewPage : ContentPage {

    public ListViewPage(ListViewPageViewModel viewModel) {
        InitializeComponent();

        this.BindingContext = viewModel;
    }
       
}