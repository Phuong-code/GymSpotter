using GymSpotter.Views;

namespace GymSpotter {

    public partial class AppShell : Shell {

        public AppShell() {
            InitializeComponent();

            Routing.RegisterRoute("SignUpPage", typeof(SignUpPage));
#if ANDROID
            Routing.RegisterRoute("GymDetailsPage", typeof(GymDetailsPage));
#elif IOS
            Routing.RegisterRoute("GymDetailsPage", typeof(GymDetailsPageOnIOS));
            Routing.RegisterRoute("EditProfilePage", typeof(EditProfilePageOnIOS));
            Routing.RegisterRoute("EditGymPage", typeof(EditGymPageOnIOS));
#endif
        }
    }
}