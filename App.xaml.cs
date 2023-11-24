using GymSpotter.MockDB;
using GymSpotter.Services;

namespace GymSpotter
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }

        protected override async void OnStart()
        {
            base.OnStart();

            var mockUserDB = MauiProgram.CreateMauiApp().Services.GetRequiredService<MockUserDB>();
            var mockGymDB = MauiProgram.CreateMauiApp().Services.GetRequiredService<MockGymDB>();
            var mockReviewDB = MauiProgram.CreateMauiApp().Services.GetRequiredService<MockReviewDB>();
            var mockFavouriteDB = MauiProgram.CreateMauiApp().Services.GetRequiredService<MockFavouriteDB>();


            await mockUserDB.AddMockUserToDatabase();
            await mockGymDB.AddMockGymToDatabase();
            await mockReviewDB.AddMockReviewToDatabase();
            await mockGymDB.UpdateAverageGymRatings();
            await mockFavouriteDB.AddMockFavouriteToDatabase();
        }
    }
}