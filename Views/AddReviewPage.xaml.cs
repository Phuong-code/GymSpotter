using CommunityToolkit.Maui.Views;
using System.Runtime.CompilerServices;

namespace GymSpotter.Views;

public partial class AddReviewPage : Popup
{
	public AddReviewPage()
	{
		InitializeComponent();
	}

	private void CancelAddReview(object sender, EventArgs e)
	{
		this.Close();
	}
}