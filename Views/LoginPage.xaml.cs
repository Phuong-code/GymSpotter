using GymSpotter.ViewModels;

namespace GymSpotter.Views;

public partial class LoginPage : ContentPage
{
	public LoginPage(LoginPageViewModel viewModel)
	{
		InitializeComponent();

        this.BindingContext = viewModel;
    }
    private async void TapGestureRecognizer_Tapped_For_SignUP(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("SignUpPage");
    }
}