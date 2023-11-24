using CommunityToolkit.Maui.Views;
using GymSpotter.ViewModels;
using GymSpotter.Services;

namespace GymSpotter.Views;

public partial class GymDetailsPage : ContentPage
{
    public GymDetailsPage(GymDetailsPageViewModel viewModel)
    {
        InitializeComponent();
        this.BindingContext = viewModel;
    }
    private void AddReviewCommand(object sender, EventArgs e)
    {
        this.ShowPopup(new AddReviewPage());
    }
}