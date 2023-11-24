using GymSpotter.ViewModels;

namespace GymSpotter.Views;

public partial class SignUpPage : ContentPage
{
	public SignUpPage(SignUpPageViewModel viewModel)
	{
		InitializeComponent();

        this.BindingContext = viewModel;
    }

    private async void TapGestureRecognizer_Tapped_For_SignIn(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//Login");
    }
}