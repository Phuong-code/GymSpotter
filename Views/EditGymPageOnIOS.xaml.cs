using GymSpotter.ViewModels;

namespace GymSpotter.Views;

public partial class EditGymPageOnIOS : ContentPage
{
	public EditGymPageOnIOS(UserDetailsPageViewModel viewModel)
	{
		InitializeComponent();

		this.BindingContext = viewModel;
	}

    void Button_Clicked(System.Object sender, System.EventArgs e)
    {
		Shell.Current.DisplayAlert("what","what", "what");
    }
}
