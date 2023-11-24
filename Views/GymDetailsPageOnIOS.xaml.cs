using CommunityToolkit.Maui.Views;
using GymSpotter.ViewModels;

namespace GymSpotter.Views;

public partial class GymDetailsPageOnIOS : ContentPage
{
    public GymDetailsPageOnIOS(GymDetailsPageViewModel viewModel)
    {
		InitializeComponent();
        this.BindingContext = viewModel;
    }
    private void AddReviewCommand(object sender, EventArgs e)
    {
        this.ShowPopup(new AddReviewPage());
    }
}
